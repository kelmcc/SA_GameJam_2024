using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.WSA;
using Object = UnityEngine.Object;

namespace Framework
{

    public class AssetFolderWindow : EditorWindow, IHasCustomMenu
    {




        [SerializeField]
        AssetFilterWindow.AssetWindowTreeView.State _treeViewState;

        [SerializeField]
        protected string[] _searchPaths;

        [SerializeField]
        protected AssetFilterWindow.AssetType _assetType;

        [SerializeField]
        protected AssetFilterWindow.DisplayMode _displayMode;


        [SerializeField]
        private bool _includePlugins;

        private AssetFilterWindow.AssetWindowTreeView _treeView;
        private SearchField _searchField;
        private GUIStyle _searchStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _emptyButtonStyle;




        public static AssetFolderWindow CreateWindow(string title, AssetFilterWindow.AssetType assetType, AssetFilterWindow.DisplayMode displayMode, bool includePlugins, params string[] folderPaths)
        {
            AssetFolderWindow window = CreateWindow<AssetFolderWindow>(typeof(AssetFolderWindow));

            window._searchPaths = folderPaths;
            window._assetType = assetType;
            window._displayMode = displayMode;
            window._includePlugins = includePlugins;

            window.titleContent = new GUIContent(title);
            window.minSize = window.minSize.WithY(EditorGUIUtility.singleLineHeight);
            window.Show();

            window.CenterInScreen(400, 200);

            return window;
        }

        private void OnGUI()
        {
            if (_searchStyle == null)
            {
                _searchStyle = new GUIStyle(GUI.skin.FindStyle("ToolbarSeachTextField") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("ToolbarSeachTextField"));
                _buttonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("ToolbarSeachCancelButton");
                _emptyButtonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("ToolbarSeachCancelButtonEmpty");
            }

            if (_searchPaths == null)
            {
                _searchPaths = new string[0];
            }

            if (_treeViewState == null)
            {
                _treeViewState = new AssetFilterWindow.AssetWindowTreeView.State();
                _treeViewState.AssetType = _assetType;
                _treeViewState.DisplayMode = _displayMode;
                _treeViewState.IncludePlugins = _includePlugins;
                _treeViewState.SearchPaths = _searchPaths;
            }


            if (_treeView == null)
            {
                _treeView = new AssetFilterWindow.AssetWindowTreeView(_treeViewState);
            }

            if (_searchField == null)
            {
                _searchField = new SearchField();
                _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;

            }

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal("Toolbar");


            Rect rect = EditorGUILayout.GetControlRect(GUILayout.MinWidth(100));
            _searchStyle.fixedWidth = rect.width;

            _treeView.searchString = _searchField.OnGUI(rect, _treeView.searchString, _searchStyle, _buttonStyle, _emptyButtonStyle);

            EditorGUI.BeginChangeCheck();

            _displayMode = (AssetFilterWindow.DisplayMode)EditorGUILayout.EnumPopup(_displayMode, "ToolbarPopup", GUILayout.MinWidth(80), GUILayout.MaxWidth(90));

            if (EditorGUI.EndChangeCheck())
            {
                _treeViewState.AssetType = _assetType;
                _treeViewState.DisplayMode = _displayMode;
                _treeViewState.IncludePlugins = _includePlugins;
                _treeViewState.SearchPaths = _searchPaths;
                _treeView.Reload();
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh").WithTooltip("Refresh"), "toolbarbuttonRight", GUILayout.Width(30f)))
            {
                _treeView.Reload();
            }

            EditorGUILayout.EndHorizontal();



            _treeView.OnGUI(GUILayoutUtility.GetRect(10, position.width, 22, position.height));


            EditorGUILayout.BeginHorizontal("Toolbar");

            if (_treeView.HasSelection())
            {
                IList<int> selection = _treeView.GetSelection();
                if (selection.Count > 1)
                {
                    GUILayout.Label(selection.Count + " Assets Selected", GUILayout.MaxWidth(position.width));
                }
                else
                {
                    Object asset = _treeView.GetAsset(selection[0]);
                    string path = AssetDatabase.GetAssetPath(asset);

                    GUIStyle style = new GUIStyle("PrefixLabel");
                    style.margin = new RectOffset(4, 4, 0, 0);

                    if (GUILayout.Button(path.RemoveFromStart("Assets/"), style, GUILayout.MaxWidth(position.width)))
                    {
                        Selection.activeObject = null;
                        EditorUtility.FocusProjectWindow();
                        Selection.activeObject = asset;
                        EditorGUIUtility.PingObject(asset);

                        Event.current.Use();
                    }
                }

            }
            else
            {
                GUILayout.Label("");
            }



            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }


        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Refresh"), false, () => { _treeView = null; });
        }

        /*  public override IEnumerable<Type> GetExtraPaneTypes()
          {
              yield return typeof(AssetFilterWindow);
          }
        */
    }
}
