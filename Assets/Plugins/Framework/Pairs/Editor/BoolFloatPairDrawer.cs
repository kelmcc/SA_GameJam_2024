using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(BoolFloatPair))]
    public class BoolFloatPairDrawer : PropertyDrawer
    {
        private const float BOOL_WIDTH = 18f;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            Rect[] rects = rect.DivideHorizontally(EditorGUIUtility.labelWidth, BOOL_WIDTH, 0);
            EditorGUI.LabelField(rects[0], label);
            EditorGUI.PropertyField(rects[1], property.FindPropertyRelative("_enabled"), GUIContent.none);
            EditorGUI.PropertyField(rects[2], property.FindPropertyRelative("_value"), GUIContent.none);

            EditorGUI.EndProperty();
        }

    }
}



