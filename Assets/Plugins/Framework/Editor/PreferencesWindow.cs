using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{

    public class PreferencesWindow : EditorWindow
    {
        [SerializeField]
        private string _settingsTypeName;

        private Type _settingsType;
        private Vector2 _scrollPosition;

        public static PreferencesWindow OpenCustomPreferencesWindow(string title, Type settingsType)
        {
            PreferencesWindow window = EditorWindow.CreateWindow<PreferencesWindow>();
            window.titleContent = new GUIContent(title);
            window.minSize = window.minSize.WithY(EditorGUIUtility.singleLineHeight);
            window.Setup(settingsType);
            window.Show();

            window.CenterInScreen(400, 200);

            return window;
        }

        public void Setup(Type settingsType)
        {
            _settingsTypeName = settingsType.AssemblyQualifiedName;
            _settingsType = settingsType;
        }

        private void OnGUI()
        {
            if (_settingsType == null)
            {
                _settingsType = Type.GetType(_settingsTypeName);
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            EditorPrefsUtils.DrawPrefencesGUI(_settingsType);
            EditorGUILayout.EndScrollView();
        }


    }
}