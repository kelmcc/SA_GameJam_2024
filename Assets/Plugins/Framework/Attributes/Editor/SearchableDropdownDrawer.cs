using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{

    [CustomPropertyDrawer(typeof(SearchableDropdownAttribute))]
    public class SearchableDropdownDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (property.type != "Enum")
            {
                GUIStyle errorStyle = "CN EntryErrorIconSmall";
                GUI.Label(rect.WithWidth(errorStyle.fixedWidth), "", errorStyle);
                GUI.Label(rect, "SearchableDropdown can only be used on enum fields.");

                return;
            }

            EditorGUI.BeginProperty(rect, label, property);

            SearchablePopup.DrawButton(rect, label, property.enumDisplayNames, property.enumValueIndex, index =>
            {
                property.enumValueIndex = index;
                property.serializedObject.ApplyModifiedProperties();
            });


            EditorGUI.EndProperty();
        }

    }

}
