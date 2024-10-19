using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(SerializableDateTime))]
    public class DateTimeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty binaryProperty = property.FindPropertyRelative("_binary");
            DateTime date = DateTime.FromBinary(binaryProperty.longValue);

            label = EditorGUI.BeginProperty(rect, label, property);

            rect = EditorGUI.PrefixLabel(rect, label);
            Rect[] rects = rect.DivideHorizontallyEqually(6, 3f);

            EditorGUI.BeginChangeCheck();


            int previousIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect[] dayRects = rects[0].DivideHorizontally(14f, 0f);
            EditorGUI.LabelField(dayRects[0], "D");
            int day = EditorGUI.DelayedIntField(dayRects[1], date.Day);

            Rect[] monthRects = rects[1].DivideHorizontally(16f, 0f);
            EditorGUI.LabelField(monthRects[0], "M");
            int month = EditorGUI.DelayedIntField(monthRects[1], date.Month);

            Rect[] yearRects = rects[2].DivideHorizontally(14f, 0f);
            EditorGUI.LabelField(yearRects[0], "Y");
            int year = EditorGUI.DelayedIntField(yearRects[1], date.Year);

            Rect[] hourRects = rects[3].DivideHorizontally(12f, 0f);
            EditorGUI.LabelField(hourRects[0], "h");
            int hour = EditorGUI.DelayedIntField(hourRects[1], date.Hour);

            Rect[] minuteRects = rects[4].DivideHorizontally(16f, 0f);
            EditorGUI.LabelField(minuteRects[0], "m");
            int minute = EditorGUI.DelayedIntField(minuteRects[1], date.Minute);

            Rect[] secondRects = rects[5].DivideHorizontally(12f, 0f);
            EditorGUI.LabelField(secondRects[0], "s");
            int second = EditorGUI.DelayedIntField(secondRects[1], date.Second);

            EditorGUI.indentLevel = previousIndent;

            if (EditorGUI.EndChangeCheck())
            {
                try
                {
                    date = new DateTime(year, month, day, hour, minute, second);
                    binaryProperty.longValue = date.ToBinary();
                }
                catch
                {

                }
            }

            EditorGUI.EndProperty();
        }
    }
}
