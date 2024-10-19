using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    [CustomPropertyDrawer(typeof(PrefabDropdownAttribute), true)]
    public class PrefabDropdownDrawer : PropertyDrawer
    {
        private Object[] _choices;
        const float PICKER_WIDTH = 36f;

        Object[] GetChoices(bool allowNull, string[] paths, Type type)
        {

            if (paths != null)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    if (!paths[i].StartsWith("Assets/"))
                    {
                        paths[i] = "Assets/" + paths[i];
                    }
                }
            }

            string[] assetGUIDs = paths == null ? AssetDatabase.FindAssets("t:Prefab") : AssetDatabase.FindAssets("t:Prefab", paths);

            List<Object> prefabs = new List<Object>();
            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                GameObject go = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGUIDs[i])) as GameObject;
                if (prefabs.Count > 500) break;
                if (go == null) continue;
                if (type != null && go.GetComponent(type) == null) continue;

                prefabs.Add(go);
            }

            if (allowNull)
            {
                prefabs.Insert(0, null);
            }

            return prefabs.ToArray();
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(rect, label, property);

            if (_choices == null)
            {
                _choices = GetChoices((attribute as PrefabDropdownAttribute).AllowNull, (attribute as PrefabDropdownAttribute).Paths, (attribute as PrefabDropdownAttribute).Type);
            }

            GUIContent[] names = new GUIContent[_choices.Length];

            int selectedIndex = 0;
            for (int i = 0; i < _choices.Length; i++)
            {
                if (_choices[i] == null)
                {
                    names[i] = new GUIContent("NONE");
                }
                else
                {
                    names[i] = new GUIContent(_choices[i].name);
                }

                if (_choices[i] == property.objectReferenceValue)
                {
                    selectedIndex = i;
                }
            }

            if (_choices.Length == 0)
            {
                _choices = new[] { (Object)null };
                names = new[] { new GUIContent("NONE") };
            }

            EditorGUI.BeginChangeCheck();

            int newChoice = EditorGUI.Popup(rect.WithWidth(rect.width - PICKER_WIDTH), label, selectedIndex, names);

            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = _choices[newChoice];
            }

            EditorGUI.indentLevel = 0;

            EditorGUI.PropertyField(new Rect(rect.xMax - PICKER_WIDTH, rect.y, PICKER_WIDTH, rect.height), property, GUIContent.none);


            EditorGUI.EndProperty();



        }

    }
}