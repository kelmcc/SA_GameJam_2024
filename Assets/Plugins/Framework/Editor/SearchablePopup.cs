using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEditor;
using UnityEngine;

namespace Framework
{

    // Adapted from https://github.com/roboryantron/UnityEditorJunkie
    public class SearchablePopup : PopupWindowContent
    {

        private const float ROW_HEIGHT = 16f;
        private const float ROW_INDENT = 8f;
        private const int SHOW_SEARCHBAR_THRESHOLD = 20;

        public static void ShowPopup(Rect buttonRect, string[] options, int current, Action<int> onSelectionMade)
        {

            PopupWindow.Show(buttonRect, new SearchablePopup(buttonRect, options, Mathf.Clamp(current, 0, options.Length - 1), onSelectionMade));

        }

        public static void DrawButton(Rect rect, GUIContent label, string[] options, int current, Action<int> onOptionSelected, bool showSearchBarForFewItems = false)
        {
            current = Mathf.Clamp(current, 0, options.Length - 1);

            if (options.Length <= SHOW_SEARCHBAR_THRESHOLD && !showSearchBarForFewItems)
            {
                GUIContent[] labels = new GUIContent[options.Length];
                for (int i = 0; i < labels.Length; i++)
                {
                    labels[i] = new GUIContent(options[i]);
                }

                EditorGUI.BeginChangeCheck();
                int selection = EditorGUI.Popup(rect, label, current, labels);
                if (EditorGUI.EndChangeCheck())
                {
                    onOptionSelected(selection);
                }
            }
            else
            {
                rect = EditorGUI.PrefixLabel(rect, label);

                if (GUI.Button(rect, options[current], EditorStyles.popup))
                {
                    ShowPopup(rect, options, current, onOptionSelected);
                }
            }
        }



        private class FilteredList
        {

            public struct Entry
            {
                public int Index;
                public string Text;
            }

            public List<Entry> Entries => _entries;
            public string Filter => _filter;
            public int MaxLength => _allItems.Length;

            private string[] _allItems;
            private List<Entry> _entries;
            private string _filter;

            public FilteredList(string[] items)
            {
                _allItems = items;
                _entries = new List<Entry>();
                UpdateFilter("");
            }

            public bool UpdateFilter(string filter)
            {
                if (_filter == filter) return false;

                _filter = filter;
                _entries.Clear();

                for (int i = 0; i < _allItems.Length; i++)
                {
                    if (string.IsNullOrEmpty(_filter) || _allItems[i].ToLower().Contains(_filter.ToLower()))
                    {
                        Entry entry = new Entry { Index = i, Text = _allItems[i] };
                        if (string.Equals(_allItems[i], _filter, StringComparison.CurrentCultureIgnoreCase))
                        {
                            _entries.Insert(0, entry);
                        }
                        else
                        {
                            _entries.Add(entry);
                        }
                    }
                }

                return true;
            }
        }


        private Action<int> _onSelectionMade;
        private int _currentIndex;
        private FilteredList _list;
        private Vector2 _scrollPosition;
        private int _hoverIndex;
        private int _scrollToIndex;
        private float _scrollOffset;
        private Rect _buttonRect;

        private static GUIStyle _searchBoxStyle = "ToolbarSeachTextField";
        private static GUIStyle _cancelButtonStyle = "ToolbarSeachCancelButton";
        private static GUIStyle _disabledCancelButtonStyle = "ToolbarSeachCancelButtonEmpty";
        private static GUIStyle _selectionStyle = "SelectionRect";

        private SearchablePopup(Rect buttonRect, string[] names, int currentIndex, Action<int> onSelectionMade)
        {
            _list = new FilteredList(names);
            _currentIndex = currentIndex;
            _onSelectionMade = onSelectionMade;

            _hoverIndex = currentIndex;
            _scrollToIndex = currentIndex;
            _buttonRect = buttonRect;
            _scrollOffset = GetWindowSize().y - ROW_HEIGHT * 2;

        }

        static void Repaint()
        {
            EditorWindow.focusedWindow.Repaint();
        }

        public override void OnOpen()
        {
            base.OnOpen();

            EditorApplication.update += Repaint;
        }

