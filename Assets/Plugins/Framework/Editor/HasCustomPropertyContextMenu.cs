using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Invokes the method with the following signature when the property is right-clicked: static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class HasCustomPropertyContextMenu : Attribute
{
    delegate void OnPropertyContextMenuCallback(GenericMenu menu, SerializedProperty property);

    private static bool _isInitialized;
    private static FieldInfo _drawerTypeField;
    private static Dictionary<Type, MethodInfo> _methodsByType;

    [InitializeOnLoadMethod]
    public static void Register()
    {
        _isInitialized = false;

        EditorApplication.contextualPropertyMenu -= OnContextualPropertyMenu;
        EditorApplication.contextualPropertyMenu += OnContextualPropertyMenu;
    }

    static void Init()
    {
        _isInitialized = true;


        TypeCache.TypeCollection drawerTypes = TypeCache.GetTypesWithAttribute<HasCustomPropertyContextMenu>();

        if (drawerTypes.Count > 0)
        {
            _drawerTypeField = typeof(CustomPropertyDrawer).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
            _methodsByType = new Dictionary<Type, MethodInfo>();

            for (int i = 0; i < drawerTypes.Count; i++)
            {
                CustomPropertyDrawer drawerAttribute = drawerTypes[i].GetAttribute<CustomPropertyDrawer>();
                Type targetType = _drawerTypeField.GetValue(drawerAttribute) as Type;
                MethodInfo method = drawerTypes[i].GetMethod("OnPropertyContextMenu", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                _methodsByType.Add(targetType, method);
            }

            for (int i = 0; i < drawerTypes.Count; i++)
            {
                CustomPropertyDrawer drawerAttribute = drawerTypes[i].GetAttribute<CustomPropertyDrawer>();
                Type targetType = _drawerTypeField.GetValue(drawerAttribute) as Type;
                MethodInfo method = drawerTypes[i].GetMethod("OnPropertyContextMenu", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                TypeCache.TypeCollection derviedTypes = TypeCache.GetTypesDerivedFrom(targetType);
                for (int j = 0; j < derviedTypes.Count; j++)
                {
                    if (!_methodsByType.ContainsKey(derviedTypes[j]))
                    {
                        MethodInfo derivedMethod = derviedTypes[j].GetMethod("OnPropertyContextMenu", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        _methodsByType.Add(derviedTypes[j], derivedMethod == null ? method : derivedMethod);
                    }
                }
            }

        }
    }

    private static void OnContextualPropertyMenu(GenericMenu menu, SerializedProperty property)
    {
        if (!_isInitialized)
        {
            Init();
        }

        if (_methodsByType.Count > 0)
        {
            Type valueType = property.GetValueType();
            if (valueType != null && _methodsByType.TryGetValue(valueType, out MethodInfo method))
            {
                method.Invoke(null, new object[] { menu, property });
            }
        }

    }
}

