using System;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(ClampAttribute), true)]
    public class ClampDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            Type type = property.GetValueType();
            if (type == typeof(FloatRange))
            {
                FloatRangeDrawer.Draw(rect, property, label, true, attribute as ClampAttribute);
            }
            else if (type == typeof(IntRange))
            {
                FloatRangeDrawer.Draw(rect, property, label, false, attribute as ClampAttribute);
            }
            else
            {
                EditorGUI.BeginProperty(rect, label, property);

                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rect, property, label);

                if (EditorGUI.EndChangeCheck())
                {
                    ClampAttribute clamp = attribute as ClampAttribute;
                    if (property.propertyType == SerializedPropertyType.Float) property.floatValue = Mathf.Clamp(property.floatValue, clamp.MinFloat, clamp.MaxFloat);
                    if (property.propertyType == SerializedPropertyType.Integer) property.intValue = Mathf.Clamp(property.intValue, clamp.MinInt, clamp.MaxInt);
                }

                EditorGUI.EndProperty();
            }
        }
    }
}
