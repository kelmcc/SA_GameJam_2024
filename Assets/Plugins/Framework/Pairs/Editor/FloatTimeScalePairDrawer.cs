using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(FloatTimeScalePair))]
    [CanEditMultipleObjects]
    public class FloatTimeScalePairDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            EditorGUI.LabelField(rect, label);


            rect.xMin += EditorGUIUtility.labelWidth;
            rect.width *= 0.5f;

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("_floatValue"), GUIContent.none);

            rect.x += rect.width;

            SerializedProperty isUnscaled = property.FindPropertyRelative("_isUnscaled");

            isUnscaled.boolValue = EditorGUI.Popup(rect, isUnscaled.boolValue ? 1 : 0, new[] { "Scaled Time", "Unscaled Time" }) == 0 ? false : true;

            EditorGUI.EndProperty();
        }

    }
}



