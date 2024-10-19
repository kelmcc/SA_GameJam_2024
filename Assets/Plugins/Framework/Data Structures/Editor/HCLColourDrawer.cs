using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(HCLColour))]
    public class HCLColourDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect totalRect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(totalRect, label, property);

            SerializedProperty hProperty = property.FindPropertyRelative("_h");
            SerializedProperty cProperty = property.FindPropertyRelative("_c");
            SerializedProperty lProperty = property.FindPropertyRelative("_l");
            SerializedProperty alphaProperty = property.FindPropertyRelative("_a");

            Color colour = new HCLColour(hProperty.floatValue, cProperty.floatValue, lProperty.floatValue, alphaProperty.floatValue).ToRGB();

            EditorGUI.BeginChangeCheck();

            HCLColour hclColour = new HCLColour(EditorGUI.ColorField(totalRect, label, colour));


            if (EditorGUI.EndChangeCheck())
            {
                hProperty.floatValue = hclColour.H;
                cProperty.floatValue = hclColour.C;
                lProperty.floatValue = hclColour.L;
                alphaProperty.floatValue = hclColour.A;
            }

            EditorGUI.EndProperty();
        }

    }
}