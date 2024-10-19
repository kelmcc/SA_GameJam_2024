using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(ExposeIfAttribute), true)]
    public class ExposeIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (attribute as ExposeIfAttribute).EvaluateCondition(property.serializedObject.targetObject) ? EditorGUI.GetPropertyHeight(property, label, true) : -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if ((attribute as ExposeIfAttribute).EvaluateCondition(property.serializedObject.targetObject)) EditorGUI.PropertyField(position, property, label, true);
        }

    }
}
