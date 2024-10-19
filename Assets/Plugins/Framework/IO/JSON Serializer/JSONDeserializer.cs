using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using Framework;
using UnityEngine;
using UnityEngine.Assertions;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Framework
{

    public class JSONDeserializer
    {
        private static Dictionary<Type, JSONDeserializationMethod> _customDeserializationMethods = new Dictionary<Type, JSONDeserializationMethod>();
        private static Dictionary<Type, CustomJSONSerializer> _customSerializers = new Dictionary<Type, CustomJSONSerializer>();

        private Dictionary<int, Object> _unityObjectsByID = new Dictionary<int, Object>();
        private Queue<UnityObjectReference> _unresolvedUnityObjectReferences = new Queue<UnityObjectReference>();
        private Dictionary<int, object> _deserializedReferenceTypeObjects = new Dictionary<int, object>();
        private bool _deserializationSessionStarted;
        private Assembly _callingAssembly;

        public void BeginDeserialization()
        {
            if (_deserializationSessionStarted)
            {
                throw new JSONSerializationException("JSON deserialization session already started. Are you sure EndDeserialization() was properly called after the last session?");
            }

            _deserializationSessionStarted = true;
        }

        public void EndDeserialization()
        {
            if (!_deserializationSessionStarted)
            {
                throw new JSONSerializationException(
                    "JSON serialization session was never started. Are you sure EndDeserialization() hasn't already been called and that a session was properly started?");
            }


            while (_unresolvedUnityObjectReferences.Count > 0)
            {
                _unresolvedUnityObjectReferences.Dequeue().Resolve(_unityObjectsByID);
            }

            _callingAssembly = null;
            _unityObjectsByID.Clear();
            _deserializedReferenceTypeObjects.Clear();
            _deserializationSessionStarted = false;
        }

        private void EnsureInSession()
        {
            if (!_deserializationSessionStarted)
            {
                throw new JSONSerializationException(
                    "JSON serialization session not started! Make sure you call BeginDeserialization() before you start serializing and EndDeserialization() when you are finished.");
            }
        }

        public static void AddCustomDeserializationMethod(Type type, JSONDeserializationMethod method)
        {
            _customDeserializationMethods[type] = method;
        }

        /// <summary>
        /// Populates the field's of a GameObject's comopnents using values from a JSON array.
        /// </summary>
        /// <param name="gameObject">The GameObject to populate</param>
        /// <param name="jsonFields">The JSON array, one JSON object per saved comoponent</param>
        public void LoadGameObjectDataFromJSON(GameObject gameObject, JSONObject jsonObject, bool addMissingComponents)
        {
            EnsureInSession();

            if (jsonObject.TryGetFieldValue("@ID", out int id))
            {
                _unityObjectsByID.Add(id, gameObject);
            }

            if (jsonObject.TryGetFieldValue("@Name", out string name))
            {
                gameObject.name = name;
            }

            if (jsonObject.TryGetFieldValue("@Tag", out string tag))
            {
                gameObject.tag = tag;
            }

            if (jsonObject.TryGetFieldValue("@Layer", out int layer))
            {
                gameObject.layer = layer;
            }

            JSONValue[] componentArray = (JSONValue[])jsonObject.GetField("@Components").JSONValue.GetRawObject();
            List<JSONObject> unloadedComponents = new List<JSONObject>();

            for (int i = 0; i < componentArray.Length; i++)
            {
                unloadedComponents.Add((JSONObject)componentArray[i]);
            }

            List<Component> components = new List<Component>();
            gameObject.GetComponents(components);

            for (int i = 0; i < components.Count; i++)
            {
                Type type = components[i].GetType();
                for (int j = 0; j < unloadedComponents.Count; j++)
                {
                    if (type.ToString() == (unloadedComponents[j].GetField("@Type").JSONValue as JSONStringValue).StringValue)
                    {
                        LoadComponentDataFromJSON(components[i], unloadedComponents[j]);
                        unloadedComponents.RemoveAt(j);
                        break;
                    }
                }
            }

            if (addMissingComponents)
            {
                for (int i = 0; i < unloadedComponents.Count; i++)
                {
                    if (_callingAssembly == null)
                    {
                        _callingAssembly = Assembly.GetCallingAssembly();
                    }

                    Type type = _callingAssembly.GetType((unloadedComponents[i].GetField("@Type").JSONValue as JSONStringValue).StringValue);
                    Component newComponent = gameObject.AddComponent(type);
                    LoadComponentDataFromJSON(newComponent, unloadedComponents[i]);
                }
            }

            if (jsonObject.TryGetField("@Children", out JSONField childrenField))
            {
                JSONArrayValue childArray = childrenField.JSONValue as JSONArrayValue;
                for (int i = 0; i < childArray.Length; i++)
                {
                    if (gameObject.transform.childCount > i)
                    {
                        LoadGameObjectDataFromJSON(gameObject.transform.GetChild(i).gameObject, childArray.Elements[i] as JSONObject, addMissingComponents);
                    }
                    else
                    {
                        Debug.LogError("JSON Deserialization error: Child count mismatch. JSON has " + childArray.Length + " children, GameObject has " + gameObject.transform.childCount + " children.", gameObject);
                    }
                }
            }


            if (jsonObject.TryGetFieldValue("@Enabled", out bool enabled))
            {
                gameObject.SetActive(enabled);
            }

        }


        public void LoadComponentDataFromJSON(Component component, JSONValue jsonValue)
        {
            EnsureInSession();

            ICustomJSONSerializable customSerializer = component as ICustomJSONSerializable;
            if (customSerializer != null)
            {
                customSerializer.Deserialize(this, jsonValue);
                return;
            }

            JSONObject jsonComponentObject = jsonValue as JSONObject;
            Type type = component.GetType();

            if (jsonComponentObject.TryGetFieldValue("@ID", out int id))
            {
                _unityObjectsByID.Add(id, component);
            }

            for (int i = 1; i < jsonComponentObject.FieldCount; i++)
            {
                FieldInfo field = type.GetFieldIncludingParentTypes(jsonComponentObject.Fields[i].Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (field != null)
                {
                    object value = GetObject(field.FieldType, jsonComponentObject.Fields[i], component, field);
                    if (value != null || !field.FieldType.IsValueType)
                    {
                        field.SetValue(component, value);
                    }
                }
                else
                {
                    PropertyInfo property = type.GetProperty(jsonComponentObject.Fields[i].Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (property != null)
                    {
                        object value = GetObject(property.PropertyType, jsonComponentObject.Fields[i], component, null);
                        if (value != null)
                        {
                            property.SetValue(component, value, null);
                        }
                    }
                }
            }


            Behaviour behaviour = component as Behaviour;
            if (behaviour != null && jsonComponentObject.TryGetFieldValue("@Enabled", out bool enabled))
            {
                behaviour.enabled = enabled;
            }

        }



        object DeserializeWithCustomSerializer(Type serializerType, Type fieldType, JSONValue jsonValue, object parentObject, FieldInfo fieldInfo, JSONFieldMode mode)
        {
            CustomJSONSerializer serializer = null;
            if (!_customSerializers.TryGetValue(serializerType, out serializer))
            {
                serializer = Activator.CreateInstance(serializerType) as CustomJSONSerializer;
                _customSerializers.Add(serializerType, serializer);
            }

            if (fieldType.IsArray)
                return GetArrayObject(fieldType.GetElementType(), fieldType.GetArrayRank(), (JSONArrayValue)jsonValue, parentObject, fieldInfo, serializer, mode == JSONFieldMode.SparseArray);
            if (typeof(IList).IsAssignableFrom(fieldType))
                return GetListObject(fieldType.GetGenericArguments()[0], (JSONArrayValue)jsonValue, parentObject, fieldInfo, serializer, mode == JSONFieldMode.SparseArray);
            if (typeof(IDictionary).IsAssignableFrom(fieldType)) return GetDictionaryObject(fieldType.GetGenericArguments(), (JSONObject)jsonValue, parentObject, fieldInfo, serializer);

            return serializer.Deserialize(this, fieldType, jsonValue);
        }


        public void DeserializeStaticFields(Type type, JSONObject jsonObject)
        {

            for (int i = 0; i < jsonObject.FieldCount; i++)
            {
                FieldInfo field = type.GetFieldIncludingParentTypes(jsonObject.Fields[i].Name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                if (field != null)
                {
                    object value = GetObject(field.FieldType, jsonObject.Fields[i], null, field);
                    if (value != null)
                    {
                        field.SetValue(null, value);
                    }
                }
                else
                {
                    PropertyInfo property = type.GetProperty(jsonObject.Fields[i].Name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    if (property != null)
                    {
                        object value = GetObject(property.PropertyType, jsonObject.Fields[i], null, null);
                        if (value != null)
                        {
                            property.SetValue(null, value, null);
                        }

                    }
                }
            }

        }

        public T Deserialize<T>(string jsonString)
        {
            Assert.IsFalse(string.IsNullOrEmpty(jsonString));
            EnsureInSession();

            return (T)(GetObject(typeof(T), new JSONField(null, JSONValue.FromString(jsonString)), null, null) ?? default(T));
        }


        public T Deserialize<T>(JSONObject jsonObject)
        {
            Assert.IsNotNull(jsonObject);
            EnsureInSession();

            return (T)(GetObject(typeof(T), new JSONField(null, jsonObject), null, null) ?? default(T));
        }

        public T Deserialize<T>(JSONField field)
        {
            Assert.IsNotNull(field);
            EnsureInSession();

            return (T)(GetObject(typeof(T), field, null, null) ?? default(T));
        }

        public object Deserialize(Type type, JSONValue jsonValue)
        {
            Assert.IsNotNull(jsonValue);
            EnsureInSession();

            return GetObject(type, new JSONField("", jsonValue), null, null);
        }

        public object DeserializeFieldValue(Type type, JSONField jsonField)
        {
            return GetObject(type, jsonField, null, null);
        }

        private object GetObject(Type type, JSONField field, object parentObject, FieldInfo fieldInfo)
        {
            Assert.IsNotNull(field);
            Assert.IsNotNull(type);

            try
            {
                if (field.JSONValue == null) return null;

                if (typeof(ICustomJSONSerializable).IsAssignableFrom(type))
                {
                    object newObject = Activator.CreateInstance(type);
                    (newObject as ICustomJSONSerializable).Deserialize(this, field.JSONValue);

                    return newObject;
                }

                if (_customDeserializationMethods.TryGetValue(type, out JSONDeserializationMethod customDeserializationMethod))
                {
                    return customDeserializationMethod(this, field.JSONValue);
                }

                JSONFieldMode mode = JSONFieldMode.Default;

                if (fieldInfo != null)
                {
                    JSONFieldAttribute jsonFieldAttribute = fieldInfo.GetAttribute<JSONFieldAttribute>();
                    if (jsonFieldAttribute != null)
                    {
                        mode = jsonFieldAttribute.Mode;
                        if (jsonFieldAttribute.CustomSerializerType != null)
                        {
                            return DeserializeWithCustomSerializer(jsonFieldAttribute.CustomSerializerType, type, field.JSONValue, parentObject, fieldInfo, mode);
                        }
                    }
                }


                if (typeof(ScriptableEnum).IsAssignableFrom(type))
                {
                    return ScriptableEnum.GetValueFromGUID(((string)field.JSONValue.GetRawObject()).Substring(0, 32));
                }

                if (parentObject != null && IsResolvableUnityObjectType(type))
                {
                    _unresolvedUnityObjectReferences.Enqueue(new UnityObjectFieldReference((int)field.JSONValue.GetRawObject(), fieldInfo, parentObject));
                    return null;
                }

                Type underlyingNullableType = Nullable.GetUnderlyingType(type);
                if (underlyingNullableType != null)
                {
                    return GetObject(underlyingNullableType, field, parentObject, fieldInfo);
                }

                if (type == typeof(bool)) return (bool)field.JSONValue.GetRawObject();
                if (type == typeof(float)) return Convert.ToSingle(field.JSONValue.GetRawObject());
                if (type == typeof(int)) return (int)field.JSONValue.GetRawObject();
                if (type == typeof(byte)) return Convert.ToByte(field.JSONValue.GetRawObject());
                if (type == typeof(string)) return (string)field.JSONValue.GetRawObject();
                if (type.IsEnum) return GetEnumObject(type, field.JSONValue);

                if (type == typeof(Vector4)) return GetVector4Object((JSONStringValue)field.JSONValue);
                if (type == typeof(Vector3)) return GetVector3Object((JSONStringValue)field.JSONValue);
                if (type == typeof(Vector2)) return GetVector2Object((JSONStringValue)field.JSONValue);
                if (type == typeof(Matrix4x4)) return GetMatrix4x4Object((JSONStringValue)field.JSONValue);
                if (type == typeof(Quaternion)) return GetQuaternionObject((JSONStringValue)field.JSONValue);
                if (type == typeof(DateTime)) return DateTime.FromBinary(long.Parse(field.JSONValue.GetRawObject().ToString()));

                if (type == typeof(sbyte)) return Convert.ToSByte(field.JSONValue.GetRawObject());
                if (type == typeof(char)) return Convert.ToChar(field.JSONValue.GetRawObject());
                if (type == typeof(decimal)) return decimal.Parse(field.JSONValue.GetRawObject().ToString());
                if (type == typeof(double)) return double.Parse(field.JSONValue.GetRawObject().ToString());

                if (type == typeof(uint)) return Convert.ToUInt32(field.JSONValue.GetRawObject());
                if (type == typeof(long)) return long.Parse(field.JSONValue.GetRawObject().ToString());
                if (type == typeof(ulong)) return ulong.Parse(field.JSONValue.GetRawObject().ToString());
                if (type == typeof(short)) return Convert.ToInt16(field.JSONValue.GetRawObject());
                if (type == typeof(ushort)) return Convert.ToUInt16(field.JSONValue.GetRawObject());

                if (type.IsArray) return GetArrayObject(type.GetElementType(), type.GetArrayRank(), field.JSONValue, parentObject, fieldInfo, null, mode == JSONFieldMode.SparseArray);
                if (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))) return field.JSONValue.GetRawObject();
                if (typeof(IList).IsAssignableFrom(type)) return GetListObject(type.GetGenericArguments()[0], field.JSONValue, parentObject, fieldInfo, null, mode == JSONFieldMode.SparseArray);
                if (typeof(IDictionary).IsAssignableFrom(type)) return GetDictionaryObject(type.GetGenericArguments(), (JSONObject)field.JSONValue, parentObject, fieldInfo, null);
                if (type.IsValueType) return InstantiateObject(type, (JSONObject)field.JSONValue);

                return GetReferenceTypeObject(type, field.JSONValue);
            }
            catch
            {
                Debug.LogError("Couldn't deserialize field.\n\nField Name: " + field.Name + "\nType: " + type + "\nValue: " + field.JSONValue.GetRawObject() + "\nParent Object: " + parentObject + "\nField Info: " + fieldInfo);
                throw;
            }
        }

        private Vector3 GetVector4Object(JSONStringValue vectorString)
        {
            string[] elements = vectorString.StringValue.Split(',');
            return new Vector4(JSONFloatValue.Parse(elements[0]), JSONFloatValue.Parse(elements[1]), JSONFloatValue.Parse(elements[2]), JSONFloatValue.Parse(elements[3]));
        }

        private Vector3 GetVector3Object(JSONStringValue vectorString)
        {
            string[] elements = vectorString.StringValue.Split(',');
            return new Vector3(JSONFloatValue.Parse(elements[0]), JSONFloatValue.Parse(elements[1]), JSONFloatValue.Parse(elements[2]));
        }

        private Vector2 GetVector2Object(JSONStringValue vectorString)
        {
            string[] elements = vectorString.StringValue.Split(',');
            return new Vector2(JSONFloatValue.Parse(elements[0]), JSONFloatValue.Parse(elements[1]));
        }

        private Quaternion GetQuaternionObject(JSONStringValue quaternionString)
        {
            string[] elements = quaternionString.StringValue.Split(',');
            return new Quaternion(JSONFloatValue.Parse(elements[0]), JSONFloatValue.Parse(elements[1]), JSONFloatValue.Parse(elements[2]), JSONFloatValue.Parse(elements[3]));
        }

        private Matrix4x4 GetMatrix4x4Object(JSONStringValue mastrixString)
        {
            string[] elements = mastrixString.StringValue.Split(',');

            Vector4 column0 = new Vector4(JSONFloatValue.Parse(elements[0]), JSONFloatValue.Parse(elements[4]), JSONFloatValue.Parse(elements[8]), JSONFloatValue.Parse(elements[12]));
            Vector4 column1 = new Vector4(JSONFloatValue.Parse(elements[1]), JSONFloatValue.Parse(elements[5]), JSONFloatValue.Parse(elements[9]), JSONFloatValue.Parse(elements[13]));
            Vector4 column2 = new Vector4(JSONFloatValue.Parse(elements[2]), JSONFloatValue.Parse(elements[6]), JSONFloatValue.Parse(elements[10]), JSONFloatValue.Parse(elements[14]));
            Vector4 column3 = new Vector4(JSONFloatValue.Parse(elements[3]), JSONFloatValue.Parse(elements[7]), JSONFloatValue.Parse(elements[11]), JSONFloatValue.Parse(elements[15]));

            return new Matrix4x4(column0, column1, column2, column3);
        }


        private object GetEnumObject(Type type, JSONValue jsonValue)
        {
            JSONIntValue intValue = jsonValue as JSONIntValue;
            if (intValue != null)
            {
                return Enum.ToObject(type, intValue.GetRawObject());
            }

            string[] parts = ((string)jsonValue.GetRawObject()).Split(',');

            if (EnumUtils.TryParse(type, parts[0], out object result))
            {
                return result;
            }

            return Enum.ToObject(type, int.Parse(parts[1]));
        }

        private object GetReferenceTypeObject(Type type, JSONValue jsonValue)
        {
            object obj;
            int id;

            JSONIntValue intValue = jsonValue as JSONIntValue;
            if (intValue != null)
            {
                id = intValue.IntValue;
                if (_deserializedReferenceTypeObjects.TryGetValue(id, out obj))
                {
                    return obj;
                }

                throw new JSONSerializationException("JSON Dserialization error: Found object reference of type " + type + " with ID " + id + ". But no corresponding deserialized object exists.");
            }

            JSONObject objectValue = jsonValue as JSONObject;
            if (objectValue != null)
            {
                if (objectValue.TryGetFieldValue("@ID", out id))
                {
                    if (_deserializedReferenceTypeObjects.TryGetValue(id, out obj))
                    {
                        return obj;
                    }

                    obj = InstantiateObject(type, objectValue);
                    _deserializedReferenceTypeObjects.Add(id, obj);
                    return obj;
                }

                return InstantiateObject(type, objectValue);
            }

            throw new JSONSerializationException("Cannot deserialize to type: " + type + ". JSON value (" + jsonValue + ") is not a JSON object.");
        }



        private object InstantiateObject(Type type, JSONObject jsonObject)
        {
            if (jsonObject.TryGetFieldValue("@Type", out string typeName))
            {
                type = type.Assembly.GetType(typeName);
            }

            object newObject = type.IsValueType ? Activator.CreateInstance(type) : FormatterServices.GetUninitializedObject(type);

            for (int i = 0; i < jsonObject.FieldCount; i++)
            {
                FieldInfo field = type.GetFieldIncludingParentTypes(jsonObject.Fields[i].Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (field != null)
                {
                    object value = GetObject(field.FieldType, jsonObject.Fields[i], newObject, field);
                    if (value != null)
                    {
                        field.SetValue(newObject, value);
                    }
                }
                else
                {
                    PropertyInfo property = type.GetProperty(jsonObject.Fields[i].Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (property != null)
                    {
                        object value = GetObject(property.PropertyType, jsonObject.Fields[i], newObject, field);
                        if (value != null)
                        {
                            property.SetValue(newObject, value, null);
                        }

                    }
                }
            }

            return newObject;
        }

        private Array GetArrayObject(Type elementType, int rank, JSONValue jsonValue, object parentObject, FieldInfo fieldInfo, CustomJSONSerializer serializer, bool sparseMode)
        {

            if (rank < 1 || rank > 3)
            {
                throw new JSONSerializationException("Cannot deserialize array of rank: " + rank);
            }

            JSONObject objectValue = null;
            JSONArrayValue arrayValue = null;
            Array array = null;

            int width = 0;
            int height = 0;
            int depth = 0;

            if (rank == 1)
            {
                if (sparseMode)
                {
                    objectValue = jsonValue as JSONObject;
                    array = Array.CreateInstance(elementType, (int)objectValue.Fields[0].JSONValue.GetRawObject());
                }
                else
                {
                    arrayValue = jsonValue as JSONArrayValue;
                    array = Array.CreateInstance(elementType, arrayValue.Length);
                }
            }
            else
            {
                if (sparseMode)
                {
                    objectValue = jsonValue as JSONObject;
                }
                else
                {
                    arrayValue = objectValue.Fields[1].JSONValue as JSONArrayValue;
                }

                string[] dimensions = ((string)objectValue.Fields[0].JSONValue.GetRawObject()).Split(',');

                if (rank == 2)
                {
                    width = int.Parse(dimensions[0]);
                    height = int.Parse(dimensions[1]);
                    array = Array.CreateInstance(elementType, width, height);
                }

                if (rank == 3)
                {
                    width = int.Parse(dimensions[0]);
                    height = int.Parse(dimensions[1]);
                    depth = int.Parse(dimensions[2]);
                    array = Array.CreateInstance(elementType, width, height, depth);
                }
            }


            if (sparseMode)
            {

                JSONArrayValue indices = objectValue.Fields[1].JSONValue as JSONArrayValue;
                JSONArrayValue values = objectValue.Fields[2].JSONValue as JSONArrayValue;

                if (IsResolvableUnityObjectType(elementType) && serializer == null)
                {

                    List<int> ids = new List<int>();
                    List<int> indicesList = new List<int>();

                    for (int i = 0; i < indices.Length; i++)
                    {
                        indicesList.Add((int)indices.Elements[i].GetRawObject());
                        ids.Add((int)values.Elements[i].GetRawObject());
                    }

                    _unresolvedUnityObjectReferences.Enqueue(new UnityObjectArrayReference(array, ids, indicesList));
                }
                else
                {
                    if (rank == 1)
                    {
                        for (int i = 0; i < indices.Length; i++)
                        {
                            object elementValue = serializer == null
                                ? GetObject(elementType, new JSONField(null, values.Elements[i]), parentObject, fieldInfo)
                                : serializer.Deserialize(this, elementType, values.Elements[i]);
                            array.SetValue(Convert.ChangeType(elementValue, elementType), (int)indices.Elements[i].GetRawObject());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < indices.Length; i++)
                        {
                            object elementValue = serializer == null
                                ? GetObject(elementType, new JSONField(null, values.Elements[i]), parentObject, fieldInfo)
                                : serializer.Deserialize(this, elementType, values.Elements[i]);

                            if (rank == 2)
                            {
                                Vector2Int index = MathUtils.Get2DArrayIndex((int)indices.Elements[i].GetRawObject(), width);
                                array.SetValue(Convert.ChangeType(elementValue, elementType), index.x, index.y);
                            }

                            if (rank == 3)
                            {
                                Vector3Int index = MathUtils.Get3DArrayIndex((int)indices.Elements[i].GetRawObject(), width, height);
                                array.SetValue(Convert.ChangeType(elementValue, elementType), index.x, index.y, index.z);
                            }
                        }
                    }
                }
            }
            else
            {

                if (IsResolvableUnityObjectType(elementType) && serializer == null)
                {
                    List<int> ids = new List<int>();
                    for (int i = 0; i < arrayValue.Length; i++)
                    {
                        JSONValue value = arrayValue.Elements[i];
                        if (value != null)
                        {
                            ids.Add((int)value.GetRawObject());
                        }
                    }

                    _unresolvedUnityObjectReferences.Enqueue(new UnityObjectArrayReference(array, ids));
                }
                else
                {
                    if (rank == 1)
                    {
                        for (int i = 0; i < arrayValue.Length; i++)
                        {
                            object elementValue = serializer == null
                                ? GetObject(elementType, new JSONField(null, arrayValue.Elements[i]), parentObject, fieldInfo)
                                : serializer.Deserialize(this, elementType, arrayValue.Elements[i]);
                            array.SetValue(Convert.ChangeType(elementValue, elementType), i);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < arrayValue.Length; i++)
                        {
                            object elementValue = serializer == null
                                ? GetObject(elementType, new JSONField(null, arrayValue.Elements[i]), parentObject, fieldInfo)
                                : serializer.Deserialize(this, elementType, arrayValue.Elements[i]);

                            if (rank == 2)
                            {
                                Vector2Int indices = MathUtils.Get2DArrayIndex(i, width);
                                array.SetValue(Convert.ChangeType(elementValue, elementType), indices.x, indices.y);
                            }

                            if (rank == 3)
                            {
                                Vector3Int indices = MathUtils.Get3DArrayIndex(i, width, height);
                                array.SetValue(Convert.ChangeType(elementValue, elementType), indices.x, indices.y, indices.z);
                            }
                        }
                    }
                }
            }

            return array;
        }

        private object GetListObject(Type elementType, JSONValue jsonValue, object parentObject, FieldInfo fieldInfo, CustomJSONSerializer serializer, bool sparseMode)
        {
            IList list = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType)) as IList;
            JSONArrayValue arrayValue = jsonValue as JSONArrayValue;

            if (IsResolvableUnityObjectType(elementType) && serializer == null)
            {

                List<int> ids = new List<int>();
                for (int i = 0; i < arrayValue.Length; i++)
                {
                    JSONValue value = arrayValue.Elements[i];
                    if (value != null)
                    {
                        ids.Add((int)value.GetRawObject());
                    }
                }

                _unresolvedUnityObjectReferences.Enqueue(new UnityObjectListReference(list, ids));
            }
            else
            {
                for (int i = 0; i < arrayValue.Length; i++)
                {
                    if (serializer == null)
                    {
                        list.Add(GetObject(elementType, new JSONField(null, arrayValue.Elements[i]), parentObject, fieldInfo));
                    }
                    else
                    {
                        list.Add(serializer.Deserialize(this, elementType, arrayValue.Elements[i]));
                    }
                }
            }

            return list;
        }

        private object GetDictionaryObject(Type[] typeArguments, JSONObject dictionaryObject, object parentObject, FieldInfo fieldInfo, CustomJSONSerializer serializer)
        {

            object DeserializeValue(JSONField field)
            {
                if (serializer != null)
                {
                    return serializer.Deserialize(this, typeArguments[1], field.JSONValue);
                }

                return GetObject(typeArguments[1], field, parentObject, fieldInfo);
            }

            List<object> GetKeys(bool isUnityObject)
            {
                List<object> keys = new List<object>();

                if (typeArguments[0] == typeof(string))
                {
                    for (int i = 0; i < dictionaryObject.FieldCount; i++)
                    {
                        keys.Add(dictionaryObject.Fields[i].Name);
                    }

                }
                else
                {
                    JSONArrayValue keysObject = dictionaryObject.Fields[0].JSONValue as JSONArrayValue;

                    for (int i = 0; i < keysObject.Length; i++)
                    {
                        if (isUnityObject)
                        {
                            keys.Add(keysObject.Elements[i].GetRawObject());
                        }
                        else
                        {

                            keys.Add(GetObject(typeArguments[0], new JSONField("Key " + i, keysObject.Elements[i]), parentObject, fieldInfo));
                        }
                    }
                }

                return keys;
            }

            List<object> GetValues(bool isUnityObject)
            {
                List<object> values = new List<object>();

                if (typeArguments[0] == typeof(string))
                {
                    for (int i = 0; i < dictionaryObject.FieldCount; i++)
                    {
                        if (isUnityObject)
                        {
                            values.Add(dictionaryObject.Fields[i].JSONValue.GetRawObject());
                        }
                        else
                        {
                            values.Add(DeserializeValue(dictionaryObject.Fields[i]));
                        }
                    }

                }
                else
                {
                    JSONArrayValue valuesObject = dictionaryObject.Fields[1].JSONValue as JSONArrayValue;

                    for (int i = 0; i < valuesObject.Length; i++)
                    {
                        if (isUnityObject)
                        {
                            values.Add(valuesObject.Elements[i].GetRawObject());
                        }
                        else
                        {
                            values.Add(DeserializeValue(new JSONField("Value " + i, valuesObject.Elements[i])));
                        }
                    }
                }

                return values;
            }


            IDictionary dictionary = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(typeArguments)) as IDictionary;


            if (IsResolvableUnityObjectType(typeArguments[0]) && IsResolvableUnityObjectType(typeArguments[1]) && serializer == null)
            {
                _unresolvedUnityObjectReferences.Enqueue(new UnityObjectDictionaryReference(dictionary, GetKeys(true), GetValues(true), true, true));
            }
            else if (IsResolvableUnityObjectType(typeArguments[0]) && serializer == null)
            {
                _unresolvedUnityObjectReferences.Enqueue(new UnityObjectDictionaryReference(dictionary, GetKeys(true), GetValues(false), true, false));
            }
            else if (IsResolvableUnityObjectType(typeArguments[1]) && serializer == null)
            {
                _unresolvedUnityObjectReferences.Enqueue(new UnityObjectDictionaryReference(dictionary, GetKeys(false), GetValues(true), false, true));
            }
            else
            {
                List<object> keys = GetKeys(false);
                List<object> values = GetValues(false);

                for (int i = 0; i < keys.Count; i++)
                {
                    dictionary.Add(keys[i], values[i]);
                }

            }


            return dictionary;
        }

        bool IsResolvableUnityObjectType(Type type)
        {
            if (!typeof(Object).IsAssignableFrom(type)) return false;
            if (typeof(ScriptableEnum).IsAssignableFrom(type)) return false;

            return true;
        }

        private abstract class UnityObjectReference
        {
            public abstract void Resolve(Dictionary<int, Object> unityObjects);
        }

        private class UnityObjectFieldReference : UnityObjectReference
        {
            private int _id;
            private FieldInfo _field;
            private object _object;

            public UnityObjectFieldReference(int id, FieldInfo field, object obj)
            {
                Assert.IsNotNull(field);
                Assert.IsNotNull(obj);

                _id = id;
                _field = field;
                _object = obj;
            }

            public override void Resolve(Dictionary<int, Object> unityObjects)
            {
                try
                {
                    _field.SetValue(_object, unityObjects[_id]);
                }
                catch
                {
                    Debug.LogError("Couldn't resolve Unity Object field reference\n\nField name: " + _field.Name + "\n\nObject: " + _object + " \n\nID: " + _id);
                    throw;
                }
            }
        }

        private class UnityObjectArrayReference : UnityObjectReference
        {
            private Array _array;
            private List<int> _ids;
            private List<int> _indices;

            public UnityObjectArrayReference(Array array, List<int> ids)
            {
                Assert.IsNotNull(array);
                Assert.IsNotNull(ids);

                _ids = ids;
                _array = array;
            }

            public UnityObjectArrayReference(Array array, List<int> ids, List<int> indices)
            {
                Assert.IsNotNull(array);
                Assert.IsNotNull(ids);
                Assert.IsNotNull(indices);
                Assert.AreEqual(indices.Count, ids.Count);

                _ids = ids;
                _indices = indices;
                _array = array;
            }

            public override void Resolve(Dictionary<int, Object> unityObjects)
            {
                int width = _array.Rank > 1 ? _array.GetLength(0) : 0;
                int height = _array.Rank > 1 ? _array.GetLength(1) : 0;


                for (int i = 0; i < _ids.Count; i++)
                {
                    int index = _indices == null ? i : _indices[i];

                    try
                    {
                        if (_array.Rank == 1)
                        {
                            _array.SetValue(unityObjects[_ids[i]], index);
                        }
                        else if (_array.Rank == 2)
                        {
                            Vector2Int indices = MathUtils.Get2DArrayIndex(index, width);
                            _array.SetValue(unityObjects[_ids[i]], indices.x, indices.y);
                        }
                        else if (_array.Rank == 3)
                        {
                            Vector3Int indices = MathUtils.Get3DArrayIndex(index, width, height);
                            _array.SetValue(unityObjects[_ids[i]], indices.x, indices.y, indices.z);
                        }
                    }
                    catch
                    {
                        Debug.LogError("Couldn't resolve Unity Object array reference\n\nIndex: " + index + "\n\nArray: " + _array + " \n\nID: " + _ids[i]);
                        throw;
                    }
                }
            }
        }

        private class UnityObjectListReference : UnityObjectReference
        {
            private IList _list;
            private List<int> _ids;

            public UnityObjectListReference(IList list, List<int> ids)
            {
                Assert.IsNotNull(list);
                Assert.IsNotNull(ids);

                _ids = ids;
                _list = list;
            }

            public override void Resolve(Dictionary<int, Object> unityObjects)
            {
                for (int i = 0; i < _ids.Count; i++)
                {

                    try
                    {
                        _list.Add(unityObjects[_ids[i]]);
                    }
                    catch
                    {
                        Debug.LogError("Couldn't resolve Unity Object list reference\n\nIndex: " + i + "\n\nList: " + _list + " \n\nID: " + _ids[i]);
                        throw;
                    }
                }
            }
        }

        private class UnityObjectDictionaryReference : UnityObjectReference
        {

            private IDictionary _dictionary;
            private List<object> _keys;
            private List<object> _values;
            private bool _keysAreUnityObjects;
            private bool _valuesAreUnityObjects;

            public UnityObjectDictionaryReference(IDictionary dictionary, List<object> keys, List<object> values, bool keysAreUnityObjects, bool valuesAreUnityObjects)
            {
                Assert.IsNotNull(values);
                Assert.IsNotNull(dictionary);
                Assert.IsNotNull(keys);
                Assert.IsFalse(!keysAreUnityObjects && !valuesAreUnityObjects);

                _values = values;
                _keys = keys;
                _dictionary = dictionary;
                _keysAreUnityObjects = keysAreUnityObjects;
                _valuesAreUnityObjects = valuesAreUnityObjects;
            }

            public override void Resolve(Dictionary<int, Object> unityObjects)
            {
                for (int i = 0; i < _keys.Count; i++)
                {
                    try
                    {
                        if (_keysAreUnityObjects && _valuesAreUnityObjects)
                        {
                            _dictionary.Add(unityObjects[(int)_keys[i]], unityObjects[(int)_values[i]]);
                        }
                        else if (_keysAreUnityObjects)
                        {
                            _dictionary.Add(unityObjects[(int)_keys[i]], _values[i]);
                        }
                        else if (_valuesAreUnityObjects)
                        {
                            _dictionary.Add(_keys[i], unityObjects[(int)_values[i]]);
                        }
                    }
                    catch
                    {
                        Debug.LogError("Couldn't resolve Unity Object dictionary reference\n\nIndex: " + i + "\n\nDictionary: " + _dictionary + " \n\nKey: " + _keys[i] + " \n\nValue: " + _values[i]);
                        throw;
                    }
                }
            }
        }
    }
}
