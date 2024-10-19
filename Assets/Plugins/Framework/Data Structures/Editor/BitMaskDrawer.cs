using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(BitMask))]
    public class BitMaskDrawer : PropertyDrawer
    {

        public float _height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        private bool _isEnumMode;
        private string[] _enumNames;

        public BitMaskDrawer()
        {

        }

        public BitMaskDrawer(bool enumMode, string[] enumNames)
        {
            _isEnumMode = enumMode;
            _enumNames = enumNames;
        }

        public override void OnGUI(Rect totalRect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty numFlagsProperty = property.FindPropertyRelative("_numFlags");
            SerializedProperty byteArrayProperty = property.FindPropertyRelative("_bytes");
            BitMask bitMask = property.GetValue<BitMask>();

            EditorGUI.BeginProperty(totalRect, label, property);
            Rect rect = new Rect(totalRect.x, totalRect.y, totalRect.width, EditorGUIUtility.singleLineHeight);

            property.isExpanded = EditorGUI.Foldout(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height), property.isExpanded, label, true);
            EditorGUI.LabelField(new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, rect.height), GetStatusLabel(bitMask));

            if (property.isExpanded)
            {

                EditorGUI.indentLevel++;
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                if (!_isEnumMode)
                {
                    numFlagsProperty.intValue = Mathf.Max(0, EditorGUI.IntField(rect, "Length", numFlagsProperty.intValue));
                    rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
                else
                {
                    numFlagsProperty.intValue = _enumNames.Length;
                }

                Rect buttonRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.5f, rect.height);
                if (GUI.Button(buttonRect, "NOTHING"))
                {
                    for (int i = 0; i < numFlagsProperty.intValue; i++)
                    {
                        SetFlag(i, false, byteArrayProperty);
                    }
                }

                buttonRect.x += buttonRect.width;
                if (GUI.Button(buttonRect, "EVERYTHING"))
                {
                    for (int i = 0; i < numFlagsProperty.intValue; i++)
                    {
                        SetFlag(i, true, byteArrayProperty);
                    }
                }

                if (numFlagsProperty.intValue != bitMask.NumFlags)
                {
                    Resize(numFlagsProperty.intValue, byteArrayProperty);
                }


                for (int i = 0; i < numFlagsProperty.intValue; i++)
                {
                    rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    bool oldValue = GetFlag(i, byteArrayProperty);
                    bool newValue = EditorGUI.Toggle(rect, _isEnumMode ? _enumNames[i] : "Flag " + i, oldValue);

                    if (oldValue != newValue)
                    {
                        SetFlag(i, newValue, byteArrayProperty);
                    }
                }

            }

            EditorGUI.EndProperty();
            _height = rect.y - totalRect.y + EditorGUIUtility.singleLineHeight;

            property.serializedObject.ApplyModifiedProperties();
        }



        string GetStatusLabel(BitMask mask)
        {
            List<int> setFlags = new List<int>();

            for (int i = 0; i < mask.NumFlags; i++)
            {
                if (mask.GetFlag(i))
                {
                    setFlags.Add(i);
                }
            }

            if (setFlags.Count == mask.NumFlags) return "Everything";
            if (setFlags.Count > 4)
            {
                return "Mixed (" + setFlags.Count + "/" + mask.NumFlags + ")";
            }

            if (setFlags.Count > 0)
            {
                StringBuilder label = new StringBuilder(_isEnumMode ? _enumNames[setFlags[0]] : setFlags[0].ToString());
                for (int i = 1; i < setFlags.Count; i++)
                {
                    label.Append(", " + (_isEnumMode ? _enumNames[setFlags[i]] : setFlags[i].ToString()));
                }

                return label.ToString();
            }

            return "Nothing";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _height;
        }

        public static void SetFlag(int index, bool value, SerializedProperty array)
        {

            int byteIndex = index / 8;
            int indexInByte = index % 8;

            SerializedProperty elementProperty = array.GetArrayElementAtIndex(byteIndex);

            if (value)
            {
                elementProperty.intValue = elementProperty.intValue | (1 << indexInByte);
            }
            else
            {
                elementProperty.intValue = elementProperty.intValue & ~(1 << indexInByte);
            }
        }

        public static bool GetFlag(int index, SerializedProperty array)
        {
            int byteIndex = index / 8;
            int indexInByte = index % 8;

            return (array.GetArrayElementAtIndex(byteIndex).intValue & (1 << indexInByte)) != 0;
        }

        public static void Resize(int numFlags, SerializedProperty array)
        {
            int newSize = (numFlags / 8) + (numFlags % 8 > 0 ? 1 : 0);

            byte[] oldBytes = new byte[Mathf.Min(newSize, array.arraySize)];
            for (int i = 0; i < oldBytes.Length; i++)
            {
                oldBytes[i] = (byte)array.GetArrayElementAtIndex(i).intValue;
            }

            array.arraySize = newSize;

            for (int i = 0; i < newSize; i++)
            {
                array.GetArrayElementAtIndex(i).intValue = i < oldBytes.Length ? oldBytes[i] : 0;
            }
        }



    }
}