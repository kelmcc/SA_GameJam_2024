using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(GameObjectIntRangePair))]
    public class GameObjectIntRangePairDrawer : PropertyDrawer
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

            if (EditorGUI.EndChangeCheck() && minProperty.intValue > maxProperty.intValue)
            {
                maxProperty.intValue = minProperty.intValue;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorGUI.indentLevel = previousIndent;

            EditorGUI.EndProperty();
        }

    }
}



