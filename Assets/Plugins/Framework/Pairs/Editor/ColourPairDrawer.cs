using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(ColourPair))]
    public class ColourPairDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect totalRect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(totalRect, label, property);

            Rect rect = EditorGUI.PrefixLabel(totalRect, GUIUtility.GetControlID(FocusType.Passive), label);
            rect.width *= 0.5f;

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("_first"), GUIContent.none);

            rect.x += rect.width;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("_second"), GUIContent.none);

            EditorGUI.EndProperty();
        }

    }
}