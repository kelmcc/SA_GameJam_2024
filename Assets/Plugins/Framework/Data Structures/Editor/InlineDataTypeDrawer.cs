using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Framework;

namespace Framework
{

    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(InlineDataType), true)]
    public class InlineDataTypeDrawer : PropertyDrawer
    {
        const float SPACING = 5f;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            Rect[] rects = rect.DivideHorizontally(EditorGUIUtility.labelWidth, 0);

            EditorGUI.LabelField(rects[0], label);
            DrawProperties(property.GetChildProperties(true, false), rects[1]);

            EditorGUI.EndProperty();
        }

        void DrawProperties(SerializedProperty[] properties, Rect rect)
        {
            float[] widths = new float[properties.Length];
            string[] labels = new string[properties.Length];

            float remainingWidth = (rect.width - (properties.Length - 1) * SPACING);
            float x = rect.x;
            int numExpandingRects = 0;

            for (int i = 0; i < properties.Length; i++)
            {
                InlineDataFieldAttribute attribute = properties[i].GetAttribute<InlineDataFieldAttribute>();
                if (attribute != null)
                {
                    if (attribute.Width > 0)
                    {
                        widths[i] = attribute.Width;
                        remainingWidth -= attribute.Width;
                    }
                    else
                    {
                        widths[i] = -1;
                        numExpandingRects++;
                    }

                    labels[i] = attribute.Label;
                }
                else
                {
                    widths[i] = -1;
                    numExpandingRects++;
                }

                if (labels[i] == null)
                {
                    labels[i] = properties[i].displayName;
                }

            }

            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            for (int i = 0; i < properties.Length; i++)
            {
                float width = widths[i] < 0 ? (remainingWidth / numExpandingRects) : widths[i];
                float textWidth = EditorUtils.GetTextWidth(labels[i]);

                EditorGUIUtility.labelWidth = textWidth;
                EditorGUIUtility.fieldWidth = width - textWidth;

                EditorGUI.PropertyField(new Rect(x, rect.y, width, rect.height), properties[i], new GUIContent(labels[i], properties[i].GetTooltip()));

                x += width + SPACING;
            }

            EditorGUI.indentLevel = indentLevel;
        }

    }

}