using Framework;
using UnityEditor;
using UnityEngine;

namespace SoundManager
{
    [CustomPropertyDrawer(typeof(AmbienceSoundBank.LoopData))]
    public class AmbienceBankLoopDataDrawer : PropertyDrawer
    {
        private bool _isBank;
        private bool _isExpanded;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.FindPropertyRelative("_initialized").boolValue)
            {
                property.FindPropertyRelative("Sound").SetValue(new ClipOrBank(false));
                property.FindPropertyRelative("RolloffDistance").SetValue(SoundManagerSettings.Instance.AmbienceBank.LoopRolloffDistance);
                property.FindPropertyRelative("VolumeVariationRange").SetValue(SoundManagerSettings.Instance.AmbienceBank.LoopVariationVolumeRange);
                property.FindPropertyRelative("VaritaionFrequency").SetValue(SoundManagerSettings.Instance.AmbienceBank.LoopVaritaionFrequency);

                property.FindPropertyRelative("_initialized").boolValue = true;
            }

            _isBank = property.FindPropertyRelative("Sound").FindPropertyRelative("_isBank").boolValue;
            _isExpanded = property.isExpanded;

            string name = "Empty";
            Object value = property.FindPropertyRelative("Sound").FindPropertyRelative(_isBank ? "_soundBank" : "_audioClip").objectReferenceValue;

            if (value != null)
            {
                name = value.name;
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.PropertyField(property, new GUIContent(name), false);
            property.ForEachChildProperty(DrawProperty);
            EditorGUI.EndProperty();
        }

        void DrawProperty(SerializedProperty property)
        {
            if (!_isExpanded) return;
            if (property.name == "_initialized") return;
            if (property.name == "RolloffDistance" && _isBank) return;
            if (property.name == "VolumeRange" && _isBank) return;

            EditorGUILayout.PropertyField(property, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight;
        }
    }
}
