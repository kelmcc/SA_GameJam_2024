using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEditor;
using UnityEngine;

public abstract class AssetTableView : EditorWindow, IHasCustomMenu
{

    protected abstract void Refresh();

    public void AddItemsToMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Refresh"), false, Refresh);
    }

    protected TableView<ScriptableObject>.Column CreateValueColumn(string columnName, TableView<ScriptableObject>.GetValueFunction getValueFunction)
    {
        TableView<ScriptableObject>.Column column = new TableView<ScriptableObject>.Column(new GUIContent(columnName))
        {
            GetValueFunction = getValueFunction,
            SortedAscending = false,
            NumberFormatting = "0.#"
        };

        return column;
    }


    protected TableView<ScriptableObject>.Column CreatePropertyColumn(string columnName, string propertyName)
    {
        TableView<ScriptableObject>.Column column = new TableView<ScriptableObject>.Column(new GUIContent(columnName))
        {
            GetPropertyFunction = (scriptableObject) => new SerializedObject(scriptableObject).FindProperty(propertyName),
            SortedAscending = false
        };

        return column;
    }


    protected TableView<GameObject>.Column CreateValueColumn(string columnName, TableView<GameObject>.GetValueFunction getValueFunction)
    {
        TableView<GameObject>.Column column = new TableView<GameObject>.Column(new GUIContent(columnName))
        {
            GetValueFunction = getValueFunction,
            SortedAscending = false,
            NumberFormatting = "0.#"
        };

        return column;
    }

    protected TableView<GameObject>.Column CreateValueColumn<T>(string columnName, Func<T, object> getValueFunction, Func<T, bool> filter = null) where T : Component
    {
        TableView<GameObject>.Column column = new TableView<GameObject>.Column(new GUIContent(columnName))
        {
            GetValueFunction = (prefab) =>
            {
                T[] components = prefab.GetComponentsInChildren<T>();

                for (int i = 0; i < components.Length; i++)
                {
                    if (filter == null || filter(components[i]))
                    {
                        return getValueFunction(components[i]);
                    }
                }
                return null;
            },
            SortedAscending = false,
            NumberFormatting = "0.#"
        };

        return column;
    }

    protected TableView<GameObject>.Column CreatePropertyColumn(Type type, string columnName, string propertyName)
    {
        TableView<GameObject>.Column column = new TableView<GameObject>.Column(new GUIContent(columnName))
        {
            GetPropertyFunction = (prefab) =>
            {
                Component component = prefab.GetComponentInChildren(type);
                if (component != null)
                {
                    return new SerializedObject(component).FindProperty(propertyName);
                }
                return null;
            },
            SortedAscending = false
        };

        return column;
    }

    protected TableView<GameObject>.Column CreatePropertyColumn<T>(string columnName, string propertyName, Func<T, bool> selector = null) where T : Component
    {
        TableView<GameObject>.Column column = new TableView<GameObject>.Column(new GUIContent(columnName))
        {
            GetPropertyFunction = (prefab) =>
            {
                T[] components = prefab.GetComponents<T>();

                for (int i = 0; i < components.Length; i++)
                {
                    if (selector == null || selector(components[i]))
                    {
                        return new SerializedObject(components[i]).FindProperty(propertyName);
                    }
                }
                return null;
            },
            SortedAscending = false
        };

        return column;
    }
}
