using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework
{
    public class HistoryWindow : EditorWindow, IHasCustomMenu
    {

        [Serializable]
        public class HistoryStack
        {
            public int Count => _count;
            public int Capacity => _elements.Length;

            public Item this[int index] => _elements[MathUtils.Wrap(_startIndex - index, 0, _count - 1)];

            [SerializeField] private Item[] _elements;
            [SerializeField] private int _count;
            [SerializeField] private int _startIndex;

            public HistoryStack(int capacity)
            {
                _elements = new Item[capacity];
            }

            public void Push(Item item)
            {
                if (_count < _elements.Length)
                {
                    _elements[_count] = item;
                    _startIndex = _count;
                    _count++;
                }
                else
                {
                    _startIndex = MathUtils.Wrap(_startIndex + 1, 0, _elements.Length - 1);
                    _elements[_startIndex] = item;
                }
            }

            public void RemoveInvalidItems()
            {
                List<Item> validItems = new List<Item>();
                for (int i = 0; i < _count; i++)
                {
                    if (this[i].IsValid)
                    {
                        validItems.Add(this[i]);
                    }
                }

                SetItems(validItems);
            }

            public bool MoveToTop(Item item)
            {
                int itemPosition = -1;

                for (int i = 0; i < _count; i++)
                {
                    if (this[i].Equals(item))
                    {
                        itemPosition = i;
                        break;
                    }
                }

                if (itemPosition >= 0)
                {
                    List<Item> newItems = new List<Item>();
                    newItems.Add(this[itemPosition]);

                    for (int i = 0; i < _count; i++)
                    {
                        if (i != itemPosition)
                        {
                            newItems.Add(this[i]);
                        }
                    }

                    SetItems(newItems);

                    return true;
                }

                return false;
            }

            void SetItems(List<Item> items)
            {
                _count = 0;
                _startIndex = 0;

                for (int i = items.Count - 1; i >= 0; i--)
                {
                    Push(items[i]);
                }
            }

            public void Clear()
            {
                _elements = new Item[_elements.Length];
                _count = 0;
                _startIndex = 0;
            }
        }

        [Serializable]
        public class Item : IEquatable<Item>
        {
            public enum ItemType
            {
                Scene,
                Asset,
                HierachyObject,
                Prefab
            }

            public string Name => GetObject().name;
            public bool IsValid => GetObject() != null;

            [SerializeField] private string _assetPath;
            [SerializeField] private int _instanceID;
            [SerializeField] private ItemType _type;

            private Object _object;


            public Item(Object obj)
            {

                if (obj is GameObject go)
                {
                    if (!go.scene.IsValid())
                    {
                        _object = obj;
                        _assetPath = AssetDatabase.GetAssetPath(obj);
                        _type = ItemType.Prefab;
                    }
                    else
                    {
                        PrefabStage stage = PrefabStageUtility.GetPrefabStage(go);
                        if (stage != null && stage.prefabContentsRoot == go)
                        {
                            _object = stage.prefabContentsRoot;
                            _assetPath = stage.assetPath;
                            _type = ItemType.Prefab;
                        }
                        else
                        {
                            _object = obj;
                            _assetPath = go.scene.path;
                            _instanceID = go.GetInstanceID();
                            _type = ItemType.HierachyObject;
                        }
                    }
                }
                else
                {
                    _object = obj;
                    _assetPath = AssetDatabase.GetAssetPath(obj);
                    _type = ItemType.Asset;
                }
            }

            public Item(Scene scene)
            {
                _assetPath = scene.path;
                _type = ItemType.Scene;
            }


            public Object GetObject()
            {
                if (_object == null)
                {
                    if (_type == ItemType.HierachyObject)
                    {
                        _object = EditorUtility.InstanceIDToObject(_instanceID);
                    }
                    else
                    {
                        _object = AssetDatabase.LoadMainAssetAtPath(_assetPath);
                    }
                }

                return _object;
            }

            public void Open()
            {
                if (_type == ItemType.Scene)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(_assetPath, OpenSceneMode.Single);
                    }
                }
                else if (_type == ItemType.Prefab)
                {
                    AssetDatabase.OpenAsset(GetObject());
                }
                else
                {
                    Selection.activeObject = GetObject();
                }
            }

            public bool IsOpened(bool selectionMode)
            {
                if (!IsValid) return false;
                if (_type == ItemType.Scene) return SceneManager.GetActiveScene().path == _assetPath;

                if (_type == ItemType.Prefab)
                {
                    PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();

                    if (stage == null) return Selection.activeObject == GetObject();
                    if (stage.assetPath != _assetPath) return false;

                    return !selectionMode || Selection.activeGameObject == stage.prefabContentsRoot;
                }

                return Selection.activeObject == GetObject();
            }


            public bool Equals(Item other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return _assetPath == other._assetPath && _instanceID == other._instanceID && _type == other._type;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;

                return Equals((Item)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (_assetPath != null ? _assetPath.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ _instanceID;
                    hashCode = (hashCode * 397) ^ (int)_type;
                    hashCode = (hashCode * 397) ^ (_object != null ? _object.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        private class FavouritesContainer : ScriptableObject
        {
            public List<Object> assets;
            public List<DefaultAsset> folders;
            public List<MonoScript> sceneObjects;
        }

        [SerializeField] private HistoryStack _selectionHistory;
        [SerializeField] private HistoryStack _prefabHistory;
        [SerializeField] private HistoryStack _sceneHistory;

        private GUIContent _forwardIcon;
        private GUIContent _backIcon;
        private GUIContent _dropdownIcon;
        private GUILayoutOption[] _dropdownOptions;
        private GUILayoutOption[] _buttonOptions;
        private Object _scrubObject;
        private static FavouritesContainer _favouritesContainer;

        [MenuItem("Window/History")]
        public static HistoryWindow GetWindow()
        {
            HistoryWindow window = GetWindow<HistoryWindow>();

            window.titleContent = new GUIContent("History");
            window.minSize = window.minSize.WithY(EditorGUIUtility.singleLineHeight);
            window.CenterInScreen(300, EditorGUIUtility.singleLineHeight + 5f);

            window.Focus();
            window.Repaint();

            return window;
        }

        private void OnEnable()
        {
            EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;

            _forwardIcon = EditorGUIUtility.IconContent("d_forward"); // d_forward d_forward@2x"
            _backIcon = EditorGUIUtility.IconContent("d_back"); // d_back d_back@2x
            _dropdownIcon = EditorGUIUtility.IconContent("icon dropdown");

            _buttonOptions = new[] { GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.MinWidth(16f), GUILayout.MaxWidth(40) };
            _dropdownOptions = new[] { GUILayout.Height(EditorGUIUtility.singleLineHeight) };

            if (_selectionHistory == null) _selectionHistory = new HistoryStack(40);
            if (_prefabHistory == null) _prefabHistory = new HistoryStack(20);
            if (_sceneHistory == null) _sceneHistory = new HistoryStack(15);

            RecordHistory(new Item(SceneManager.GetActiveScene()), _sceneHistory);
        }


        private void OnDisable()
        {

            EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChanged;
            PrefabStage.prefabStageOpened -= OnPrefabStageOpened;
        }

        private void OnGUI()
        {

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(_backIcon, _buttonOptions))
                {
                    ScrubSelectionHistory(false);
                }
                if (GUILayout.Button(_forwardIcon, _buttonOptions))
                {
                    ScrubSelectionHistory(true);
                }

                Rect dropdownRect = EditorGUILayout.GetControlRect(false, _buttonOptions);

                if (GUI.Button(dropdownRect, _dropdownIcon))
                {
                    ShowDropdown(dropdownRect, _selectionHistory, true);
                }


                Rect prefabsRect = EditorGUILayout.GetControlRect(false, _dropdownOptions);

                if (EditorGUI.DropdownButton(prefabsRect, new GUIContent("Prefabs"), FocusType.Passive))
                {
                    ShowDropdown(prefabsRect, _prefabHistory, false);
                }

                Rect scenesRect = EditorGUILayout.GetControlRect(false, _dropdownOptions);

                if (EditorGUI.DropdownButton(scenesRect, new GUIContent("Scenes"), FocusType.Passive))
                {
                    ShowDropdown(scenesRect, _sceneHistory, false);
                }

                Rect favouritesRect = EditorGUILayout.GetControlRect(false, _dropdownOptions);

                if (EditorGUI.DropdownButton(favouritesRect, new GUIContent("Favourites"), FocusType.Passive))
                {
                    ShowFavouritesDropdown(favouritesRect);
                }

            }
        }

        private void OnPrefabStageOpened(PrefabStage stage)
        {
            RecordHistory(new Item(stage.prefabContentsRoot), _prefabHistory);
        }

        private void OnSelectionChange()
        {

            if (Selection.objects.Length == 1 && Selection.activeObject != null && Selection.activeObject != _scrubObject)
            {
                Object selectedObject = Selection.activeObject;

                /*if (selectedObject is GameObject go)
            {
                if (go.IsPrefabAsset() && PrefabStageUtility.GetCurrentPrefabStage() == null)
                {

                    RecordHistory(new Item(go), _prefabHistory);
                }
            }
            */

                Type type = selectedObject.GetType();
                if (type == typeof(SceneAsset)) return;
                if (type == typeof(DefaultAsset) && Directory.Exists(AssetDatabase.GetAssetPath(selectedObject))) return;

                RecordHistory(new Item(selectedObject), _selectionHistory);
            }

            _scrubObject = null;
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            RecordHistory(new Item(newScene), _sceneHistory);
        }

        void RecordHistory(Item item, HistoryStack history)
        {
            if (!history.MoveToTop(item))
            {
                history.Push(item);
            }
        }

        void ScrubSelectionHistory(bool forward)
        {
            if (_selectionHistory.Count == 0) return;

            int currentIndex = -1;

            for (int i = 0; i < _selectionHistory.Count; i++)
            {
                if (_selectionHistory[i].GetObject() == Selection.activeObject)
                {
                    currentIndex = i;
                }
            }

            if (currentIndex >= 0)
            {
                if (forward && currentIndex > 0)
                {
                    _scrubObject = _selectionHistory[currentIndex - 1].GetObject();
                    Selection.activeObject = _scrubObject;
                }

                if (!forward && currentIndex < _selectionHistory.Count - 1)
                {
                    _scrubObject = _selectionHistory[currentIndex + 1].GetObject();
                    Selection.activeObject = _scrubObject;
                }
            }
            else if (!forward)
            {
                _scrubObject = _selectionHistory[0].GetObject();
                Selection.activeObject = _scrubObject;
            }


        }

        void ShowDropdown(Rect rect, HistoryStack history, bool isSelectionDropdown)
        {
            GenericMenu menu = new GenericMenu();

            history.RemoveInvalidItems();

            if (history.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No History"));
            }
            else
            {
                for (int i = 0; i < history.Count; i++)
                {
                    AddDropdownChoice(menu, history[i].IsOpened(isSelectionDropdown), history[i].Name, OpenHistoryItem, history[i], history);
                }
            }

            menu.DropDown(rect);
        }

        void ShowFavouritesDropdown(Rect rect)
        {
            GenericMenu menu = new GenericMenu();

            LoadFavourites();

            List<Object> assets = _favouritesContainer.assets;
            List<DefaultAsset> folders = _favouritesContainer.folders;
            List<MonoScript> scripts = _favouritesContainer.sceneObjects;

            if (_favouritesContainer.assets.Count == 0 && _favouritesContainer.folders.Count == 0 && _favouritesContainer.sceneObjects.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Favourites"));
            }
            else
            {
                int totalAdded = 0;
                int addedInSection = 0;

                for (int i = 0; i < assets.Count; i++)
                {
                    if (assets[i] != null)
                    {
                        menu.AddItem(new GUIContent(assets[i].name), false, (asset) => AssetDatabase.OpenAsset((Object)asset), assets[i]);
                        totalAdded++;
                        addedInSection++;
                    }
                }

                addedInSection = 0;

                for (int i = 0; i < folders.Count; i++)
                {
                    if (folders[i] != null)
                    {
                        if (addedInSection == 0 && totalAdded != 0)
                        {
                            menu.AddSeparator("");
                        }

                        menu.AddItem(new GUIContent(folders[i].name), false, (folder) => Selection.activeObject = (Object)folder, folders[i]);
                        totalAdded++;
                        addedInSection++;
                    }
                }

                addedInSection = 0;

                for (int i = 0; i < scripts.Count; i++)
                {
                    if (scripts[i] != null)
                    {
                        Component[] components = (Component[])FindObjectsByType(scripts[i].GetClass(), FindObjectsInactive.Include, FindObjectsSortMode.None);

                        if (components.Length > 0)
                        {
                            if (addedInSection == 0 && totalAdded != 0)
                            {
                                menu.AddSeparator("");
                            }

                            if (components.Length == 1)
                            {
                                menu.AddItem(new GUIContent(scripts[i].name + " (Scene)"), false, () => Selection.activeObject = components[0]);
                            }
                            else if (components.Length > 1)
                            {
                                menu.AddItem(new GUIContent(scripts[i].name + " (Scene)"), false, () => Selection.objects = components.Select(component => component.gameObject).ToArray());
                            }

                            totalAdded++;
                            addedInSection++;
                        }
                    }
                }
            }

            menu.DropDown(rect);
        }



        void AddDropdownChoice(GenericMenu menu, bool isSelected, string text, Action<Item, HistoryStack> action, Item item, HistoryStack history)
        {
            void PerformHitoryAction(Action<Item, HistoryStack> hitoryAction, object historyData)
            {
                Tuple<Item, HistoryStack> tuple = (Tuple<Item, HistoryStack>)historyData;
                hitoryAction(tuple.First, tuple.Second);
            }

            object data = new Tuple<Item, HistoryStack> { First = item, Second = history };
            menu.AddItem(new GUIContent(text), isSelected, (d) => PerformHitoryAction(action, d), data);
        }

        void OpenHistoryItem(Item item, HistoryStack history)
        {
            item.Open();
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Clear History"), false, () =>
            {
                _selectionHistory.Clear();
                _sceneHistory.Clear();
                _prefabHistory.Clear();
            });
            menu.AddItem(new GUIContent("Edit Favourites"), false, () =>
            {
                SettingsService.OpenUserPreferences("Preferences/Favourites");
            });
        }

        static void LoadFavourites()
        {
            if (_favouritesContainer == null)
            {
                _favouritesContainer = CreateInstance<FavouritesContainer>();
            }

            _favouritesContainer.assets = EditorPrefsUtils.GetAssetList("FavouriteAssets", new List<Object>());
            _favouritesContainer.folders = EditorPrefsUtils.GetAssetList("FavouriteFolders", new List<Object>()).Cast<DefaultAsset>().ToList();
            _favouritesContainer.sceneObjects = EditorPrefsUtils.GetAssetList("FavouriteObjects", new List<Object>()).Cast<MonoScript>().ToList();
        }

        static void SaveFavourites()
        {
            EditorPrefsUtils.SetAssetList("FavouriteAssets", _favouritesContainer.assets);
            EditorPrefsUtils.SetAssetList("FavouriteFolders", _favouritesContainer.folders.Cast<Object>().ToList());
            EditorPrefsUtils.SetAssetList("FavouriteObjects", _favouritesContainer.sceneObjects.Cast<Object>().ToList());
        }



        [UnityEditor.SettingsProvider]
        static UnityEditor.SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Preferences/Favourites", SettingsScope.User)
            {

                guiHandler = (searchContext) =>
                {
                    LoadFavourites();

                    SerializedObject serializedObject = new SerializedObject(_favouritesContainer);

                    EditorGUI.BeginChangeCheck();

                    serializedObject.Update();
                    SerializedProperty property = serializedObject.GetIterator();
                    if (property.NextVisible(true))
                    {
                        do
                        {
                            if (property.name != "m_Script")
                            {
                                EditorGUILayout.PropertyField(property, true);
                            }
                        }
                        while (property.NextVisible(false));
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        SaveFavourites();
                    }
                },

                keywords = new[] { "Favourites" }
            };
        }

    }
}