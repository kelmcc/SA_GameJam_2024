using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(Spring))]
    public class SpringDrawer : SpringBaseDrawer { }

    [CustomPropertyDrawer(typeof(Spring2))]
    public class Spring2Drawer : SpringBaseDrawer { }

    [CustomPropertyDrawer(typeof(Spring3))]
    public class Spring3Drawer : SpringBaseDrawer { }


    public abstract class SpringBaseDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect totalRect, SerializedProperty property, GUIContent label)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            int indentLevel = EditorGUI.indentLevel;

            EditorGUI.BeginProperty(totalRect, label, property);

            Rect rect = EditorGUI.PrefixLabel(totalRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(label.text, property.GetTooltip()));

            SerializedProperty frequencyProperty = property.FindPropertyRelative("_frequency");
            SerializedProperty dampingProperty = property.FindPropertyRelative("_damping");
            SerializedProperty scaleProperty = property.FindPropertyRelative("_useUnscaledTime");

            const float SPACING = 3f;
            bool smallScale = rect.width < 350;

            float width = rect.width - SPACING * 2;
            float scaleWidth = smallScale ? width * 0.3f : width * 0.33f;
            float floatWidth = (width - scaleWidth) * 0.5f;


            Rect[] rects = rect.DivideHorizontallyWithSpacing(SPACING, floatWidth, floatWidth, scaleWidth);


            EditorGUI.indentLevel = 0;

            EditorGUIUtility.labelWidth = 65f;
            EditorGUI.PropertyField(rects[0], frequencyProperty, new GUIContent("Frequency"));


            EditorGUIUtility.labelWidth = 55f;
            EditorGUI.PropertyField(rects[1], dampingProperty, new GUIContent("Damping"));


            GUIContent[] options = { new GUIContent(smallScale ? "Scaled" : "Scaled Time"), new GUIContent(smallScale ? "Unscaled" : "Unscaled Time") };

            EditorGUI.BeginChangeCheck();
            scaleProperty.boolValue = EditorGUI.Popup(rects[2], scaleProperty.boolValue ? 1 : 0, options) == 0 ? false : true;
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indentLevel;

            EditorGUI.EndProperty();
        }
    }

}