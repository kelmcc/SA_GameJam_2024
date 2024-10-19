using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(HideFieldAttribute), true)]
    public class HideDrawer : PropertyDrawer
    {


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

        }




    }
}
