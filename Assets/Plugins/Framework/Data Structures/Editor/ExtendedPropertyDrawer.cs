using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEditor;
using UnityEngine;

namespace Framework
{

    public abstract class ExtendedPropertyDrawer : PropertyDrawer
    {
        protected abstract int GetExtraLineCount();
        protected abstract void OnExtraGUI(Rect rect, SerializedProperty property, GUIContent label);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            if (property.isExpanded)
            {
                int lines = GetExtraLineCount();
                float width = position.width - (EditorGUI.indentLevel + 1) * EditorUtils.INDENT_WIDTH;
                float height = lines * EditorGUIUtility.singleLineHeight + (lines - 1) * EditorGUIUtility.standardVerticalSpacing;

                OnExtraGUI(new Rect(position.xMax - width, position.yMax - height, width, height), property, label);
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return EditorGUI.GetPropertyHeight(property) + GetExtraLineCount() * (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight);
            }

            return EditorGUI.GetPropertyHeight(property);
        }

    }
}
