using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectTableWindow : AssetTableView
{

    [SerializeField]
    private string _assetPath;

    [SerializeField]
    private string _typeName;

    private TableView<ScriptableObject> _table;
    private Type _type;

    public void Setup<T>(string assetPath) where T : ScriptableObject
    {
        _assetPath = assetPath;
        _typeName = typeof(T).AssemblyQualifiedName;
        titleContent = new GUIContent(StringUtils.Pluralise(StringUtils.Titelize(typeof(T).Name)));
        Focus();
        Repaint();
    }

    void Init()
    {
        _table = new TableView<ScriptableObject>();
        _type = Type.GetType(_typeName);

        TableView<ScriptableObject>.Column nameColumn = new TableView<ScriptableObject>.Column(new GUIContent("Name"))
        {
            GetValueFunction = prefab => prefab.name,
            Width = 180f
        };

        _table.AddColumn(nameColumn);


        FieldInfo[] fields = _type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        for (int j = 0; j < fields.Length; j++)
        {
            if (fields[j].IsPublic || fields[j].HasAttribute<SerializeField>())
            {

                _table.AddColumn(CreatePropertyColumn(StringUtils.Titelize(fields[j].Name), fields[j].Name));
            }
        }

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
            ScriptableObject[] assets = FileUtils.GetAssetsInFolder<ScriptableObject>(_assetPath, true);
            for (int i = 0; i < assets.Length; i++)
            {
                _table.AddRow(assets[i]);
            }
        }

        const float SEARCH_HEIGHT = 20f;
        const float SEARCH_MARGIN = 5f;
        const float LOCK_WIDTH = 16f;
        const float LOCK_MARGIN = 2f;
        const float WINDOW_MARGIN = 10f;

        Rect tableRect = new Rect(WINDOW_MARGIN, WINDOW_MARGIN + SEARCH_HEIGHT + SEARCH_MARGIN, position.width - WINDOW_MARGIN * 2, position.height - SEARCH_HEIGHT - SEARCH_MARGIN - WINDOW_MARGIN * 2);
        Rect searchRect = new Rect(WINDOW_MARGIN, WINDOW_MARGIN, position.width - LOCK_WIDTH - WINDOW_MARGIN * 2, SEARCH_HEIGHT);
        Rect lockRect = new Rect(searchRect.xMax + LOCK_MARGIN, searchRect.y, LOCK_WIDTH, SEARCH_HEIGHT);

        _table.AllowPropertyEditing = !GUI.Toggle(lockRect, !_table.AllowPropertyEditing, GUIContent.none, GUI.skin.GetStyle("IN LockButton"));
        _table.AllowRenaming = _table.AllowPropertyEditing;
        _table.Draw(tableRect, searchRect);
    }


    protected override void Refresh()
    {
        _table = null;
    }


}
