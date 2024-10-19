using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class SelectionRenameWindow : EditorWindow
    {

        private const float WIDTH = 360;
        private const float HEIGHT = 100;

        [SerializeField]
        private string _baseName;

        [SerializeField]
        private string _prefix;

        [SerializeField]
        private string _suffix;


        [MenuItem("GameObject/Selection/Rename", false, 0)]
        static void ShowWindow()
        {
            SelectionRenameWindow window = GetWindow<SelectionRenameWindow>(true, "Rename Selection", true);

            window.ShowUtility();
            window.CenterInScreen(360, 100);
        }

        void OnGUI()
        {

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(Selection.transforms.Length < 1);

            RenameButton("Rename", ref _baseName, o => _baseName);
            RenameButton("Add prefix", ref _prefix, o => _prefix + o.name);
            RenameButton("Add suffix", ref _suffix, o => o.name + _suffix);


            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add Number", GUILayout.Width(100)))
            {
                GameObject[] selection = GetOrderedSelection();
                Undo.RecordObjects(selection, "Rename Selection");

                for (int i = 0; i < selection.Length; i++)
                {
                    selection[i].name = selection[i].name + " (" + (i + 1) + ")";
                }
            }

            EditorGUILayout.EndHorizontal();


            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

        }

        void RenameButton(string label, ref string text, Func<GameObject, string> renameFunction)
        {
            EditorGUILayout.BeginHorizontal();

            text = EditorGUILayout.TextField(text);

            if (GUILayout.Button(label, GUILayout.Width(100)))
            {
                Undo.RecordObjects(Selection.gameObjects, "Rename Selection");

                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    Selection.gameObjects[i].name = renameFunction(Selection.gameObjects[i]);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        GameObject[] GetOrderedSelection()
        {
            List<Transform> transforms = new List<Transform>(Selection.transforms);
            transforms.Sort((x, y) => x.GetSiblingIndex().CompareTo(y.GetSiblingIndex()));
            return transforms.Select(t => t.gameObject).ToArray();
        }

    }
}
