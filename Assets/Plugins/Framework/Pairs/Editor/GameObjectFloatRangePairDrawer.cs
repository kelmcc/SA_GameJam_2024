using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(GameObjectFloatRangePair))]
    public class GameObjectFloatRangePairDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            rect = EditorGUI.PrefixLabel(rect, label);

            Rect minRect = new Rect(rect.x, rect.y, 60, rect.height);
            Rect maxRect = new Rect(rect.x + 65, rect.y, 60, rect.height);
            Rect objectRect = new Rect(rect.x + 130, rect.y, rect.width - 130, rect.height);

            SerializedProperty minProperty = property.FindPropertyRelative("Range").FindPropertyRelative("_min");
            SerializedProperty maxProperty = property.FindPropertyRelative("Range").FindPropertyRelative("_max");

            int previousIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(minRect, minProperty, GUIContent.none);
            EditorGUI.PropertyField(maxRect, maxProperty, GUIContent.none);
            EditorGUI.PropertyField(objectRect, property.FindPropertyRelative("GameObject"), GUIContent.none);

            if (EditorGUI.EndChangeCheck() && minProperty.floatValue > maxProperty.floatValue)
            {
                maxProperty.floatValue = minProperty.floatValue;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorGUI.indentLevel = previousIndent;

            EditorGUI.EndProperty();
        }

    }
}



