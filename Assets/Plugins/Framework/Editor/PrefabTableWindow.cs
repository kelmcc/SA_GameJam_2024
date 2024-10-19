using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework;
using UnityEditor;
using UnityEngine;

public class PrefabTableWindow : AssetTableView
{

    [SerializeField]
    private string[] _componentTypeNames;

    [SerializeField]
    private string _assetPath;

    private TableView<GameObject> _table;
    private bool _refreshComponentColumns = true;
    private Dictionary<Type, List<TableView<GameObject>.Column>> _componentColumns;
    private List<Type> _selectedComponents = new List<Type>();
    private Type[] _componentTypes;


    public void Setup(string title, string assetPath, Type[] componentTypes)
    {
        _assetPath = assetPath;

        _componentTypeNames = new string[componentTypes.Length];

        for (int i = 0; i < componentTypes.Length; i++)
        {
            _componentTypeNames[i] = componentTypes[i].AssemblyQualifiedName;
        }

        titleContent = new GUIContent(title);
        Focus();
        Repaint();
    }

    void Init()
    {
        _refreshComponentColumns = true;




        _table = new TableView<GameObject>();

        TableView<GameObject>.Column nameColumn = new TableView<GameObject>.Column(new GUIContent("Name"))
        {
            GetValueFunction = prefab => prefab.name,
            Width = 180f
        };

        _table.AddColumn(nameColumn);

        _componentTypes = new Type[_componentTypeNames.Length];
        for (int i = 0; i < _componentTypes.Length; i++)
        {
            _componentTypes[i] = Type.GetType(_componentTypeNames[i]);
        }

        _componentColumns = new Dictionary<Type, List<TableView<GameObject>.Column>>();
        for (int i = 0; i < _componentTypes.Length; i++)
        {

            FieldInfo[] fields = _componentTypes[i].GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<TableView<GameObject>.Column> columns = new List<TableView<GameObject>.Column>();

            for (int j = 0; j < fields.Length; j++)
            {
                if (fields[j].IsPublic || fields[j].HasAttribute<SerializeField>())
                {
                    columns.Add(CreatePropertyColumn(_componentTypes[i], StringUtils.Titelize(fields[j].Name), fields[j].Name));
                }
            }

            _componentColumns.Add(_componentTypes[i], columns);
        }

        _table.OnItemRightClicked += (prefab) =>
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Open Prefab"), false, () => AssetDatabase.OpenAsset(prefab));
            menu.ShowAsContext();
        };

        _table.SetSortingOrder(nameColumn);
    }

    private void OnGUI()
    {


        if (_table == null)
        {
            Init();
        }

        if (_table.UnfilteredRowCount == 0)
        {
            GameObject[] prefabs = FileUtils.GetAssetsInFolder<GameObject>(_assetPath, true);
            for (int i = 0; i < prefabs.Length; i++)
            {
                _table.AddRow(prefabs[i]);
            }
        }


        const float SEARCH_HEIGHT = 20f;
        const float SEARCH_MARGIN = 5f;
        const float LOCK_WIDTH = 16f;
        const float SETTINGS_MARGIN = 4f;
        const float LOCK_MARGIN = 2f;
        const float WINDOW_MARGIN = 10f;
        const float SETTINGS_WIDTH = 250f;

        Rect tableRect = new Rect(WINDOW_MARGIN, WINDOW_MARGIN + SEARCH_HEIGHT + SEARCH_MARGIN, position.width - WINDOW_MARGIN * 2, position.height - SEARCH_HEIGHT - SEARCH_MARGIN - WINDOW_MARGIN * 2);
        Rect searchRect = new Rect(WINDOW_MARGIN, WINDOW_MARGIN, position.width - SETTINGS_WIDTH - WINDOW_MARGIN * 2, SEARCH_HEIGHT);
        Rect settingsRect = new Rect(searchRect.xMax + SETTINGS_MARGIN, searchRect.y - 1, SETTINGS_WIDTH - LOCK_WIDTH - LOCK_MARGIN - SETTINGS_MARGIN, SEARCH_HEIGHT);
        Rect lockRect = new Rect(settingsRect.xMax + LOCK_MARGIN, searchRect.y, LOCK_WIDTH, SEARCH_HEIGHT);

        GUILayout.BeginArea(settingsRect);
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();


        _selectedComponents = EditorUtils.MaskField(GUIContent.none, _selectedComponents, _componentTypes);
        if (EditorGUI.EndChangeCheck())
        {
            _refreshComponentColumns = true;
        }



        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        _table.AllowPropertyEditing = !GUI.Toggle(lockRect, !_table.AllowPropertyEditing, GUIContent.none, GUI.skin.GetStyle("IN LockButton"));
        _table.AllowRenaming = _table.AllowPropertyEditing;

        if (_refreshComponentColumns)
        {
            _refreshComponentColumns = false;


            for (int i = 0; i < _componentTypes.Length; i++)
            {
                List<TableView<GameObject>.Column> columns = _componentColumns[_componentTypes[i]];
                bool selected = _selectedComponents.Contains(_componentTypes[i]);

                for (int j = 0; j < columns.Count; j++)
                {
                    _table.RemoveColumn(columns[j]);
                    if (selected)
                    {
                        _table.AddColumn(columns[j]);
                    }
                }
            }
        }


        _table.Draw(tableRect, searchRect);
    }



    protected override void Refresh()
    {
        _table = null;
    }

}
