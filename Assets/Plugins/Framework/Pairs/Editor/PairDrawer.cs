using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{

    public abstract class PairDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            rect = EditorGUI.PrefixLabel(rect, label);

            float width = GetFirstFieldWidth();
            float margin = GetMargin();

            if (width == 0)
            {
                width = (rect.width - margin) * 0.5f;
            }

            Rect numberRect = new Rect(rect.x, rect.y, width, rect.height);
            Rect objectRect = new Rect(rect.x + width + margin, rect.y, rect.width - width - margin, rect.height);

            int previousIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;


            EditorGUI.PropertyField(numberRect, property.FindPropertyRelative(GetFirstFieldName()), GUIContent.none);
            EditorGUI.PropertyField(objectRect, property.FindPropertyRelative(GetSecondFieldName()), GUIContent.none);

            EditorGUI.indentLevel = previousIndent;

            EditorGUI.EndProperty();
        }

        protected abstract string GetFirstFieldName();
        protected abstract string GetSecondFieldName();

        protected virtual float GetMargin() => 5f;
        protected virtual float GetFirstFieldWidth() => 0f;

    }
}
