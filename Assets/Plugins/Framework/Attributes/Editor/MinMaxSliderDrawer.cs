using System;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute), true)]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            Type type = property.GetValueType();
            if (type == typeof(FloatRange) || type == typeof(IntRange))
            {
                float minLimit = ((MinMaxSliderAttribute)attribute).Min;
                float maxLimit = ((MinMaxSliderAttribute)attribute).Max;

                SerializedProperty minProperty = property.FindPropertyRelative("_min");
                SerializedProperty maxProperty = property.FindPropertyRelative("_max");

                float minValue = minProperty.floatValue;
                float maxValue = maxProperty.floatValue;

                EditorGUI.BeginProperty(rect, label, property);
                EditorGUI.BeginChangeCheck();

                EditorGUI.MinMaxSlider(rect, label, ref minValue, ref maxValue, minLimit, maxLimit);

                if (EditorGUI.EndChangeCheck())
                {
                    minProperty.floatValue = minValue;
                    maxProperty.floatValue = maxValue;

                    property.serializedObject.ApplyModifiedProperties();
                }

                EditorGUI.EndProperty();
            }

            else
            {
                EditorGUI.LabelField(rect, new GUIContent("Min Max slider only works on ranges!"), "ErrorLabel");
            }
        }




    }
}

