using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangeDrawer : PropertyDrawer
    {
        public static void Draw(Rect totalRect, SerializedProperty property, GUIContent label, bool isFloat, ClampAttribute clamp)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            int indentLevel = EditorGUI.indentLevel;

            EditorGUI.BeginProperty(totalRect, label, property);

            Rect rect = EditorGUI.PrefixLabel(totalRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(label.text, property.GetTooltip()));
            SerializedProperty minProperty = property.FindPropertyRelative("_min");
            SerializedProperty maxProperty = property.FindPropertyRelative("_max");

            float valueWidth = (rect.width * 0.5f);

            Rect minValueRect = new Rect(rect.xMin, rect.y, valueWidth - 3f, rect.height);
            Rect maxValueRect = new Rect(rect.x + (rect.width * 0.5f), rect.y, valueWidth, rect.height);

            bool changedMin = false;
            bool changedMax = false;


            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 30f;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(minValueRect, minProperty, new GUIContent("Min"));
            if (EditorGUI.EndChangeCheck())
            {
                changedMin = true;
            }


            EditorGUIUtility.labelWidth = 33f;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(maxValueRect, maxProperty, new GUIContent("Max"));
            if (EditorGUI.EndChangeCheck())
            {
                changedMax = true;
            }
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indentLevel;

            if (changedMin || changedMax)
            {
                if (clamp != null)
                {
                    if (isFloat)
                    {
                        minProperty.floatValue = Mathf.Clamp(minProperty.floatValue, clamp.MinFloat, clamp.MaxFloat);
                        maxProperty.floatValue = Mathf.Clamp(maxProperty.floatValue, clamp.MinFloat, clamp.MaxFloat);
                    }
                    else
                    {
                        minProperty.intValue = Mathf.Clamp(minProperty.intValue, clamp.MinInt, clamp.MaxInt);
                        maxProperty.intValue = Mathf.Clamp(maxProperty.intValue, clamp.MinInt, clamp.MaxInt);
                    }
                }
            }

            if (changedMin)
            {
                if (isFloat && minProperty.floatValue > maxProperty.floatValue) maxProperty.floatValue = minProperty.floatValue;
                if (isFloat && maxProperty.floatValue < minProperty.floatValue) minProperty.floatValue = maxProperty.floatValue;

                if (!isFloat && minProperty.intValue > maxProperty.intValue) maxProperty.intValue = minProperty.intValue;
                if (!isFloat && maxProperty.intValue < minProperty.intValue) minProperty.intValue = maxProperty.intValue;
            }

            if (changedMax)
            {
                if (isFloat && maxProperty.floatValue < minProperty.floatValue) minProperty.floatValue = maxProperty.floatValue;
                if (isFloat && minProperty.floatValue > maxProperty.floatValue) maxProperty.floatValue = minProperty.floatValue;

                if (!isFloat && maxProperty.intValue < minProperty.intValue) minProperty.intValue = maxProperty.intValue;
                if (!isFloat && minProperty.intValue > maxProperty.intValue) maxProperty.intValue = minProperty.intValue;
            }




            if (changedMin || changedMax)
            {
                property.serializedObject.ApplyModifiedProperties();
            }


            EditorGUI.EndProperty();
        }

        public override void OnGUI(Rect totalRect, SerializedProperty property, GUIContent label)
        {
            Draw(totalRect, property, label, true, null);
        }

    }

    [CustomPropertyDrawer(typeof(IntRange))]
    public class IntRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect totalRect, SerializedProperty property, GUIContent label)
        {
            FloatRangeDrawer.Draw(totalRect, property, label, false, null);
        }
    }
}