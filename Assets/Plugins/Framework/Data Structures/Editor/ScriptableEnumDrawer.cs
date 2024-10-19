using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Framework
{
    [CustomPropertyDrawer(typeof(ScriptableEnum), true)]
    [CanEditMultipleObjects]
    [HasCustomPropertyContextMenu]
    public class ScriptableEnumDrawer : PropertyDrawer
    {
        const float PICKER_WIDTH = 36f;

        private static Dictionary<Type, ScriptableEnum[]> _dropdownOptions = new Dictionary<Type, ScriptableEnum[]>();
        private static Dictionary<Type, string[]> _dropdownLabels = new Dictionary<Type, string[]>();
        private static Dictionary<Type, float> _lastDropdownRefreshTimes = new Dictionary<Type, float>();

        static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            menu.AddItem(new GUIContent("Create New"), false, () =>
            {
                Type type = property.GetValueType();
                string name = StringUtils.Titelize(type.Name);
                string path = EditorUtility.SaveFilePanelInProject("Create " + name, "New " + name, "asset", "", "Assets/Resources/" + StringUtils.Pluralise(name));

                if (!string.IsNullOrEmpty(path))
                {
                    Object asset = ScriptableObject.CreateInstance(type);

                    FileUtils.CreateAssetFolders(path);
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                    ScriptableEnum.MarkAssetsForReload();

                    property.objectReferenceValue = asset;
                    property.serializedObject.ApplyModifiedProperties();
                }
            });
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            Type type = fieldInfo.FieldType;
            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                type = type.GetGenericArguments()[0];
            }

            EditorGUI.BeginProperty(position, label, property);

            DrawPopup(position.WithWidth(position.width - PICKER_WIDTH), label, type, (ScriptableEnum)property.objectReferenceValue, selectedValue =>
              {
                  property.objectReferenceValue = selectedValue;
                  property.serializedObject.ApplyModifiedProperties();
              });


            EditorGUI.indentLevel = 0;

            Rect rect = new Rect(position.xMax - PICKER_WIDTH, position.y, PICKER_WIDTH, position.height);
            EditorGUI.PropertyField(rect, property, GUIContent.none);

            EditorGUI.EndProperty();

        }


        public static void DrawPopup(Rect rect, GUIContent label, Type type, ScriptableObject currentValue, Action<ScriptableObject> onOptionSelected, bool allowNull = true)
        {
            float lastRefreshTime;
            if (!_lastDropdownRefreshTimes.TryGetValue(type, out lastRefreshTime))
            {
                lastRefreshTime = -1000f;
            }

            if (ScriptableEnum.LastAssetReloadTime > lastRefreshTime || ScriptableEnum.LastAssetReloadTime < 0)
            {
                RefreshDropdownOptions(type);
            }

            ScriptableEnum[] options = _dropdownOptions[type];
            string[] labels = _dropdownLabels[type];

            if (!allowNull)
            {
                options = options.SubArray(1);
                labels = labels.SubArray(1);
            }


            int selectedIndex = 0;
            if (currentValue != null)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    if (options[i] == currentValue)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }


            SearchablePopup.DrawButton(rect, label, labels, selectedIndex, index => onOptionSelected(options[index]));
        }



        static void RefreshDropdownOptions(Type type)
        {

            List<ScriptableEnum> values = new List<ScriptableEnum>(ScriptableEnum.GetValues(type));
            values.Sort((x, y) => x.name.CompareTo(y.name));
            values.Insert(0, null);

            ScriptableEnum[] options = values.ToArray();
            string[] labels = new string[options.Length];

            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == null)
                {
                    labels[i] = "NONE";
                }
                else
                {
                    labels[i] = options[i].name;
                }
            }


            _dropdownOptions[type] = options;
            _dropdownLabels[type] = labels;
            _lastDropdownRefreshTimes[type] = (float)EditorApplication.timeSinceStartup;

        }



    }
}

