using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(XYZColour))]
    public class XYZColourDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect totalRect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(totalRect, label, property);

            SerializedProperty xProperty = property.FindPropertyRelative("_x");
            SerializedProperty yProperty = property.FindPropertyRelative("_y");
            SerializedProperty zProperty = property.FindPropertyRelative("_z");
            SerializedProperty alphaProperty = property.FindPropertyRelative("_a");

            EditorGUI.BeginChangeCheck();
            XYZColour xyzColour = new XYZColour(EditorGUI.ColorField(totalRect, label, new XYZColour(xProperty.floatValue, yProperty.floatValue, zProperty.floatValue, alphaProperty.floatValue).ToRGB()));

            if (EditorGUI.EndChangeCheck())
            {
                xProperty.floatValue = xyzColour.X;
                yProperty.floatValue = xyzColour.Y;
                zProperty.floatValue = xyzColour.Z;
                alphaProperty.floatValue = xyzColour.A;
            }

            EditorGUI.EndProperty();
        }

    }
}