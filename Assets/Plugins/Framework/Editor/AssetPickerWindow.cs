using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{

    public class AssetPickerWindow : EditorWindow
    {
        private Object[] _rawAssets;
        private string _searchString;
        private Vector2 _scrollPosition;

        private SearchField _searchField;
        private Action<Object> _onAssetSelected;
        private Object _selectedAsset;

        public static AssetPickerWindow Show(string titleText, Object[] assets, Object selectedAsset, Action<Object> onAssetSelected)
        {
            AssetPickerWindow window = GetWindow<AssetPickerWindow>(true);
            if (window == null)
            {
                window = CreateInstance<AssetPickerWindow>();
            }

            window._rawAssets = assets;
            window._onAssetSelected = onAssetSelected;
            window._selectedAsset = selectedAsset;

            window.titleContent = new GUIContent(titleText);
            window.minSize = new Vector2(100, 100);
            window.ShowAuxWindow();

            return window;
        }



        List<Object> GetFilteredAssets()
        {

            bool search = !(string.IsNullOrEmpty(_searchString) || string.IsNullOrEmpty(_searchString.Trim()));

            List<Object> results = new List<Object>();

            if (_rawAssets != null)
            {
                for (int i = 0; i < _rawAssets.Length; i++)
                {
                    if (_rawAssets[i] == null) continue;


                    if (!search || _rawAssets[i].name.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        results.Add(_rawAssets[i]);
                    }
                }
            }

            return results;
        }

        void OnGUI()
        {
            const float MARGIN = 3f;

            GUILayout.BeginHorizontal();
            GUILayout.Space(MARGIN);

            Rect searchRect = GUILayoutUtility.GetRect(100f, 100f, 22f, 22f);
            searchRect.y += MARGIN;

            if (_searchField == null) _searchField = new SearchField();
            _searchString = _searchField.OnToolbarGUI(searchRect, _searchString);

            float previewScale = EditorPrefsUtils.GetFloat("AssetPickerWindow.PreviewScale", 0.5f);

            EditorGUI.BeginChangeCheck();

            previewScale = GUILayout.HorizontalSlider(previewScale, 0f, 1f, GUILayout.Width(100f));
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefsUtils.SetFloat("AssetPickerWindow.PreviewScale", previewScale);
            }

            GUILayout.Space(MARGIN);
            GUILayout.EndHorizontal();

            GUILayout.Space(MARGIN);


            float iconWidth = Mathf.Lerp(30f, 140f, previewScale);
            int columns = Mathf.FloorToInt((position.width + 5f) / (iconWidth + 5f));


            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);


            bool insideRow = false;
            bool close = false;
            int column = 0;

            List<Object> assets = GetFilteredAssets();

            EditorUtils.EnsureAssetPreviewCacheSize(assets.Count);

            for (int i = 0; i < assets.Count; i++)
            {
                if (column == 0)
                {
                    GUILayout.BeginHorizontal();
                    insideRow = true;
                }

                column++;

                GUI.backgroundColor = _selectedAsset == assets[i] ? Color.green : Color.white;
                GUIContent content = new GUIContent(AssetPreview.GetAssetPreview(assets[i]), assets[i].name);

                if (GUILayout.Button(content, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth)) && _onAssetSelected != null)
                {
                    _onAssetSelected(assets[i]);
                    close = true;
                }

                if (column == columns)
                {
                    column = 0;
                    GUILayout.EndHorizontal();
                    insideRow = false;
                }
            }

            if (insideRow)
            {
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUI.backgroundColor = Color.white;

            if (close)
            {
                Close();
            }
        }

    }
}