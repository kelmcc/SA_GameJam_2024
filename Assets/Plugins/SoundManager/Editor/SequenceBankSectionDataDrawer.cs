using Framework;
using UnityEditor;
using UnityEngine;

namespace SoundManager
{
    [CustomPropertyDrawer(typeof(SequenceSoundBank.SectionData))]
    public class SequenceBankSectionDataDrawer : PropertyDrawer
    {

        private bool _isExpanded;
        private bool _loop;
        private bool _loopable;
        private bool _isBank;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.FindPropertyRelative("_initialized").boolValue)
            {
                property.FindPropertyRelative("Sound").SetValue(new ClipOrBank(true));
                property.FindPropertyRelative("RolloffDistance").SetValue(SoundManagerSettings.Instance.SequenceBank.SectionRolloffDistance);
                property.FindPropertyRelative("Loop").SetValue(SoundManagerSettings.Instance.SequenceBank.SectionLooping);
                property.FindPropertyRelative("Repititions").SetValue(SoundManagerSettings.Instance.SequenceBank.SectionRepititions);
                property.FindPropertyRelative("Delay").SetValue(SoundManagerSettings.Instance.SequenceBank.SectionDelay);

                property.FindPropertyRelative("_initialized").boolValue = true;
            }

            _isBank = property.FindPropertyRelative("Sound").FindPropertyRelative("_isBank").boolValue;
            _loop = property.FindPropertyRelative("Loop").boolValue;
            _isExpanded = property.isExpanded;

            string name = "Empty";
            Object value = property.FindPropertyRelative("Sound").FindPropertyRelative(_isBank ? "_soundBank" : "_audioClip").objectReferenceValue;

            if (value != null)
            {
                name = value.name;
            }

            _loopable = true;
            if (_isBank)
            {
                SoundBank bank = property.FindPropertyRelative("Sound").FindPropertyRelative("_soundBank").objectReferenceValue as SoundBank;

                if (bank != null)
                {
                    _loopable = bank.GetType() != typeof(AmbienceSoundBank);
                }
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
            if (property.name == "Loop" && !_loopable) return;
            if (property.name == "Repititions" && (_loop || !_loopable)) return;

            EditorGUILayout.PropertyField(property, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight;
        }
    }
}
