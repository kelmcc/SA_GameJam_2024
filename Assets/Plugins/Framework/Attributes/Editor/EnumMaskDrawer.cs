using System;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(EnumMaskAttribute))]
    public class EnumMaskDrawer : PropertyDrawer
    {

        private float _height = EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type enumType = (attribute as EnumMaskAttribute).Type;
            SerializedProperty byteArrayProperty = property.FindPropertyRelative("_bytes");
            SerializedProperty numFlagsProperty = property.FindPropertyRelative("_numFlags");



            if (!enumType.IsEnum)
            {
                ShowError(position, property, label, enumType + " is not an Enum type");
                return;
            }

            if (property.type != typeof(BitMask).Name)
            {
                ShowError(position, property, label, label.text + " is not a BitMask");
                return;
            }

            int enumLength = Enum.GetValues(enumType).Length;

            if (enumLength > 32)
            {
                BitMaskDrawer drawer = new BitMaskDrawer(true, Enum.GetNames(enumType));
                drawer.OnGUI(position, property, label);
                _height = drawer._height;
                return;
            }

            if (numFlagsProperty.intValue != enumLength)
            {
                numFlagsProperty.intValue = enumLength;
                BitMaskDrawer.Resize(enumLength, byteArrayProperty);
            }



            int oldMaskValue = 0;

            for (int i = 0; i < enumLength; i++)
            {
                if (BitMaskDrawer.GetFlag(i, byteArrayProperty))
                {
                    oldMaskValue = oldMaskValue | (1 << i);
                }
                else
                {
                    oldMaskValue = oldMaskValue & (~(1 << i));
                }
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            int newMaskValue = EditorGUI.MaskField(position, label, oldMaskValue, Enum.GetNames(enumType));

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < enumLength; i++)
                {
                    bool value = (newMaskValue & (1 << i)) != 0;
                    BitMaskDrawer.SetFlag(i, value, byteArrayProperty);
                }
            }


            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();

            _height = EditorGUIUtility.singleLineHeight;

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _height;
        }

        void ShowError(Rect position, SerializedProperty property, GUIContent label, string errorText)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.HelpBox(position, errorText, MessageType.None);
            EditorGUI.EndProperty();
        }
    }
}