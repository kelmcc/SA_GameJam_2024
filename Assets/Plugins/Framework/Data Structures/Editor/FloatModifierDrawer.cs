using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(FloatModifier))]
    public class FloatModifierDrawer : PropertyDrawer
    {

        private bool _expanded;
        private float _height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {

            _height = rect.y;

            SerializedProperty operationProperty = property.FindPropertyRelative("_operation");
            SerializedProperty operandProperty = property.FindPropertyRelative("_operand");
            SerializedProperty variationProperty = property.FindPropertyRelative("_variation");
            SerializedProperty fixedProperty = property.FindPropertyRelative("_fixed");
            SerializedProperty floatRangeProperty = property.FindPropertyRelative("_floatRange");
            SerializedProperty intRangeProperty = property.FindPropertyRelative("_intRange");
            SerializedProperty alwaysRandomiseProperty = property.FindPropertyRelative("_alwaysRandomise");


            EditorGUI.BeginProperty(rect, label, property);

            rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            _expanded = EditorGUI.Foldout(rect, _expanded, new GUIContent(label.text + ": " + property.GetValue<FloatModifier>().GetLongLabel()), true);

            if (_expanded)
            {

                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.indentLevel++;

                EditorGUI.PropertyField(rect, operationProperty);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, operandProperty);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, variationProperty);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                switch ((FloatModifier.VariationType)variationProperty.enumValueIndex)
                {
                    case FloatModifier.VariationType.FixedValue:
                        EditorGUI.PropertyField(rect, fixedProperty, new GUIContent("Value")); break;
                    case FloatModifier.VariationType.IntRange:
                        EditorGUI.PropertyField(rect, intRangeProperty, new GUIContent("Value"));
                        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, alwaysRandomiseProperty); break;
                    case FloatModifier.VariationType.FloatRange:
                        EditorGUI.PropertyField(rect, floatRangeProperty, new GUIContent("Value"));
                        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, alwaysRandomiseProperty); break;
                }

            }

            EditorGUI.EndProperty();
            _height = rect.y - _height + EditorGUIUtility.singleLineHeight;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _height;
        }


    }
}
