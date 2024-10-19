using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Framework;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Framework
{

    public enum JSONPolicy
    {
        AllFields,
        MarkedFieldsOnly,
        PublicAndSerializedFieldsOnly,
        NoFields,
        NonSerialized
    }

    public enum JSONFieldMode
    {
        Default,
        SparseArray
    }

    public class JSONSerializationException : Exception
    {
        public JSONSerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public JSONSerializationException(string message) : base(message)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class JSONFieldAttribute : Attribute
    {
        public Type CustomSerializerType;
        public JSONFieldMode Mode;

        public JSONFieldAttribute(Type customSerializerType)
        {
            Mode = JSONFieldMode.Default;
            CustomSerializerType = customSerializerType;
        }

        public JSONFieldAttribute(JSONFieldMode mode = JSONFieldMode.Default)
        {
            Mode = mode;
            CustomSerializerType = null;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class NonJSONFieldAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class JSONPolicyAttribute : Attribute
    {
        public JSONPolicy Policy;

        public JSONPolicyAttribute(JSONPolicy policy)
        {
            Policy = policy;
        }
    }

    public interface ICustomJSONSerializable
    {
        JSONValue Serialize(JSONSerializer serializer);
        void Deserialize(JSONDeserializer deserializer, JSONValue jsonValue);
    }

    public abstract class CustomJSONSerializer
    {
        public abstract JSONValue Serialize(JSONSerializer serializer, Type type, object obj);
        public abstract object Deserialize(JSONDeserializer deserializer, Type type, JSONValue jsonValue);
    }

    public delegate JSONValue JSONSerializationMethod(JSONSerializer serializer, object obj);
    public delegate object JSONDeserializationMethod(JSONDeserializer deserializer, JSONValue json);


    public class JSONSerializer
    {
        public enum LogLevel
        {
            None,
            GameObjects,
            Components,
            AllValues
        }

        public class Config
        {
            public bool SerializeObjectReferences = true;
            public bool SerializeGameObjectActiveState = true;
            public bool SerializeBehaviourEnabledState = true;
            public bool SerializePosition = true;
            public bool SerializeRotation = true;
            public bool SerializeScale = true;
            public bool SerializeGameObjectNames = true;
            public bool SerializeTags = false;
            public bool SerializeLayers = false;
            public JSONPolicy DefaultComponentPolicy = JSONPolicy.AllFields;
            public JSONPolicy DefaultObjectPolicy = JSONPolicy.AllFields;
        }


        private static Dictionary<Type, JSONSerializationMethod> _customSerializationMethods = new Dictionary<Type, JSONSerializationMethod>();
        private static Dictionary<Type, CustomJSONSerializer> _customSerializers = new Dictionary<Type, CustomJSONSerializer>();

        private Dictionary<object, int> _referenceObjectIDs = new Dictionary<object, int>();
        private int _nextReferenceObjectID;
        private Config _config;
        private bool _serializationSessionStarted;
        private LogLevel _logLevel;

        public void BeginSerialization()
        {
            BeginSerialization(new Config());
        }

        public void BeginSerialization(Config config, LogLevel logLevel = LogLevel.None)
        {
            Assert.IsNotNull(config);

            if (_serializationSessionStarted)
            {
                throw new JSONSerializationException("JSON serialization session already started. Are you sure EndSerialization() was properly called after the last session?");
            }

            _config = config;
            _referenceObjectIDs.Clear();
            _nextReferenceObjectID = 0;
            _serializationSessionStarted = true;
            _logLevel = logLevel;
        }

        public void EndSerialization()
        {
            if (!_serializationSessionStarted)
            {
                throw new JSONSerializationException("JSON serialization session was never started. Are you sure EndSerialization() hasn't already been called and that a session was properly started?");
            }

            _serializationSessionStarted = false;
        }

        private void EnsureInSession()
        {
            if (!_serializationSessionStarted)
            {
                throw new JSONSerializationException(
                    "JSON serialization session not started! Make sure you call BeginSerialization() before you start serializing and EndSerialization() when you are finished.");
            }
        }

        public void SetConfig(Config config)
        {
            Assert.IsNotNull(config);

            _config = config;
        }

        public static void AddCustomSerializationMethod(Type type, JSONSerializationMethod method)
        {
            _customSerializationMethods[type] = method;
        }

        /// <summary>
        /// Converts a GameObject into a JSON object. Note that this will only look at components on the GameObject itself, not any of its child objects.
        /// </summary>
        /// <param name="gameObject">The GameObject to convert</param>
        /// <returns>A JSON object representing the GameObject</returns>
        public JSONObject SerializeGameObject(GameObject gameObject, bool includeChildren)
        {
            EnsureInSession();

            if (gameObject == null) return null;

            if (_logLevel >= LogLevel.GameObjects) Log("GameObject: " + gameObject.name);

            List<JSONField> jsonFields = new List<JSONField>();

            if (_config.SerializeGameObjectNames) jsonFields.Add(new JSONField("@Name", gameObject.name));
            if (_config.SerializeObjectReferences) jsonFields.Add(new JSONField("@ID", gameObject.GetInstanceID()));
            if (_config.SerializeTags) jsonFields.Add(new JSONField("@Tag", gameObject.tag));
            if (_config.SerializeLayers) jsonFields.Add(new JSONField("@Layer", gameObject.layer));
            if (_config.SerializeGameObjectActiveState) jsonFields.Add(new JSONField("@Enabled", gameObject.activeSelf));

            List<JSONValue> jsonComponents = new List<JSONValue>();
            List<Component> components = new List<Component>();
            gameObject.GetComponents(components);

            //   int maxComponents = 2;

            for (int i = 0; i < components.Count; i++)
            {
                //    if (i == maxComponents) break;

                // if (components[i].name == "House") continue;

                //  Debug.Log(i + ": " + components[i]);


                JSONValue componentJsonObject = SerializeComponent(components[i]);
                if (componentJsonObject != null)
                {
                    jsonComponents.Add(componentJsonObject);
                }

            }

            jsonFields.Add(new JSONField("@Components", jsonComponents.ToArray()));


            if (includeChildren && gameObject.transform.childCount > 0)
            {
                List<JSONObject> jsonChildren = new List<JSONObject>();
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    jsonChildren.Add(SerializeGameObject(gameObject.transform.GetChild(i).gameObject, includeChildren));
                }

                jsonFields.Add(new JSONField("@Children", jsonChildren.ToArray()));
            }


            return new JSONObject(jsonFields.ToArray());
        }


        public JSONValue SerializeComponent(Component component)
        {
            EnsureInSession();

            if (component == null) return null;

            if (_logLevel >= LogLevel.Components) Log("Component: " + component.GetType());

            ICustomJSONSerializable customSerializer = component as ICustomJSONSerializable;
            if (customSerializer != null)
            {
                return customSerializer.Serialize(this);
            }

            Type type = component.GetType();

            List<JSONField> jsonFields = new List<JSONField>();

            if (type == typeof(Transform))
            {
                if (_config.SerializePosition || _config.SerializeRotation || _config.SerializeScale)
                {
                    Transform transform = component as Transform;

                    jsonFields.Add(new JSONField("@Type", type.ToString()));

                    if (_config.SerializeObjectReferences)
                    {
                        jsonFields.Add(new JSONField("@ID", component.GetInstanceID()));
                    }

                    if (_config.SerializePosition) jsonFields.Add(new JSONField("position", GetVector3JSON(transform.position)));
                    if (_config.SerializeRotation) jsonFields.Add(new JSONField("rotation", GetQuaternionJSON(transform.rotation)));
                    if (_config.SerializeScale) jsonFields.Add(new JSONField("scale", GetVector3JSON(transform.localScale)));
                }
            }
            else if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                JSONPolicy policy = _config.DefaultComponentPolicy;

                JSONPolicyAttribute[] policyAttibutes = type.GetCustomAttributes(typeof(JSONPolicyAttribute), false) as JSONPolicyAttribute[];
                if (policyAttibutes != null && policyAttibutes.Length > 0)
                {
                    policy = policyAttibutes[0].Policy;
                }

                if (policy == JSONPolicy.NonSerialized)
                {
                    return null;
                }

                jsonFields.Add(new JSONField("@Type", type.ToString()));

                if (_config.SerializeObjectReferences)
                {
                    jsonFields.Add(new JSONField("@ID", component.GetInstanceID()));
                }

                if (_config.SerializeBehaviourEnabledState)
                {
                    Behaviour behaviour = component as Behaviour;
                    if (behaviour != null)
                    {
                        jsonFields.Add(new JSONField("@Enabled", behaviour.enabled));
                    }
                }

                if (policy != JSONPolicy.NoFields)
                {
                    jsonFields.AddRange(GetJSONFieldsViaReflection(type, component, policy));
                }
            }
            else
            {
                return null;
            }

            for (int i = 0; i < jsonFields.Count; i++)
            {
                if (jsonFields[i] == null)
                {
                    jsonFields.RemoveAt(i);
                    i--;
                }
            }

            if (jsonFields.Count <= 1)
            {
                return null;
            }

            return new JSONObject(jsonFields.ToArray());
        }


        public JSONField Serialize(string name, object obj)
        {
            if (_logLevel >= LogLevel.AllValues) Log("Field: " + name);

            if (obj == null || obj.Equals(null)) return new JSONField(name);

            return new JSONField(name, GetJSONValue(obj.GetType(), obj, JSONFieldMode.Default));

        }

        public JSONValue Serialize(object obj)
        {
            if (obj == null || obj.Equals(null)) return null;

            return GetJSONValue(obj.GetType(), obj, JSONFieldMode.Default);
        }

        private JSONValue GetJSONValue(Type type, object obj, JSONFieldMode mode)
        {
            EnsureInSession();

            try
            {
                if (_logLevel >= LogLevel.Components) Log("Value: " + obj);

                if (type == typeof(bool)) return new JSONBooleanValue((bool)obj);
                if (type == typeof(float)) return new JSONFloatValue((float)obj);
                if (type == typeof(int)) return new JSONIntValue((int)obj);
                if (type == typeof(byte)) return new JSONIntValue((int)obj);
                if (type == typeof(string)) return new JSONStringValue((string)obj);
                if (type.IsEnum) return new JSONStringValue(obj + "," + (int)obj);

                if (type == typeof(Vector4)) return GetVector4JSON((Vector4)obj);
                if (type == typeof(Vector3)) return GetVector3JSON((Vector3)obj);
                if (type == typeof(Vector2)) return GetVector2JSON((Vector2)obj);
                if (type == typeof(Matrix4x4)) return GetMatrix4x4JSON((Matrix4x4)obj);
                if (type == typeof(Quaternion)) return GetQuaternionJSON((Quaternion)obj);
                if (type == typeof(DateTime)) return new JSONStringValue(((DateTime)obj).ToBinary().ToString());

                if (type == typeof(sbyte)) return new JSONIntValue((int)obj);
                if (type == typeof(char)) return new JSONIntValue((int)obj);
                if (type == typeof(decimal)) return new JSONStringValue(obj.ToString());
                if (type == typeof(double)) return new JSONStringValue(obj.ToString());

                if (type == typeof(uint)) return new JSONIntValue((int)obj);
                if (type == typeof(long)) return new JSONStringValue(obj.ToString());
                if (type == typeof(ulong)) return new JSONStringValue(obj.ToString());
                if (type == typeof(short)) return new JSONIntValue((int)obj);
                if (type == typeof(ushort)) return new JSONIntValue((int)obj);

                if (obj is Array array) return GetArrayJSON(array, type.GetElementType(), null, mode == JSONFieldMode.SparseArray);
                if (typeof(ScriptableEnum).IsAssignableFrom(type)) return GetScriptableEnumJSON(obj as ScriptableEnum);
                if (typeof(IList).IsAssignableFrom(type)) return GetListJSON((IList)obj, type.GetGenericArguments()[0], null, mode == JSONFieldMode.SparseArray);
                if (typeof(IDictionary).IsAssignableFrom(type)) return GetDictionaryJSON((IDictionary)obj, null);

                if (_customSerializationMethods.TryGetValue(type, out JSONSerializationMethod customSerializationMethod))
                {
                    return customSerializationMethod(this, obj);
                }

                if (typeof(Object).IsAssignableFrom(type))
                {
                    if (_config.SerializeObjectReferences)
                    {
                        Object unityObject = (Object)obj;
                        if (unityObject != null)
                        {
                            return new JSONIntValue(unityObject.GetInstanceID());
                        }
                    }

                    return null;
                }

                JSONPolicy policy = _config.DefaultObjectPolicy;

                JSONPolicyAttribute[] policyAttibutes = type.GetCustomAttributes(typeof(JSONPolicyAttribute), false) as JSONPolicyAttribute[];
                if (policyAttibutes != null && policyAttibutes.Length > 0)
                {
                    policy = policyAttibutes[0].Policy;
                }

                if (policy == JSONPolicy.NonSerialized)
                {
                    return null;
                }

                return GetObjectJSON(obj, type, policy);

            }
            catch
            {
                Debug.LogError("Couldn't serialize value.\n\nType: " + type + "\nValue: " + obj);
                throw;
            }

        }

        public JSONValue SerializeStaticFields(Type type)
        {
            JSONPolicy policy = _config.DefaultObjectPolicy;

            JSONPolicyAttribute[] policyAttibutes = type.GetCustomAttributes(typeof(JSONPolicyAttribute), false) as JSONPolicyAttribute[];
            if (policyAttibutes != null && policyAttibutes.Length > 0)
            {
                policy = policyAttibutes[0].Policy;
            }

            if (policy == JSONPolicy.NonSerialized)
            {
                return null;
            }

            return new JSONObject(GetJSONFieldsViaReflection(type, null, policy));
        }

        private JSONValue GetObjectJSON(object obj, Type type, JSONPolicy policy)
        {

            ICustomJSONSerializable customSerializer = obj as ICustomJSONSerializable;
            if (customSerializer != null)
            {
                return customSerializer.Serialize(this);
            }

            if (type.IsValueType)
            {
                return new JSONObject(GetJSONFieldsViaReflection(type, obj, policy));
            }

            int id;
            if (_referenceObjectIDs.TryGetValue(obj, out id))
            {
                return new JSONIntValue(id);
            }

            List<JSONField> fields = new List<JSONField>();

            id = _nextReferenceObjectID;
            _nextReferenceObjectID++;

            if (_config.SerializeObjectReferences)
            {
                fields.Add(new JSONField("@ID", id));
            }


            if (policy != JSONPolicy.NoFields)
            {
                fields.AddRange(GetJSONFieldsViaReflection(type, obj, policy));
            }

            _referenceObjectIDs.Add(obj, id);

            return new JSONObject(fields.ToArray());

        }

        private JSONValue GetScriptableEnumJSON(ScriptableEnum value)
        {
            return new JSONStringValue(value.GUID + "," + value.name);
        }

        private JSONValue GetVector4JSON(Vector4 vector)
        {
            return new JSONStringValue(JSONFloatValue.Format(vector.x) + "," + JSONFloatValue.Format(vector.y) + "," + JSONFloatValue.Format(vector.z) + "," + JSONFloatValue.Format(vector.w));
        }

        private JSONValue GetVector3JSON(Vector3 vector)
        {
            return new JSONStringValue(JSONFloatValue.Format(vector.x) + "," + JSONFloatValue.Format(vector.y) + "," + JSONFloatValue.Format(vector.z));
        }

        private JSONValue GetVector2JSON(Vector2 vector)
        {
            return new JSONStringValue(JSONFloatValue.Format(vector.x) + "," + JSONFloatValue.Format(vector.y));
        }

        private JSONValue GetQuaternionJSON(Quaternion quaternion)
        {
            return new JSONStringValue(JSONFloatValue.Format(quaternion.x) + "," + JSONFloatValue.Format(quaternion.y) + "," + JSONFloatValue.Format(quaternion.z) + "," +
                                       JSONFloatValue.Format(quaternion.w));
        }


        private JSONValue GetMatrix4x4JSON(Matrix4x4 matrix)
        {
            string value = JSONFloatValue.Format(matrix.m00);
            value += "," + JSONFloatValue.Format(matrix.m01);
            value += "," + JSONFloatValue.Format(matrix.m02);
            value += "," + JSONFloatValue.Format(matrix.m03);
            value += "," + JSONFloatValue.Format(matrix.m10);
            value += "," + JSONFloatValue.Format(matrix.m11);
            value += "," + JSONFloatValue.Format(matrix.m12);
            value += "," + JSONFloatValue.Format(matrix.m13);
            value += "," + JSONFloatValue.Format(matrix.m20);
            value += "," + JSONFloatValue.Format(matrix.m21);
            value += "," + JSONFloatValue.Format(matrix.m22);
            value += "," + JSONFloatValue.Format(matrix.m23);
            value += "," + JSONFloatValue.Format(matrix.m30);
            value += "," + JSONFloatValue.Format(matrix.m31);
            value += "," + JSONFloatValue.Format(matrix.m32);
            value += "," + JSONFloatValue.Format(matrix.m33);

            return new JSONStringValue(value);
        }

        private JSONValue GetDictionaryJSON(IDictionary dictionary, CustomJSONSerializer customSerializer)
        {

            Type keyType = dictionary.Keys.GetType().GetGenericArguments()[0];
            Type valueType = dictionary.Values.GetType().GetGenericArguments()[1];

            if (keyType == typeof(string))
            {
                JSONField[] elements = new JSONField[dictionary.Count];
                object[] keys = dictionary.Keys.GetCollectionElements();
                object[] values = dictionary.Values.GetCollectionElements();

                for (int i = 0; i < elements.Length; i++)
                {
                    if (customSerializer != null)
                    {
                        elements[i] = new JSONField(keys[i].ToString(), customSerializer.Serialize(this, valueType, values[i]));
                    }
                    else
                    {
                        if (values[i] == null || values[i].Equals(null))
                        {
                            elements[i] = new JSONField(keys[i].ToString());
                        }
                        else
                        {
                            elements[i] = new JSONField(keys[i].ToString(), Serialize(values[i]));
                        }
                    }
                }

                return new JSONObject(elements);
            }
            else
            {
                JSONField[] elements = new JSONField[2];

                elements[0] = new JSONField("@Keys", GetArrayJSON(dictionary.Keys.GetCollectionElements(), keyType, null, false));
                elements[1] = new JSONField("@Values", GetArrayJSON(dictionary.Values.GetCollectionElements(), valueType, customSerializer, false));

                return new JSONObject(elements);
            }
        }

        private JSONValue GetArrayJSON(Array array, Type elementType, CustomJSONSerializer customSerializer, bool sparseMode)
        {
            if (array.Rank < 1 || array.Rank > 3)
            {
                throw new JSONSerializationException("Cannot serialize array of rank: " + array.Rank);
            }

            JSONValue SerializeElement(object element)
            {
                if (customSerializer != null)
                {
                    return customSerializer.Serialize(this, elementType, element);
                }

                if (element == null || element.Equals(null))
                {
                    return null;
                }

                return Serialize(element);
            }

            if (sparseMode)
            {
                List<JSONIntValue> indices = new List<JSONIntValue>();
                List<JSONValue> values = new List<JSONValue>();
                JSONValue dimensions = null;

                if (array.Rank == 1)
                {
                    dimensions = new JSONIntValue(array.Length);

                    for (int i = 0; i < array.Length; i++)
                    {
                        JSONValue value = SerializeElement(array.GetValue(i));
                        if (value != null && !value.Equals(null))
                        {
                            indices.Add(new JSONIntValue(i));
                            values.Add(value);
                        }
                    }
                }

                if (array.Rank == 2)
                {
                    int width = array.GetLength(0);
                    int height = array.GetLength(1);

                    dimensions = new JSONStringValue(width + "," + height);

                    for (int i = 0; i < array.Length; i++)
                    {
                        Vector2Int index = MathUtils.Get2DArrayIndex(i, width);
                        JSONValue value = SerializeElement(array.GetValue(index.x, index.y));
                        if (value != null && !value.Equals(null))
                        {
                            indices.Add(new JSONIntValue(i));
                            values.Add(value);
                        }
                    }
                }

                if (array.Rank == 3)
                {
                    int width = array.GetLength(0);
                    int height = array.GetLength(1);
                    int depth = array.GetLength(1);

                    dimensions = new JSONStringValue(width + "," + height + "," + depth);

                    for (int i = 0; i < array.Length; i++)
                    {
                        Vector3Int index = MathUtils.Get3DArrayIndex(i, width, height);
                        JSONValue value = SerializeElement(array.GetValue(index.x, index.y, index.z));
                        if (value != null && !value.Equals(null))
                        {
                            indices.Add(new JSONIntValue(i));
                            values.Add(value);
                        }
                    }
                }

                JSONField[] elements = new JSONField[3];

                elements[0] = new JSONField(array.Rank == 1 ? "@Length" : "@Dimensions", dimensions);
                elements[1] = new JSONField("@Indices", new JSONArrayValue(indices.ToArray()));
                elements[2] = new JSONField("@Values", new JSONArrayValue(values.ToArray()));

                return new JSONObject(elements);
            }

            if (array.Rank == 1)
            {
                JSONValue[] elements = new JSONValue[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    elements[i] = SerializeElement(array.GetValue(i));
                }

                return new JSONArrayValue(elements);
            }

            if (array.Rank == 2)
            {
                int width = array.GetLength(0);
                int height = array.GetLength(1);

                JSONValue[] elements = new JSONValue[width * height];

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        elements[MathUtils.Get1DArrayIndex(x, y, width)] = SerializeElement(array.GetValue(x, y));
                    }
                }

                return new JSONObject(new[] { new JSONField("@Dimensions", new JSONStringValue(width + "," + height)), new JSONField("@Values", new JSONArrayValue(elements)) });
            }

            if (array.Rank == 3)
            {
                int width = array.GetLength(0);
                int height = array.GetLength(1);
                int depth = array.GetLength(1);

                JSONValue[] elements = new JSONValue[width * height * depth];

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int z = 0; z < depth; z++)
                        {
                            elements[MathUtils.Get1DArrayIndex(x, y, z, width, height)] = SerializeElement(array.GetValue(x, y, z));
                        }
                    }
                }

                return new JSONObject(new[] { new JSONField("@Dimensions", new JSONStringValue(width + "," + height + "," + depth)), new JSONField("@Values", new JSONArrayValue(elements)) });
            }

            return null;
        }


        private JSONValue GetListJSON(IList list, Type elementType, CustomJSONSerializer customSerializer, bool sparseMode)
        {
            JSONValue SerializeElement(object element)
            {
                if (customSerializer != null)
                {
                    return customSerializer.Serialize(this, elementType, element);
                }

                if (element == null || element.Equals(null))
                {
                    return null;
                }

                return Serialize(element);
            }

            if (sparseMode)
            {
                List<JSONValue> keys = new List<JSONValue>();
                List<JSONValue> values = new List<JSONValue>();
                JSONValue count = new JSONIntValue(list.Count);

                for (int i = 0; i < list.Count; i++)
                {
                    JSONValue value = SerializeElement(list[i]);
                    if (value != null && !value.Equals(null))
                    {
                        keys.Add(new JSONIntValue(i));
                        values.Add(value);
                    }
                }

                JSONField[] elements = new JSONField[3];

                elements[0] = new JSONField("@Count", count);
                elements[1] = new JSONField("@Keys", new JSONArrayValue(keys.ToArray()));
                elements[2] = new JSONField("@Values", new JSONArrayValue(values.ToArray()));

                return new JSONObject(elements);

            }
            else
            {

                JSONValue[] elements = new JSONValue[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    elements[i] = SerializeElement(list[i]);
                }

                return new JSONArrayValue(elements);
            }
        }


        private JSONField[] GetJSONFieldsViaReflection(Type type, object obj, JSONPolicy policy)
        {
            List<JSONField> savableFields = new List<JSONField>();

            if (_logLevel >= LogLevel.AllValues) Log("Get Fields via Reflection: " + obj);

            if (obj != null)
            {
                Type objectType = obj.GetType();
                if (type != objectType)
                {
                    savableFields.Add(new JSONField("@Type", objectType.FullName));
                    type = objectType;
                }
            }

            FieldInfo[] fields = type.GetFieldsIncludingParentTypes((obj == null ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.NonPublic | BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {



                if (ShouldSerializeField(policy, fields[i]))
                {
                    if (_logLevel >= LogLevel.AllValues) Log("Field: " + fields[i].Name + "(" + fields[i].DeclaringType + ")");
                    try
                    {
                        JSONFieldAttribute jsonFieldAttribute = fields[i].GetAttribute<JSONFieldAttribute>();
                        JSONFieldMode mode = jsonFieldAttribute == null ? JSONFieldMode.Default : jsonFieldAttribute.Mode;

                        if (jsonFieldAttribute != null && jsonFieldAttribute.CustomSerializerType != null)
                        {
                            JSONValue value = SerializeWithCustomSerializer(jsonFieldAttribute.CustomSerializerType, fields[i].FieldType, fields[i].GetValue(obj), mode);
                            savableFields.Add(new JSONField(fields[i].Name, value));
                        }
                        else
                        {
                            object value = fields[i].GetValue(obj);
                            if (value == null || value.Equals(null))
                            {
                                savableFields.Add(new JSONField(fields[i].Name));
                            }
                            else
                            {
                                savableFields.Add(new JSONField(fields[i].Name, GetJSONValue(fields[i].FieldType, value, mode)));
                            }


                        }

                    }
                    catch
                    {
                        Debug.LogError(
                            "Couldn't serialize field.\n\nField Name: " + fields[i].Name + "\nType: " + fields[i].FieldType + "\nValue: " + fields[i].GetValue(obj) + "\nParent Object: " + obj +
                            "\nParent Type: " + type + "\n\n");
                        throw;
                    }
                }
            }

            return savableFields.ToArray();
        }

        JSONValue SerializeWithCustomSerializer(Type serializerType, Type fieldType, object obj, JSONFieldMode mode)
        {

            CustomJSONSerializer serializer = null;
            if (!_customSerializers.TryGetValue(serializerType, out serializer))
            {
                serializer = Activator.CreateInstance(serializerType) as CustomJSONSerializer;
                _customSerializers.Add(serializerType, serializer);
            }

            if (obj is Array array) return GetArrayJSON(array, fieldType.GetElementType(), serializer, mode == JSONFieldMode.SparseArray);
            if (typeof(IList).IsAssignableFrom(fieldType)) return GetListJSON((IList)obj, fieldType.GetGenericArguments()[0], serializer, mode == JSONFieldMode.SparseArray);
            if (typeof(IDictionary).IsAssignableFrom(fieldType)) return GetDictionaryJSON((IDictionary)obj, serializer);

            return serializer.Serialize(this, fieldType, obj);
        }

        bool ShouldSerializeField(JSONPolicy policy, FieldInfo field)
        {
            if (field.IsLiteral) return false;
            if (policy == JSONPolicy.AllFields && !Attribute.IsDefined(field, typeof(NonJSONFieldAttribute))) return true;
            if (policy == JSONPolicy.MarkedFieldsOnly && Attribute.IsDefined(field, typeof(JSONFieldAttribute))) return true;
            if (policy == JSONPolicy.PublicAndSerializedFieldsOnly && (field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField)))) return true;

            return false;
        }

        [HideInCallstack]
        void Log(string message)
        {
            Debug.Log(message);
        }

    }
}
