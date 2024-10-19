using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(OptionalSettings), true)]
    public class OptionalSettingsDrawer : PropertyDrawer
    {

        private bool _isExpanded;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);


            SerializedProperty enabledProperty = property.FindPropertyRelative("Enabled");

            rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            _isExpanded = EditorGUI.Foldout(rect.WithWidth(EditorGUIUtility.labelWidth), _isExpanded, GUIContent.none, true);


            EditorGUI.PropertyField(rect, enabledProperty, label);

            if (_isExpanded)
            {
                EditorGUI.indentLevel++;

                EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);

                property.ForEachChildProperty(p =>
                {
                    if (p.propertyPath != enabledProperty.propertyPath)
                    {

                        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, p);
                    }
                });

                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_isExpanded) return EditorGUIUtility.singleLineHeight;

            float height = 0;

            property.ForEachChildProperty(p => { height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; });

            return height - EditorGUIUtility.standardVerticalSpacing;
        }

    }
}
