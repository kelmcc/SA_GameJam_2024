using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public static class SerializedPropertyExtensions
    {
        public delegate void PropertyCallback(SerializedProperty property);

        public static void LogChildProperties(this SerializedProperty property)
        {
            SerializedProperty endProperty = property.GetEndProperty(false);

            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(property, endProperty))
                {
                    return;
                }

                enterChildren = false;

                string type = property.propertyType == SerializedPropertyType.Generic ? ("Generic<" + property.type + ">") : property.propertyType.ToString();
                Debug.Log(property.displayName + " (" + type + ") = " + property.GetValue() + "\n" + property.propertyPath);
            }
        }

        public static void LogProperties(this SerializedObject serializedObject)
        {
            serializedObject.Update();
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    string type = property.propertyType == SerializedPropertyType.Generic ? ("Generic<" + property.type + ">") : property.propertyType.ToString();
                    Debug.Log(property.displayName + " (" + type + ") = " + property.GetValue() + "\n" + property.propertyPath);
                } while (property.NextVisible(false));
            }
        }

        public static void ForEachChildProperty(this SerializedProperty property, PropertyCallback propertyCallback)
        {
            SerializedProperty endProperty = property.GetEndProperty(false);

            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(property, endProperty))
                {
                    return;
                }

                enterChildren = false;
                propertyCallback(property);
            }
        }

        public static void ForEachProperty(this SerializedObject serializedObject, PropertyCallback propertyCallback)
        {
            serializedObject.Update();
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    propertyCallback(property);
                } while (property.NextVisible(false));
            }
        }

        public static SerializedProperty[] GetProperties(this SerializedObject serializedObject)
        {
            List<SerializedProperty> properties = new List<SerializedProperty>();

            serializedObject.Update();
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    properties.Add(property);
                } while (property.NextVisible(false));
            }

            return properties.ToArray();
        }

        public static SerializedProperty[] GetChildProperties(this SerializedProperty property)
        {
            List<SerializedProperty> properties = new List<SerializedProperty>();

            SerializedProperty endProperty = property.GetEndProperty(false);

            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(property, endProperty))
                {
                    break;
                }

                enterChildren = false;
                properties.Add(property);
            }

            return properties.ToArray();
        }

        public static object[] GetArrayValues(this SerializedProperty property)
        {
            if (!property.isArray) throw new ArgumentException("Property is not an array!");

            object[] values = new object[property.arraySize];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = property.GetArrayElementAtIndex(i).GetValue();
            }

            return values;
        }

        public static bool IsArrayElement(this SerializedProperty property)
        {
            if (property.propertyPath.Length < 16) return false;
            return property.propertyPath.Substring(property.propertyPath.Length - 14, 12) == ".Array.data[";
        }

        public static void AppendArrayElement(this SerializedProperty property, bool setDefaultValues = true)
        {
            property.InsertArrayElementAtIndex(property.arraySize);
            property.serializedObject.ApplyModifiedProperties();

            if (setDefaultValues)
            {
                SetDefaultValues(property.GetArrayElementAtIndex(property.arraySize - 1));
            }
        }

        public static void RemoveLastArrayElement(this SerializedProperty property)
        {
            property.arraySize = property.arraySize - 1;
            property.serializedObject.ApplyModifiedProperties();
        }

        public static void SetDefaultValues(this SerializedProperty property)
        {
            if (property.isArray)
            {
                property.ClearArray();
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                property.SetValue(Activator.CreateInstance(property.GetValueType()));
            }
        }

        public static void SetValue(this SerializedProperty property, object value)
        {

            if (property.propertyType == SerializedPropertyType.Generic && value == null)
            {
                SetDefaultValues(property);
                return;
            }

            if (property.isArray)
            {
                if (value != null)
                {
                    object[] values = (object[])value;
                    property.arraySize = values.Length;
                    for (int i = 0; i < values.Length; i++)
                    {
                        property.GetArrayElementAtIndex(i).SetValue(values[i]);
                    }
                }

                return;
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic: SetGenericValue(property, value); return;
                case SerializedPropertyType.Integer: property.intValue = (int)value; return;
                case SerializedPropertyType.Boolean: property.boolValue = (bool)value; return;
                case SerializedPropertyType.Float: property.floatValue = (float)value; return;
                case SerializedPropertyType.String: property.stringValue = (string)value; return;
                case SerializedPropertyType.Color: property.colorValue = (Color)value; return;
                case SerializedPropertyType.ObjectReference: property.objectReferenceValue = (Object)value; return;
                case SerializedPropertyType.LayerMask: property.intValue = (int)value; return;
                case SerializedPropertyType.Enum: property.enumValueIndex = (int)value; return;
                case SerializedPropertyType.Vector2: property.vector2Value = (Vector2)value; return;
                case SerializedPropertyType.Vector3: property.vector3Value = (Vector3)value; return;
                case SerializedPropertyType.Vector4: property.vector4Value = (Vector4)value; return;
                case SerializedPropertyType.Rect: property.rectValue = (Rect)value; return;
                case SerializedPropertyType.ArraySize: property.arraySize = (int)value; return;
                case SerializedPropertyType.Character: property.intValue = (int)value; return;
                case SerializedPropertyType.AnimationCurve: property.animationCurveValue = (AnimationCurve)value; return;
                case SerializedPropertyType.Bounds: property.boundsValue = (Bounds)value; return;
                case SerializedPropertyType.Gradient: SetGenericValue(property, (Gradient)value); return;
                case SerializedPropertyType.Quaternion: property.quaternionValue = (Quaternion)value; return;
                case SerializedPropertyType.ExposedReference: property.exposedReferenceValue = (Object)value; return;
                case SerializedPropertyType.Vector2Int: property.vector2IntValue = (Vector2Int)value; return;
                case SerializedPropertyType.Vector3Int: property.vector3IntValue = (Vector3Int)value; return;
                case SerializedPropertyType.RectInt: property.rectIntValue = (RectInt)value; return;
                case SerializedPropertyType.BoundsInt: property.boundsIntValue = (BoundsInt)value; return;

                default: throw new NotImplementedException("Unimplemented propertyType: " + property.propertyType);
            }
        }

        public static T GetValue<T>(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic: return GetGenericValue<T>(property);
                default: return (T)property.GetValue();
            }
        }

        public static object GetValue(this SerializedProperty property)
        {

            switch (property.propertyType)
            {
                case SerializedPropertyType.Enum: return Enum.ToObject(property.GetValueType(), property.enumValueIndex);
                case SerializedPropertyType.Generic: return GetGenericValue(property);
                case SerializedPropertyType.Integer: return property.intValue;
                case SerializedPropertyType.Boolean: return property.boolValue;
                case SerializedPropertyType.Float: return property.floatValue;
                case SerializedPropertyType.String: return property.stringValue;
                case SerializedPropertyType.Color: return property.colorValue;
                case SerializedPropertyType.ObjectReference: return property.objectReferenceValue;
                case SerializedPropertyType.LayerMask: return property.intValue;
                case SerializedPropertyType.Vector2: return property.vector2Value;
                case SerializedPropertyType.Vector3: return property.vector3Value;
                case SerializedPropertyType.Rect: return property.rectValue;
                case SerializedPropertyType.ArraySize: return property.intValue;
                case SerializedPropertyType.Character: return property.intValue;
                case SerializedPropertyType.AnimationCurve: return property.animationCurveValue;
                case SerializedPropertyType.Bounds: return property.boundsValue;
                case SerializedPropertyType.Gradient: return SafeGradientValue(property);
                case SerializedPropertyType.Quaternion: return property.quaternionValue;
                case SerializedPropertyType.ExposedReference: return property.exposedReferenceValue;
                case SerializedPropertyType.FixedBufferSize: return property.fixedBufferSize;
                case SerializedPropertyType.Vector2Int: return property.vector2IntValue;
                case SerializedPropertyType.Vector3Int: return property.vector3IntValue;
                case SerializedPropertyType.RectInt: return property.rectIntValue;
                case SerializedPropertyType.BoundsInt: return property.boundsIntValue;

                default: throw new NotImplementedException("Unimplemented propertyType " + property.propertyType + ".");
            }
        }

        static void SetGenericValue(SerializedProperty property, object value)
        {
            Type type = property.GetValueType();

            SerializedProperty endProperty = property.GetEndProperty(false);

            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(property, endProperty)) return;

                enterChildren = false;
                property.SetValue(type.GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(value));

            }
        }


        static object GetGenericValue(SerializedProperty property)
        {
            //  Debug.Log(property.propertyPath);

            string[] paths = property.propertyPath.Replace(".Array.data[", "[").Split('.');

            // Debug.Log(paths.GetElementsAsString(false));

            FieldInfo field = null;
            object currentObject = property.serializedObject.targetObject;

            // Debug.Log(currentObject);

            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                int arrayIndex = -1;
                int startIndex = path.IndexOf('[');

                // Debug.Log(path + " " + arrayIndex + " " + startIndex);

                if (startIndex > 0)
                {
                    arrayIndex = Int32.Parse(path.Substring(startIndex + 1, path.IndexOf(']') - startIndex - 1));
                    path = path.Substring(0, startIndex);
                }

                field = currentObject.GetType().GetField(path, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (field == null) return null;

                if (arrayIndex >= 0)
                {
                    currentObject = (field.GetValue(currentObject) as object[])[arrayIndex];
                }
                else if (i < paths.Length - 1)
                {
                    currentObject = field.GetValue(currentObject);
                }
            }

            //Debug.Log(field + " " + currentObject);

            return field.GetValue(currentObject);
        }

        static T GetGenericValue<T>(SerializedProperty property)
        {
            return (T)GetGenericValue(property);
        }

        public static Type GetValueType(this SerializedProperty property)
        {
            if (property.isArray) return GetTypeFromField();

            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Enum:
                    return GetTypeFromField();

                case SerializedPropertyType.Integer: return typeof(int);
                case SerializedPropertyType.Boolean: return typeof(bool);
                case SerializedPropertyType.Float: return typeof(float);
                case SerializedPropertyType.String: return typeof(string);
                case SerializedPropertyType.Color: return typeof(Color);
                case SerializedPropertyType.ObjectReference:

                    string typeName = property.type.RemoveFromStart("PPtr<").RemoveFromStart("$");
                    typeName = typeName.Substring(0, typeName.Length - 1);

                    return TypeUtils.GetTypeFromName(typeName);
                case SerializedPropertyType.LayerMask: return typeof(LayerMask);
                case SerializedPropertyType.Vector2: return typeof(Vector2);
                case SerializedPropertyType.Vector3: return typeof(Vector3);
                case SerializedPropertyType.Rect: return typeof(Rect);
                case SerializedPropertyType.ArraySize: return typeof(int);
                case SerializedPropertyType.Character: return typeof(char);
                case SerializedPropertyType.AnimationCurve: return typeof(AnimationCurve);
                case SerializedPropertyType.Bounds: return typeof(Bounds);
                case SerializedPropertyType.Gradient: return typeof(Gradient);
                case SerializedPropertyType.Quaternion: return typeof(Quaternion);
                case SerializedPropertyType.ExposedReference: return typeof(Object);
                case SerializedPropertyType.FixedBufferSize: return typeof(int);
                case SerializedPropertyType.Vector2Int: return typeof(Vector2Int);
                case SerializedPropertyType.Vector3Int: return typeof(Vector3Int);
                case SerializedPropertyType.RectInt: return typeof(RectInt);
                case SerializedPropertyType.BoundsInt: return typeof(BoundsInt);

                default: throw new NotImplementedException("Unimplemented propertyType: " + property.propertyType);
            }

            Type GetTypeFromField()
            {
                string[] paths = property.propertyPath.Replace(".Array.data[", "[").Split('.');

                FieldInfo field = null;
                object currentObject = property.serializedObject.targetObject;

                for (int i = 0; i < paths.Length; i++)
                {
                    string path = paths[i];
                    int arrayIndex = -1;
                    int startIndex = path.IndexOf('[');

                    if (startIndex > 0)
                    {
                        arrayIndex = Int32.Parse(path.Substring(startIndex + 1, path.IndexOf(']') - startIndex - 1));
                        path = path.Substring(0, startIndex);
                    }


                    field = currentObject.GetType().GetField(path, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    if (field == null) return null;

                    if (arrayIndex >= 0)
                    {
                        currentObject = (field.GetValue(currentObject) as IList)[arrayIndex];
                        if (i == paths.Length - 1) return currentObject.GetType();
                    }
                    else if (i < paths.Length - 1)
                    {
                        currentObject = field.GetValue(currentObject);
                    }
                }

                return field.FieldType;
            }
        }

        public static Type GetObjectReferenceType(this SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference) throw new ArgumentException("SerializedProperty is not an ObjectReference property");

            string typeName = property.type.RemoveFromStart("PPtr<$");
            typeName = typeName.Substring(0, typeName.Length - 1);

            return TypeUtils.GetTypeFromName(typeName);
        }

        public static T GetAttribute<T>(this SerializedProperty property) where T : Attribute
        {
            string[] paths = property.propertyPath.Replace(".Array.data[", "[").Split('.');

            FieldInfo field = null;
            object currentObject = property.serializedObject.targetObject;

            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                int arrayIndex = -1;
                int startIndex = path.IndexOf('[');

                if (startIndex > 0)
                {
                    arrayIndex = Int32.Parse(path.Substring(startIndex + 1, path.IndexOf(']') - startIndex - 1));
                    path = path.Substring(0, startIndex);
                }

                field = currentObject.GetType().GetField(path, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (field == null) return null;

                if (arrayIndex >= 0)
                {
                    IList list = (field.GetValue(currentObject) as IList);
                    if (list == null || list.Count <= arrayIndex) return null;

                    currentObject = list[arrayIndex];
                }
                else if (i < paths.Length - 1)
                {
                    currentObject = field.GetValue(currentObject);
                }
            }

            return field.GetAttribute<T>();
        }

        public static string GetTooltip(this SerializedProperty property)
        {
            TooltipAttribute tooltipAttribute = property.GetAttribute<TooltipAttribute>();
            if (tooltipAttribute != null)
            {
                return tooltipAttribute.tooltip;
            }

            return "";
        }

        static Gradient SafeGradientValue(SerializedProperty property)
        {
            BindingFlags instanceAnyPrivacyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo propertyInfo = typeof(SerializedProperty).GetProperty("gradientValue", instanceAnyPrivacyBindingFlags, null, typeof(Gradient), new Type[0], null);

            if (propertyInfo == null) return null;

            return propertyInfo.GetValue(property, null) as Gradient;
        }

        public static SerializedProperty[] GetChildProperties(this SerializedProperty property, bool visibleOnly, bool enterChildren)
        {
            SerializedProperty copy = property.Copy();
            SerializedProperty endProperty = copy.GetEndProperty(true);
            List<SerializedProperty> childProperties = new List<SerializedProperty>();

            bool enter = true;
            while (visibleOnly ? copy.NextVisible(enter) : copy.Next(enter))
            {
                if (SerializedProperty.EqualContents(copy, endProperty)) break;

                enter = enterChildren;
                childProperties.Add(copy.Copy());
            }

            return childProperties.ToArray();
        }

        public static int GetIndexInArray(this SerializedProperty property)
        {
            if (!property.IsArrayElement()) return -1;

            int num = property.propertyPath.LastIndexOf('[') + 1;
            int length = property.propertyPath.LastIndexOf(']') - num;
            return int.Parse(property.propertyPath.Substring(num, length));
        }

        public static Matrix4x4 GetMatrix4x4Value(this SerializedProperty property)
        {
            Matrix4x4 matrix = new Matrix4x4();

            matrix.m00 = property.FindPropertyRelative("e00").floatValue;
            matrix.m01 = property.FindPropertyRelative("e01").floatValue;
            matrix.m02 = property.FindPropertyRelative("e02").floatValue;
            matrix.m03 = property.FindPropertyRelative("e03").floatValue;

            matrix.m10 = property.FindPropertyRelative("e10").floatValue;
            matrix.m11 = property.FindPropertyRelative("e11").floatValue;
            matrix.m12 = property.FindPropertyRelative("e12").floatValue;
            matrix.m13 = property.FindPropertyRelative("e13").floatValue;

            matrix.m20 = property.FindPropertyRelative("e20").floatValue;
            matrix.m21 = property.FindPropertyRelative("e21").floatValue;
            matrix.m22 = property.FindPropertyRelative("e22").floatValue;
            matrix.m23 = property.FindPropertyRelative("e23").floatValue;

            matrix.m30 = property.FindPropertyRelative("e30").floatValue;
            matrix.m31 = property.FindPropertyRelative("e31").floatValue;
            matrix.m32 = property.FindPropertyRelative("e32").floatValue;
            matrix.m33 = property.FindPropertyRelative("e33").floatValue;

            return matrix;
        }

        public static void SetMatrix4x4Value(this SerializedProperty property, Matrix4x4 matrix)
        {
            property.FindPropertyRelative("e00").floatValue = matrix.m00;
            property.FindPropertyRelative("e01").floatValue = matrix.m01;
            property.FindPropertyRelative("e02").floatValue = matrix.m02;
            property.FindPropertyRelative("e03").floatValue = matrix.m03;

            property.FindPropertyRelative("e10").floatValue = matrix.m10;
            property.FindPropertyRelative("e11").floatValue = matrix.m11;
            property.FindPropertyRelative("e12").floatValue = matrix.m12;
            property.FindPropertyRelative("e13").floatValue = matrix.m13;

            property.FindPropertyRelative("e20").floatValue = matrix.m20;
            property.FindPropertyRelative("e21").floatValue = matrix.m21;
            property.FindPropertyRelative("e22").floatValue = matrix.m22;
            property.FindPropertyRelative("e23").floatValue = matrix.m23;

            property.FindPropertyRelative("e30").floatValue = matrix.m30;
            property.FindPropertyRelative("e31").floatValue = matrix.m31;
            property.FindPropertyRelative("e32").floatValue = matrix.m32;
            property.FindPropertyRelative("e33").floatValue = matrix.m33;
        }

        /*
    public static void SetAsPrefabOverride(this SerializedProperty property)
    {
 
         switch (property.propertyType)
        {
            case SerializedPropertyType.Generic: return GetGenericValue<T>(property);
            case SerializedPropertyType.ObjectReference: return (T)(object)property.objectReferenceValue;
            case SerializedPropertyType.Gradient: return (T)(object)SafeGradientValue(property);
            case SerializedPropertyType.ExposedReference: return (T)(object)property.exposedReferenceValue;
            case SerializedPropertyType.AnimationCurve: return (T)(object)property.animationCurveValue;

            default:
                PrefabUtility.SetPropertyModifications(property.serializedObject,new []{new PropertyModification(}), });
                break;
        }

            PrefabUtility.SetPropertyModifications();
    }
    */
    }
}