        public override void OnClose()
        {
            base.OnClose();
            EditorApplication.update -= Repaint;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(Mathf.Max(base.GetWindowSize().x, _buttonRect.width), Mathf.Min(600, _list.MaxLength * ROW_HEIGHT + EditorStyles.toolbar.fixedHeight));
        }

        public override void OnGUI(Rect rect)
        {
            Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
            Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

            HandleKeyboard();
            DrawSearch(searchRect);
            DrawSelectionArea(scrollRect);
        }

        private void DrawSearch(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.toolbar.Draw(rect, false, false, false, false);
            }

            Rect searchRect = new Rect(rect);
            searchRect.xMin += 6;
            searchRect.xMax -= 6;
            searchRect.y += 2;
            searchRect.width -= _cancelButtonStyle.fixedWidth;

            GUI.FocusControl("SearchField");
            GUI.SetNextControlName("SearchField");
            string newText = GUI.TextField(searchRect, _list.Filter, _searchBoxStyle);

            if (_list.UpdateFilter(newText))
            {
                _hoverIndex = 0;
                _scrollPosition = Vector2.zero;
            }

            searchRect.x = searchRect.xMax;
            searchRect.width = _cancelButtonStyle.fixedWidth;

            if (string.IsNullOrEmpty(_list.Filter))
            {
                GUI.Box(searchRect, GUIContent.none, _disabledCancelButtonStyle);
            }
            else if (GUI.Button(searchRect, "x", _cancelButtonStyle))
            {
                _list.UpdateFilter("");
                _scrollPosition = Vector2.zero;
            }
        }

        private void DrawSelectionArea(Rect scrollRect)
        {
            Rect contentRect = new Rect(0, 0,
                scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth,
                _list.Entries.Count * ROW_HEIGHT);

            _scrollPosition = GUI.BeginScrollView(scrollRect, _scrollPosition, contentRect);

            Rect rowRect = new Rect(0, 0, scrollRect.width, ROW_HEIGHT);

            for (int i = 0; i < _list.Entries.Count; i++)
            {
                if (_scrollToIndex == i && (Event.current.type == EventType.Repaint || Event.current.type == EventType.Layout))
                {

                    GUI.ScrollTo(new Rect(rowRect).WithY(rowRect.y + _scrollOffset));

                    _scrollToIndex = -1;
                    _scrollPosition.x = 0;
                }

                if (rowRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.ScrollWheel)
                    {
                        _hoverIndex = i;
                    }

                    if (Event.current.type == EventType.MouseDown)
                    {
                        _onSelectionMade(_list.Entries[i].Index);
                        EditorWindow.focusedWindow.Close();
                    }
                }

                DrawRow(rowRect, i);

                rowRect.y = rowRect.yMax;
            }

            GUI.EndScrollView();
        }

        private void DrawRow(Rect rowRect, int i)
        {
            Color oldColour = GUI.color;

            if (_list.Entries[i].Index == _currentIndex)
            {
                GUI.color = Color.cyan;
                GUI.Box(rowRect, "", _selectionStyle);
            }
            else if (i == _hoverIndex)
            {
                GUI.color = Color.white;
                GUI.Box(rowRect, "", _selectionStyle);
            }

            GUI.color = oldColour;


            Rect labelRect = new Rect(rowRect);
            labelRect.xMin += ROW_INDENT;

            GUI.Label(labelRect, _list.Entries[i].Text);
        }

        private void HandleKeyboard()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    _hoverIndex = Mathf.Min(_list.Entries.Count - 1, _hoverIndex + 1);
                    Event.current.Use();
                    _scrollToIndex = _hoverIndex;
                    _scrollOffset = ROW_HEIGHT;
                }

                if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    _hoverIndex = Mathf.Max(0, _hoverIndex - 1);
                    Event.current.Use();
                    _scrollToIndex = _hoverIndex;
                    _scrollOffset = -ROW_HEIGHT;
                }

                if (Event.current.keyCode == KeyCode.Return)
                {
                    if (_hoverIndex >= 0 && _hoverIndex < _list.Entries.Count)
                    {
                        _onSelectionMade(_list.Entries[_hoverIndex].Index);
                        EditorWindow.focusedWindow.Close();
                    }
                }

                if (Event.current.keyCode == KeyCode.Escape)
                {
                    EditorWindow.focusedWindow.Close();
                }
            }
        }
    }
}