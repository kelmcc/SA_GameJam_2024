using System;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(EnumIndexedAttribute))]
    public class EnumIndexedDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            if (property.serializedObject.isEditingMultipleObjects)
            {
                GUI.Label(position, "Cannot multi-edit enum-indexed arrays");
            }
            else
            {

                string path = property.propertyPath;
                int index = GetIndex(path);
                string arrayPath = path.Substring(0, path.IndexOf(".Array"));

                SerializedProperty array = property.serializedObject.FindProperty(arrayPath);

                Type type = (attribute as EnumIndexedAttribute).Type;

                string[] names = Enum.GetNames(type);

                if (array.arraySize != names.Length)
                {
                    array.arraySize = names.Length;
                }

                if (index == 0)
                {
                    position.y += 10;
                }

                EditorGUI.PropertyField(position, property, new GUIContent(StringUtils.Titelize(names[index])), true);
            }


        }

        static int GetIndex(string path)
        {
            int substringIndex = path.LastIndexOf("[") + 1;
            return int.Parse(path.Substring(substringIndex, path.Length - substringIndex - 1));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (GetIndex(property.propertyPath) == 0)
            {
                return EditorGUI.GetPropertyHeight(property, label, true) + 10;
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
