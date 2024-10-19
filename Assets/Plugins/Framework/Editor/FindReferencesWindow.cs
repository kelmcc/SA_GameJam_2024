using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Framework
{
    public class FindReferencesWindow : EditorWindow
    {

        enum Tab
        {
            Asset,
            Struct,
            FieldValue,
            ShaderProperty
        }

        private Tab _tab;
        private int _selectedFieldIndex;
        private int _selectedSubFieldIndex;
        private int _selectedShaderPropertyIndex;
        private Object _selectedAsset;
        private bool _includePlugins;
        private string _searchFilter;
        private List<Type> _structTypes;
        private List<string> _structOptionNames;
        private int _selectedStructTypeIndex;
        private int _selectedStructSubfieldIndex;

        [MenuItem("Assets/Find References", true)]
        static bool HasSelectedAssets()
        {
            return Selection.assetGUIDs.Length > 0;
        }

        [MenuItem("Assets/Find References", false)]
        static void ShowWindow()
        {
            FindReferencesWindow window = GetWindow<FindReferencesWindow>(true, "Find References", true);

            if (IsValidAsset(Selection.activeObject) || IsValidSceneAsset(Selection.activeObject))
            {
                window._selectedAsset = Selection.activeObject;
            }

            window.ShowUtility();
            window.CenterInScreen(420, 268);
        }

        private void OnSelectionChange()
        {
            _selectedFieldIndex = 0;
            Repaint();
        }

        void OnGUI()
        {
            _tab = (Tab)GUILayout.Toolbar((int)_tab, Enum.GetNames(typeof(Tab)).Select(s => StringUtils.Titelize(s)).ToArray());

            EditorGUILayout.Space();

            switch (_tab)
            {
                case Tab.Asset: AssetTab(); break;
                case Tab.Struct: StructTab(); break;
                case Tab.FieldValue: FieldTab(); break;
                case Tab.ShaderProperty: ShaderFieldTab(); break;
            }
        }

        void DrawField(Action fieldAction)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            fieldAction();
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
        }

        void AssetTab()
        {
            bool isValidAsset = IsValidAsset(_selectedAsset);
            bool isValidSceneAsset = IsValidSceneAsset(_selectedAsset);


            DrawField(() => _selectedAsset = EditorGUILayout.ObjectField(new GUIContent("Asset"), _selectedAsset, typeof(Object), true));
            DrawField(() => _searchFilter = EditorGUILayout.TextField(new GUIContent("Search Filter"), _searchFilter));
            DrawField(() => _includePlugins = EditorGUILayout.Toggle(new GUIContent("Include Plugin Folders"), _includePlugins));


            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(!isValidAsset);

            if (GUILayout.Button("Find References In Prefabs"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInPrefabs(_selectedAsset, _searchFilter, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Obj.name + "\n" + references[i].Path, references[i].Obj);
                }
            }

            if (GUILayout.Button("Find References In Materials"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInMaterials(_selectedAsset, _searchFilter, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Obj.name + "\n" + references[i].Path, references[i].Obj);
                }
            }

            if (GUILayout.Button("Find References In Scriptable Objects"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInScriptableObjects(_selectedAsset, _searchFilter, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Obj.name + "\n" + references[i].Path, references[i].Obj);
                }
            }

            if (GUILayout.Button("Find References In All Assets"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInAssets(_selectedAsset, _searchFilter, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Obj.name + "\n" + references[i].Path, references[i].Obj);
                }
            }

            EditorGUI.BeginDisabledGroup(!isValidSceneAsset);

            if (GUILayout.Button("Find References In Current Scene"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInCurrentScene(_selectedAsset);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Obj.name + "\n" + references[i].Path, references[i].Obj);
                }

            }

            if (GUILayout.Button("Find References In Build Settings Scenes"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInBuildSettingsScenes(_selectedAsset);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Path.Substring(references[i].Path.LastIndexOf('/') + 1) + "\n" + references[i].Path, references[i].Scene);
                }
            }

            if (GUILayout.Button("Find References In All Scenes"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInAllScenes(_selectedAsset, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Path.Substring(references[i].Path.LastIndexOf('/') + 1) + "\n" + references[i].Path, references[i].Scene);
                }
            }

            if (GUILayout.Button("Count References In All Scenes"))
            {
                List<Tuple<SceneAsset, int>> counts = ReferenceFinder.CountReferencesInAllScenes(_selectedAsset);
                for (int i = 0; i < counts.Count; i++)
                {
                    Debug.Log(counts[i].First.name + ": " + counts[i].Second, counts[i].First);
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
        }

        void FieldTab()
        {
            bool isComponent = IsValidComponent(_selectedAsset);
            bool isScriptableObject = IsValidScriptableObject(_selectedAsset);

            Type type = null;
            FieldInfo field = null;
            FieldInfo subField = null;

            if (isScriptableObject || isComponent)
            {
                DrawField(() => _selectedAsset = EditorGUILayout.ObjectField(new GUIContent("Script"), _selectedAsset, typeof(MonoScript), false));

                type = (_selectedAsset as MonoScript).GetClass();
                FieldInfo[] fields = TypeUtils.GetSerializedFields(type);

                if (_selectedFieldIndex >= fields.Length) _selectedFieldIndex = 0;

                if (fields.Length > 0)
                {
                    DrawField(() => _selectedFieldIndex = EditorGUILayout.Popup("Field", _selectedFieldIndex, fields.Select(f => f.Name).ToArray()));
                    field = fields[_selectedFieldIndex];

                    FieldInfo[] subFields = TypeUtils.GetSerializedFields(field.FieldType);
                    if (_selectedSubFieldIndex >= subFields.Length + 1) _selectedSubFieldIndex = 0;

                    DrawField(() => _selectedSubFieldIndex = EditorGUILayout.Popup("Sub Field", _selectedSubFieldIndex, subFields.Select(f => f.Name).Prepend("None").ToArray()));
                    subField = _selectedSubFieldIndex == 0 ? null : subFields[_selectedSubFieldIndex - 1];
                }
                else
                {
                    DrawField(() => EditorGUILayout.LabelField("Field", "Script has no fields"));
                }

            }
            else
            {
                DrawField(() => _selectedAsset = EditorGUILayout.ObjectField(new GUIContent("Script"), null, typeof(MonoScript), false));
                DrawField(() => EditorGUILayout.LabelField("Field", "NO SCRIPT SELECTED"));
            }

            DrawField(() => _includePlugins = EditorGUILayout.Toggle(new GUIContent("Include Plugin Folders"), _includePlugins));


            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!isComponent);

            if (GUILayout.Button("Find Values In Prefabs"))
            {


                List<ReferenceFinder.ValueReference> references = subField == null ? ReferenceFinder.FindValuesInPrefabs(type, field, _includePlugins) : ReferenceFinder.FindValuesInPrefabs(type, field, subField, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(!isScriptableObject);

            if (GUILayout.Button("Find Values In Scriptable Objects"))
            {
                List<ReferenceFinder.ValueReference> references = subField == null ? ReferenceFinder.FindValuesInScriptableObjects(type, field, _includePlugins) : ReferenceFinder.FindValuesInScriptableObjects(type, field, subField, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!isComponent);


            if (GUILayout.Button("Find Values In Current Scene"))
            {
                List<ReferenceFinder.ValueReference> references = subField == null ? ReferenceFinder.FindComponentFieldValuesInCurrentScene(type, field) : ReferenceFinder.FindComponentFieldValuesInCurrentScene(type, field, subField);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                }

            }

            if (GUILayout.Button("Find Values In Build Settings Scenes"))
            {
                List<ReferenceFinder.ValueReference> references = subField == null ? ReferenceFinder.FindComponentFieldValuesInBuildSettingsScenes(type, field) : ReferenceFinder.FindComponentFieldValuesInBuildSettingsScenes(type, subField);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Scene);
                }
            }

            if (GUILayout.Button("Find Values In All Scenes"))
            {
                List<ReferenceFinder.ValueReference> references = subField == null ? ReferenceFinder.FindComponentFieldValuesInAllScenes(type, field, _includePlugins) : ReferenceFinder.FindComponentFieldValuesInAllScenes(type, field, subField, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Scene);
                }
            }
            EditorGUI.EndDisabledGroup();

        }

        void StructTab()
        {

            Type structType = null;
            FieldInfo subField = null;

            if (_structTypes == null)
            {
                _structTypes = new List<Type>();
                _structOptionNames = new List<string>();

                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    if (!assemblies[i].FullName.StartsWith("Assembly-CSharp")) continue;

                    Type[] types = assemblies[i].GetTypes();

                    for (int j = 0; j < types.Length; j++)
                    {
                        if (types[j].IsAbstract) continue;
                        if (types[j].IsGenericType) continue;
                        if (!types[j].HasAttribute<SerializableAttribute>()) continue;
                        if (types[j].HasAttribute<CompilerGeneratedAttribute>()) continue;
                        if (typeof(Object).IsAssignableFrom(types[j])) continue;

                        _structTypes.Add(types[j]);
                    }
                }

                for (int i = 0; i < _structTypes.Count; i++)
                {
                    _structOptionNames.Add(_structTypes[i].FullName.RemoveFromEnd("+<>c"));
                }
            }

            if (_structTypes.Count > 0)
            {
                if (_selectedStructTypeIndex >= _structTypes.Count) _selectedStructTypeIndex = 0;
                structType = _structTypes[_selectedStructTypeIndex];
            }

            DrawField(() =>
            {
                Rect rect = EditorGUILayout.GetControlRect(true);

                SearchablePopup.DrawButton(rect, new GUIContent("Struct Type"), _structOptionNames.ToArray(), _selectedStructTypeIndex, index =>
                {
                    _selectedStructTypeIndex = index;
                    structType = _structTypes[index];
                });
            });

            if (structType != null)
            {
                FieldInfo[] subFields = TypeUtils.GetSerializedFields(structType);
                if (_selectedStructSubfieldIndex >= subFields.Length + 1) _selectedStructSubfieldIndex = 0;

                DrawField(() => _selectedStructSubfieldIndex = EditorGUILayout.Popup("Field", _selectedStructSubfieldIndex, subFields.Select(f => f.Name).Prepend("ALL FIELDS").ToArray()));
                subField = _selectedStructSubfieldIndex == 0 ? null : subFields[_selectedStructSubfieldIndex - 1];
            }

            DrawField(() => _includePlugins = EditorGUILayout.Toggle(new GUIContent("Include Plugin Folders"), _includePlugins));


            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(structType == null);

            if (GUILayout.Button("Find Structs In Prefabs"))
            {
                if (_selectedStructSubfieldIndex == 0)
                {
                    List<ReferenceFinder.StructReference> references = ReferenceFinder.FindStructsInPrefabs(structType, _includePlugins);
                    for (int i = 0; i < references.Count; i++)
                    {
                        Debug.Log(references[i].Field.DeclaringType + ": " + references[i].Field.Name + "\n" + references[i].Path + "\n\n" + references[i].ValueString + "\n", references[i].Obj);
                    }
                }
                else
                {
                    List<ReferenceFinder.ValueReference> references = ReferenceFinder.FindStructFieldValuesInPrefabs(structType, subField, _includePlugins);
                    for (int i = 0; i < references.Count; i++)
                    {
                        Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                    }
                }
            }


            if (GUILayout.Button("Find Structs In Scriptable Objects"))
            {
                if (_selectedStructSubfieldIndex == 0)
                {
                    List<ReferenceFinder.StructReference> references = ReferenceFinder.FindStructsInScriptableObjects(structType, _includePlugins);
                    for (int i = 0; i < references.Count; i++)
                    {
                        Debug.Log(references[i].Field.DeclaringType + ": " + references[i].Field.Name + "\n" + references[i].Path + "\n\n" + references[i].ValueString + "\n", references[i].Obj);
                    }
                }
                else
                {
                    List<ReferenceFinder.ValueReference> references = ReferenceFinder.FindStructFieldValuesInScriptableObjects(structType, subField, _includePlugins);
                    for (int i = 0; i < references.Count; i++)
                    {
                        Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                    }
                }
            }

            if (GUILayout.Button("Find Structs In Current Scene"))
            {
                if (_selectedStructSubfieldIndex == 0)
                {
                    List<ReferenceFinder.StructReference> references = ReferenceFinder.FindStructsInCurrentScene(structType);
                    for (int i = 0; i < references.Count; i++)
                    {
                        Debug.Log(references[i].Field.DeclaringType + ": " + references[i].Field.Name + "\n" + references[i].Path + "\n\n" + references[i].ValueString + "\n", references[i].Obj);
                    }
                }
                else
                {
                    List<ReferenceFinder.ValueReference> references = ReferenceFinder.FindStructFieldValuesInCurrentScene(structType, subField);
                    for (int i = 0; i < references.Count; i++)
                    {
                        Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                    }
                }
            }

            if (GUILayout.Button("Find Structs In Build Settings Scenes"))
            {
                if (_selectedStructSubfieldIndex == 0)
                {
                    List<ReferenceFinder.StructReference> references = ReferenceFinder.FindStructsInBuildSettingsScenes(structType);
                    for (int i = 0; i < references.Count; i++)
                    {
                        Debug.Log(references[i].Field.DeclaringType + ": " + references[i].Field.Name + "\n" + references[i].Path + "\n\n" + references[i].ValueString + "\n", references[i].Obj);
                    }
                }
                else
                {
                    List<ReferenceFinder.ValueReference> references = ReferenceFinder.FindStructFieldValuesInBuildSettingsScenes(structType, subField);
                    for (int i = 0; i < references.Count; i++)
                    {
                        Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                    }
                }
            }

            if (GUILayout.Button("Find Structs In All Scenes"))
            {
                if (_selectedStructSubfieldIndex == 0)
                {
                    List<ReferenceFinder.StructReference> references = ReferenceFinder.FindStructsInAllScenes(structType, _includePlugins);
                    for (int i = 0; i < references.Count; i++)
                    {
                        Debug.Log(references[i].Field.DeclaringType + ": " + references[i].Field.Name + "\n" + references[i].Path + "\n\n" + references[i].ValueString + "\n", references[i].Obj);
                    }
                }
                else
                {
                    List<ReferenceFinder.ValueReference> references = ReferenceFinder.FindStructFieldValuesInAllScenes(structType, subField, _includePlugins);
                    for (int i = 0; i < references.Count; i++)
                    {
                        Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                    }
                }
            }

            EditorGUI.EndDisabledGroup();

        }

        void ShaderFieldTab()
        {

            Shader shader = null;
            bool isValidShader = IsValidShader(_selectedAsset);

            if (isValidShader)
            {
                DrawField(() => _selectedAsset = EditorGUILayout.ObjectField(new GUIContent("Shader"), _selectedAsset, typeof(Shader), false));

                shader = (_selectedAsset as Shader);

                string[] names = new string[ShaderUtil.GetPropertyCount(shader)];
                for (int i = 0; i < names.Length; i++)
                {
                    names[i] = ShaderUtil.GetPropertyName(shader, i);
                }

                if (names.Length > 0)
                {
                    if (_selectedShaderPropertyIndex >= names.Length) _selectedShaderPropertyIndex = 0;

                    DrawField(() => _selectedShaderPropertyIndex = EditorGUILayout.Popup("Property", _selectedShaderPropertyIndex, names));
                }
                else
                {
                    DrawField(() => EditorGUILayout.LabelField("Property", "Shader has no properties"));
                }

            }
            else
            {
                DrawField(() => _selectedAsset = EditorGUILayout.ObjectField(new GUIContent("Shader"), null, typeof(Shader), false));
                DrawField(() => EditorGUILayout.LabelField("Property", "NO SHADER SELECTED"));
            }

            DrawField(() => _includePlugins = EditorGUILayout.Toggle(new GUIContent("Include Plugin Folders"), _includePlugins));


            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!isValidShader);
            if (GUILayout.Button("Find Values In Materials"))
            {
                List<ReferenceFinder.ValueReference> references = ReferenceFinder.FindValuesInMaterials(shader, _selectedShaderPropertyIndex, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                }
            }
            EditorGUI.EndDisabledGroup();



        }

        static bool IsValidShader(Object obj)
        {
            if (obj == null) return false;

            return obj is Shader;
        }


        static bool IsValidAsset(Object obj)
        {
            if (obj == null) return false;
            if (obj is GameObject go && go.IsPrefabAsset()) return true;
            if (obj is DefaultAsset) return false;

            return true;
        }

        static bool IsValidComponent(Object obj)
        {
            if (obj == null) return false;
            if (obj is MonoScript script)
            {
                Type type = script.GetClass();
                if (type != null)
                {
                    if (typeof(Component).IsAssignableFrom(type))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static bool IsValidScriptableObject(Object obj)
        {
            if (obj == null) return false;
            if (obj is MonoScript script)
            {
                Type type = script.GetClass();
                if (type != null)
                {
                    if (typeof(ScriptableObject).IsAssignableFrom(type))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static bool IsValidSceneAsset(Object obj)
        {
            if (obj == null) return false;
            if (obj is GameObject go && go.IsPrefabAsset()) return true;
            if (obj is MonoScript) return true;

            return false;
        }
    }
}
