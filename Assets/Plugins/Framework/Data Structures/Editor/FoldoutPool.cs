using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class FoldoutPool
    {
        [SerializeField] private Dictionary<string, bool> _values = new Dictionary<string, bool>();
        [SerializeField] private bool _defaultValue;

        public FoldoutPool(bool defaultValue = false)
        {
            _defaultValue = defaultValue;
        }

        public bool Foldout(Rect rect, string name)
        {
            bool value = EditorGUI.Foldout(rect, IsExpanded(name), name);
            SetExpanded(name, value);

            return value;
        }

        public bool Foldout(Rect rect, string name, GUIStyle style)
        {
            bool value = EditorGUI.Foldout(rect, IsExpanded(name), name, style);
            SetExpanded(name, value);

            return value;
        }

        public bool Foldout(Rect rect, string name, bool toggleOnLabelClick)
        {
            bool value = EditorGUI.Foldout(rect, IsExpanded(name), name, toggleOnLabelClick);
            SetExpanded(name, value);

            return value;
        }

        public bool Foldout(Rect rect, string name, bool toggleOnLabelClick, GUIStyle style)
        {
            bool value = EditorGUI.Foldout(rect, IsExpanded(name), name, toggleOnLabelClick, style);
            SetExpanded(name, value);

            return value;
        }

        public bool Foldout(string name)
        {
            bool value = EditorGUILayout.Foldout(IsExpanded(name), name);
            SetExpanded(name, value);

            return value;
        }

        public bool Foldout(string name, GUIStyle style)
        {
            bool value = EditorGUILayout.Foldout(IsExpanded(name), name, style);
            SetExpanded(name, value);

            return value;
        }

        public bool Foldout(string name, bool toggleOnLabelClick)
        {
            bool value = EditorGUILayout.Foldout(IsExpanded(name), name, toggleOnLabelClick);
            SetExpanded(name, value);

            return value;
        }

        public bool Foldout(string name, bool toggleOnLabelClick, GUIStyle style)
        {
            bool value = EditorGUILayout.Foldout(IsExpanded(name), name, toggleOnLabelClick, style);
            SetExpanded(name, value);

            return value;
        }



        public void SetExpanded(string name, bool expanded)
        {
            _values[name] = expanded;
        }

        public bool IsExpanded(string name)
        {
            if (!_values.TryGetValue(name, out bool value))
            {
                _values.Add(name, _defaultValue);
                return _defaultValue;
            }

            return value;
        }
    }
}


