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

    public class AssetFilterWindow : EditorWindow, IHasCustomMenu
    {
        public enum AssetType
        {
            Prefab,
            ScriptableObject,
            Scene,
            Material,
            Texture,
            Model,
            Mesh,
            Script,
            AudioClip,
            Avatar,
            AnimationClip,
            AnimatorController,
            PhysicsMaterial,
        }

        public enum DisplayMode
        {
            Name,
            Path,
            Recent,
            RootFolder,
            Hierarchy
        }



        [MenuItem("Window/Asset Filter")]
        static AssetFilterWindow GetWindow()
        {
            AssetFilterWindow window = CreateWindow<AssetFilterWindow>(typeof(AssetFilterWindow));

            window.titleContent = new GUIContent("Asset Filter");
            window.minSize = window.minSize.WithY(EditorGUIUtility.singleLineHeight);
            window.Show();

            window.CenterInScreen(400, 200);

            return window;
        }


        [SerializeField]
        AssetWindowTreeView.State _treeViewState;

        [SerializeField]
        private AssetType _assetType;

        [SerializeField]
        private DisplayMode _displayMode;


        [SerializeField]
        private bool _includePlugins;

        [SerializeField]
        private bool _matchTitle;

        private AssetWindowTreeView _treeView;
        private SearchField _searchField;
        private GUIStyle _searchStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _emptyButtonStyle;


        static Type[] GetAssetTypes(AssetType type)
        {
            switch (type)
            {
                case AssetType.Prefab: return new[] { typeof(GameObject) };
                case AssetType.ScriptableObject: return new[] { typeof(ScriptableObject) };
                case AssetType.Scene: return new[] { typeof(SceneAsset) };
                case AssetType.Material: return new[] { typeof(Material) };
                case AssetType.Texture: return new[] { typeof(Texture) };
                case AssetType.Model: return new[] { typeof(GameObject) };
                case AssetType.Mesh: return new[] { typeof(Mesh) };
                case AssetType.Script: return new[] { typeof(MonoScript) };
                case AssetType.AudioClip: return new[] { typeof(AudioClip) };
                case AssetType.Avatar: return new[] { typeof(Avatar), typeof(AvatarMask) };
                case AssetType.AnimationClip: return new[] { typeof(AnimationClip) };
                case AssetType.AnimatorController: return new[] { typeof(AnimatorController), typeof(AnimatorOverrideController) };
                case AssetType.PhysicsMaterial: return new[] { typeof(PhysicMaterial), typeof(PhysicsMaterial2D) };

            }

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        static bool OnlyLoadMainAsset(AssetType type)
        {
            switch (type)
            {
                case AssetType.Prefab: return true;
                case AssetType.ScriptableObject: return true;
                case AssetType.Scene: return true;
                case AssetType.Material: return true;
                case AssetType.Texture: return true;
                case AssetType.Model: return true;
                case AssetType.Mesh: return false;
                case AssetType.Script: return true;
                case AssetType.AudioClip: return true;
                case AssetType.Avatar: return false;
                case AssetType.AnimationClip: return false;
                case AssetType.AnimatorController: return true;
                case AssetType.PhysicsMaterial: return true;

            }

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        static string GetWindowName(AssetType type)
        {
            return StringUtils.Titelize(type.ToString()) + "s";
        }

        void OnEnable()
        {
            titleContent = new GUIContent(_matchTitle ? GetWindowName(_assetType) : "Asset Filter");
        }


        private void OnGUI()
        {
            if (_searchStyle == null)
            {
                _searchStyle = new GUIStyle(GUI.skin.FindStyle("ToolbarSeachTextField") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("ToolbarSeachTextField"));
                _buttonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("ToolbarSeachCancelButton");
                _emptyButtonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("ToolbarSeachCancelButtonEmpty");
            }

            if (_treeViewState == null)
            {
                _treeViewState = new AssetWindowTreeView.State();
                _treeViewState.SearchPaths = new string[0];
                _treeViewState.AssetType = _assetType;
                _treeViewState.DisplayMode = _displayMode;
                _treeViewState.IncludePlugins = _includePlugins;

            }


            if (_treeView == null)
            {
                _treeView = new AssetWindowTreeView(_treeViewState);
            }

            if (_searchField == null)
            {
                _searchField = new SearchField();
                _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;

            }

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal("Toolbar");


            //  _treeView.searchString = _searchField.OnToolbarGUI(_treeView.searchString, GUILayout.MinWidth(100));

            Rect rect = EditorGUILayout.GetControlRect(GUILayout.MinWidth(100));
            _searchStyle.fixedWidth = rect.width;

            _treeView.searchString = _searchField.OnGUI(rect, _treeView.searchString, _searchStyle, _buttonStyle, _emptyButtonStyle);

            EditorGUI.BeginChangeCheck();

            _assetType = (AssetType)EditorGUILayout.EnumPopup(_assetType, "ToolbarPopup", GUILayout.MaxWidth(140));
            _displayMode = (DisplayMode)EditorGUILayout.EnumPopup(_displayMode, "ToolbarPopup", GUILayout.MinWidth(80), GUILayout.MaxWidth(90));

            _includePlugins = GUILayout.Toggle(_includePlugins, EditorGUIUtility.IconContent("d_Assembly Icon").WithTooltip("Include Plugins and Packages"), "ToolbarButton", GUILayout.MaxWidth(30));


            if (EditorGUI.EndChangeCheck())
            {
                _treeViewState.AssetType = _assetType;
                _treeViewState.DisplayMode = _displayMode;
                _treeViewState.IncludePlugins = _includePlugins;

                if (_matchTitle)
                {
                    titleContent = new GUIContent(GetWindowName(_assetType));
                }

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


        public class AssetWindowTreeView : TreeView
        {
            protected State ViewState => state as State;

            [Serializable]
            public class State : TreeViewState
            {
                public AssetType AssetType;
                public DisplayMode DisplayMode;
                public bool IncludePlugins;
                public string[] SearchPaths;
            }


            public AssetWindowTreeView(TreeViewState state) : base(state)
            {
                Reload();
            }

            public Object GetAsset(int id)
            {
                return (FindItem(id, rootItem) as AssetItem).Asset;
            }

            protected override bool CanRename(TreeViewItem item)
            {
                return true;
            }

            protected override void RenameEnded(RenameEndedArgs args)
            {
                base.RenameEnded(args);

                if (args.acceptedRename)
                {
                    AssetItem item = FindItem(args.itemID, rootItem) as AssetItem;
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item.Asset), args.newName);
                    item.displayName = args.newName;
                }
            }

            protected override bool CanStartDrag(CanStartDragArgs args)
            {
                return true;
            }

            protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
            {
                base.SetupDragAndDrop(args);

                DragAndDrop.PrepareStartDrag();

                string[] paths = new string[args.draggedItemIDs.Count];
                Object[] references = new Object[args.draggedItemIDs.Count];

                for (int i = 0; i < args.draggedItemIDs.Count; i++)
                {
                    AssetItem item = FindItem(args.draggedItemIDs[i], rootItem) as AssetItem;
                    paths[i] = AssetDatabase.GetAssetPath(item.Asset);
                    references[i] = item.Asset;
                }

                DragAndDrop.paths = paths;
                DragAndDrop.objectReferences = references;
                DragAndDrop.StartDrag("Dragging Assets");
            }

            protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
            {



                if (ViewState.DisplayMode != DisplayMode.Hierarchy) return DragAndDropVisualMode.None;
                if (args.dragAndDropPosition != DragAndDropPosition.UponItem) return DragAndDropVisualMode.None;
                if (DragAndDrop.paths.Length == 0) return DragAndDropVisualMode.None;

                DefaultAsset folder = (args.parentItem as AssetItem).Asset as DefaultAsset;

                if (folder == null) return DragAndDropVisualMode.None;

                if (args.performDrop)
                {
                    string folderPath = AssetDatabase.GetAssetPath(folder);
                    bool moved = false;

                    for (int i = 0; i < DragAndDrop.paths.Length; i++)
                    {
                        string path = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + FileUtils.GetFileName(DragAndDrop.paths[i], true));
                        if (AssetDatabase.ValidateMoveAsset(DragAndDrop.paths[i], path) == "")
                        {
                            AssetDatabase.MoveAsset(DragAndDrop.paths[i], path);
                            moved = true;
                        }
                    }

                    if (moved)
                    {
                        Reload();
                    }
                }

                return DragAndDropVisualMode.Move;

            }

            protected override void DoubleClickedItem(int id)
            {
                base.DoubleClickedItem(id);

                Object asset = (FindItem(id, rootItem) as AssetItem).Asset;
                Selection.activeObject = asset;
                AssetDatabase.OpenAsset(asset);
            }


            protected override void ContextClickedItem(int id)
            {
                base.ContextClickedItem(id);

                EditorUtility.DisplayPopupMenu(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), "Assets/", null);
            }

            protected override void KeyEvent()
            {
                base.KeyEvent();

                if (HasSelection() && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
                {

                    for (int i = 0; i < ViewState.selectedIDs.Count; i++)
                    {
                        Object asset = (FindItem(ViewState.selectedIDs[i], rootItem) as AssetItem).Asset;
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));

                    }

                    Reload();
                    Event.current.Use();
                }

                if (HasSelection() && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D && Event.current.modifiers == EventModifiers.Control)
                {

                    for (int i = 0; i < ViewState.selectedIDs.Count; i++)
                    {
                        Object asset = (FindItem(ViewState.selectedIDs[i], rootItem) as AssetItem).Asset;
                        string path = AssetDatabase.GetAssetPath(asset);
                        AssetDatabase.CopyAsset(path, AssetDatabase.GenerateUniqueAssetPath(path));
                    }

                    Reload();
                    Event.current.Use();
                }
            }

            protected override void SelectionChanged(IList<int> selectedIds)
            {
                base.SelectionChanged(selectedIds);

                Object[] assets = new Object[selectedIds.Count];

                for (int i = 0; i < assets.Length; i++)
                {
                    assets[i] = (FindItem(ViewState.selectedIDs[i], rootItem) as AssetItem).Asset;
                }

                Selection.objects = assets;
            }


            protected override TreeViewItem BuildRoot()
            {

                try
                {
                    double startTime = EditorApplication.timeSinceStartup;



                    AssetItem root = new AssetItem { id = 0, depth = -1, displayName = "Root" };
                    List<TreeViewItem> items = new List<TreeViewItem>();
                    Dictionary<string, TreeViewItem> foldersByPath = new Dictionary<string, TreeViewItem>();
                    List<TreeViewItem> allFolders = new List<TreeViewItem>();
                    HashSet<Object> addedAssets = new HashSet<Object>();
                    List<TreeViewItem> rootFolders = new List<TreeViewItem>();
                    List<TreeViewItem> rootItems = new List<TreeViewItem>();
                    Type[] types = GetAssetTypes(ViewState.AssetType);
                    bool onlyMainAsset = OnlyLoadMainAsset(ViewState.AssetType);

                    string commonSearchPathPrefix = "Assets/";

                    if (ViewState.SearchPaths.Length == 1)
                    {
                        commonSearchPathPrefix = ViewState.SearchPaths[0] + "/";
                    }
                    else if (ViewState.SearchPaths.Length > 1)
                    {
                        commonSearchPathPrefix = StringUtils.GetLongestCommonPrefix(ViewState.SearchPaths);
                    }

                    int itemID = 1;

                    bool FilterAsset(Object asset)
                    {
                        Type actualType = asset.GetType();

                        if (ViewState.AssetType == AssetType.Prefab)
                        {
                            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(asset);
                            if (prefabAssetType != PrefabAssetType.Regular && prefabAssetType != PrefabAssetType.Variant) return false;
                        }
                        else if (ViewState.AssetType == AssetType.Model)
                        {
                            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(asset);
                            if (prefabAssetType != PrefabAssetType.Model) return false;
                        }
                        else
                        {
                            for (int i = 0; i < types.Length; i++)
                            {
                                if (actualType == types[i]) return true;
                            }

                            for (int i = 0; i < types.Length; i++)
                            {
                                if (types[i].IsAssignableFrom(actualType)) return true;
                            }

                            return false;
                        }


                        return true;
                    }

                    TreeViewItem GetFolderItem(string folderPath)
                    {
                        TreeViewItem folder = null;
                        bool isRoot = !folderPath.Contains('/');


                        if (ViewState.DisplayMode == DisplayMode.RootFolder && !isRoot)
                        {
                            isRoot = true;
                            folderPath = folderPath.Substring(0, folderPath.IndexOf('/'));
                        }

                        if (!foldersByPath.TryGetValue(folderPath, out folder))
                        {
                            folder = new AssetItem()
                            {
                                id = itemID,
                                depth = 0,
                                displayName = isRoot ? folderPath : folderPath.Substring(folderPath.LastIndexOf('/') + 1),
                                Asset = AssetDatabase.LoadMainAssetAtPath(commonSearchPathPrefix + folderPath),
                                icon = (Texture2D)AssetDatabase.GetCachedIcon(commonSearchPathPrefix + folderPath),
                                DateModified = DateTime.MinValue
                            };

                            itemID++;


                            foldersByPath.Add(folderPath, folder);
                            allFolders.Add(folder);

                            if (isRoot)
                            {
                                rootFolders.Add(folder);
                            }
                            else
                            {
                                string parentPath = folderPath.Substring(0, folderPath.LastIndexOf("/"));

                                TreeViewItem parent = GetFolderItem(parentPath);
                                parent.AddChild(folder);
                            }

                        }


                        return folder;
                    }

                    void AddItem(Object asset, string path)
                    {
                        if (asset == null) return;

                        if (!ViewState.IncludePlugins)
                        {
                            if (path.Contains("/Plugins/")) return;
                            if (path.StartsWith("Packages/")) return;
                        }

                        if (addedAssets.Contains(asset)) return;
                        if (!FilterAsset(asset)) return;

                        AssetItem newItem = new AssetItem
                        {
                            id = itemID,
                            depth = 0,
                            displayName = ViewState.DisplayMode == DisplayMode.Path ? path.RemoveFromStart("Assets/") : asset.name,
                            Asset = asset,
                            icon = (Texture2D)(onlyMainAsset || AssetDatabase.IsMainAsset(asset) ? AssetDatabase.GetCachedIcon(path) : EditorGUIUtility.ObjectContent(asset, asset.GetType()).image),
                            DateModified = ViewState.DisplayMode == DisplayMode.Recent ? FileUtils.GetLastModifiedTimeIncludingMetaFile(path) : DateTime.MinValue
                        };


                        itemID++;

                        items.Add(newItem);
                        addedAssets.Add(asset);


                        if (ViewState.DisplayMode == DisplayMode.RootFolder || ViewState.DisplayMode == DisplayMode.Hierarchy)
                        {
                            if (path.LastIndexOf('/') == 6)
                            {
                                rootItems.Add(newItem);
                            }
                            else
                            {
                                string folderPath = path.RemoveFromStart(commonSearchPathPrefix);

                                if (folderPath.Length == 0 || folderPath.IndexOf('/') < 0)
                                {

                                    rootItems.Add(newItem);
                                }
                                else
                                {
                                    folderPath = folderPath.Substring(0, folderPath.LastIndexOf('/'));
                                    GetFolderItem(folderPath).AddChild(newItem);
                                }
                            }
                        }
                    }

                    List<string> guids = new List<string>();
                    for (int i = 0; i < types.Length; i++)
                    {
                        if (ViewState.SearchPaths.Length > 0)
                        {
                            guids.AddRange(AssetDatabase.FindAssets("t:" + types[i].Name, ViewState.SearchPaths));
                        }
                        else
                        {
                            guids.AddRange(AssetDatabase.FindAssets("t:" + types[i].Name));
                        }
                    }

                    for (int i = 0; i < guids.Count; i++)
                    {
                        if (EditorApplication.timeSinceStartup - startTime > 1f)
                        {
                            EditorUtility.DisplayProgressBar("Asset Filter Window", "Processing Assets (" + i + "/" + guids.Count + ")", (float)i / guids.Count);
                        }

                        string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);

                        if (onlyMainAsset)
                        {
                            AddItem(AssetDatabase.LoadMainAssetAtPath(assetPath), assetPath);
                        }
                        else
                        {
                            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                            for (int j = 0; j < assets.Length; j++)
                            {
                                AddItem(assets[j], assetPath);
                            }
                        }



                    }


                    if (ViewState.DisplayMode == DisplayMode.Recent)
                    {
                        items.Sort((a, b) => ((AssetItem)b).DateModified.CompareTo(((AssetItem)a).DateModified));
                    }

                    if (ViewState.DisplayMode == DisplayMode.RootFolder || ViewState.DisplayMode == DisplayMode.Hierarchy)
                    {
                        rootFolders.AddRange(rootItems);
                        root.children = rootFolders;
                    }
                    else
                    {
                        root.children = items;
                    }

                    if (ViewState.DisplayMode == DisplayMode.Hierarchy)
                    {

                        for (int i = 0; i < allFolders.Count; i++)
                        {


                            allFolders[i].children.Sort((a, b) =>
                            {
                                bool aIsFolder = ((AssetItem)a).Asset is DefaultAsset;
                                bool bIsFolder = ((AssetItem)b).Asset is DefaultAsset;

                                if (aIsFolder && !bIsFolder) return -1;
                                if (bIsFolder && !aIsFolder) return 1;

                                return EditorUtility.NaturalCompare(a.displayName, b.displayName);
                            });
                        }
                    }

                    SetSelection(new List<int>());
                    SetupDepthsFromParentsAndChildren(root);

                    return root;

                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }

        }

        public class AssetItem : TreeViewItem
        {
            public Object Asset;
            public DateTime DateModified;
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Refresh"), false, () => { _treeView = null; });


            menu.AddItem(new GUIContent("Match Title To Asset Type"), _matchTitle, () =>
            {
                _matchTitle = !_matchTitle;
                titleContent = new GUIContent(_matchTitle ? GetWindowName(_assetType) : "Asset Filter");
            });

        }

        public override IEnumerable<Type> GetExtraPaneTypes()
        {
            yield return typeof(AssetFilterWindow);
        }
    }
}