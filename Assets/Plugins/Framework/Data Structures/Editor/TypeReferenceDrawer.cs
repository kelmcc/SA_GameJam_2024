using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    [CustomPropertyDrawer(typeof(TypeReference))]
    public class TypeReferenceDrawer : PropertyDrawer
    {

        private Type[] _subTypes;
        private MonoScript _selectedScript;

        private const float PICKER_WIDTH = 35f;


        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {

            SubtypeAttribute attribute = property.GetAttribute<SubtypeAttribute>();
            SerializedProperty typeNameProperty = property.FindPropertyRelative("_typeName");

            EditorGUI.BeginProperty(rect, label, property);

            List<string> options = new List<string>();
            int selectedValue = 0;
            Type selectedType = null;

            if (attribute != null)
            {

                if (_subTypes == null)
                {
                    _subTypes = attribute.Type.GetAllSubtypesInUnityAssemblies();
                }

                for (int i = 0; i < _subTypes.Length; i++)
                {
                    options.Add(_subTypes[i].Name);
                    if (typeNameProperty.stringValue == _subTypes[i].Name)
                    {
                        selectedValue = i;
                        selectedType = _subTypes[i];
                    }
                }

            }

            if (options.Count > 0)
            {

                if (selectedType != null)
                {
                    if (_selectedScript == null || _selectedScript.GetClass() != selectedType)
                    {
                        _selectedScript = EditorUtils.GetMonoScript(selectedType, false);
                    }
                }

                Rect popupRect = rect;
                if (_selectedScript != null)
                {
                    popupRect.width -= PICKER_WIDTH;
                }

                SearchablePopup.DrawButton(popupRect, label, options.ToArray(), selectedValue, index =>
                {
                    typeNameProperty.stringValue = options[index];
                    property.serializedObject.ApplyModifiedProperties();
                });


                if (_selectedScript != null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.indentLevel = 0;
                    EditorGUI.ObjectField(new Rect(rect.xMax - PICKER_WIDTH, rect.y, PICKER_WIDTH, rect.height), _selectedScript, typeof(MonoScript), false);
                    EditorGUI.EndDisabledGroup();
                }
            }

            EditorGUI.EndProperty();
        }

    }
}
