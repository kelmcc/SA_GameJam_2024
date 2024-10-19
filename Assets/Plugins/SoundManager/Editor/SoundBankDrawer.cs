using Framework;
using UnityEditor;
using UnityEngine;

namespace SoundManager
{
    [CustomPropertyDrawer(typeof(SoundBank), true)]
    public class SoundBankDrawer : PropertyDrawer
    {
        private SerializedProperty _property;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            _property = property;

            EditorGUI.BeginProperty(rect, label, property);

            if (Event.current.type == EventType.ContextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create New"), false, CreateNew);
                menu.ShowAsContext();
            }

            EditorGUI.PropertyField(rect, property, label);
            EditorGUI.EndProperty();
        }

        void CreateNew()
        {
            SoundBank bank = SoundManagerEditorUtils.CreateNewSoundBank(_property.GetValueType(), _property.displayName);
            _property.objectReferenceValue = bank;
            _property.serializedObject.ApplyModifiedProperties();
        }
    }
}