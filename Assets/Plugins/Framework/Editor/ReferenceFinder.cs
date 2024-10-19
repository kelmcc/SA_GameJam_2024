using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;


namespace Framework
{
    public static class ReferenceFinder
    {

        public class AssetReference
        {
            public Object Obj;
            public SceneAsset Scene;
            public string Path;

            public AssetReference(Object obj, SceneAsset scene, string path)
            {
                Obj = obj;
                Scene = scene;
                Path = path;
            }
        }

        public class ValueReference
        {
            public SceneAsset Scene;
            public string Path;
            public object Value;
            public Object Obj;
            public string ValueString;

            public ValueReference(Object obj, object value, SceneAsset scene, string path)
            {
                Obj = obj;
                Value = value;
                Scene = scene;
                Path = path;
                ValueString = value == null ? "NULL" : value.ToString();
            }
        }

        public class StructReference
        {
            public SceneAsset Scene;
            public string Path;
            public Object Obj;
            public FieldInfo Field;
            public string ValueString;

            public StructReference(Object obj, FieldInfo field, object value, SceneAsset scene, string path)
            {
                Obj = obj;
                Field = field;
                Scene = scene;
                Path = path;
                ValueString = "";

                FieldInfo[] fields = value.GetType().GetFieldsIncludingParentTypes(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                for (int j = 0; j < fields.Length; j++)
                {
                    if (fields[j].IsPublic || fields[j].HasAttribute<SerializeField>())
                    {
                        if (ValueString.Length != 0)
                        {
                            ValueString += "\n";
                        }
                        ValueString += fields[j].Name + ": " + fields[j].GetValue(value);
                    }
                }
            }
        }

        static bool FilterPath(string path, bool includePlugins)
        {
            if (includePlugins) return true;
            if (path.Contains("/Plugins/")) return false;
            if (path.StartsWith("Packages/")) return false;

            return true;
        }



        public static List<AssetReference> FindReferencesInAssets(Object asset, string searchFilter, bool includePlugins)
        {

            string guid;
            long localid;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out localid);

            return ProcessAssets<AssetReference>("", searchFilter, includePlugins, "Searching for " + asset.name + " references", (path, references) =>
           {
               if (FileContainsGUID(path, guid))
               {
                   references.Add(new AssetReference(AssetDatabase.LoadMainAssetAtPath(path), null, path));
               }
           });

        }


        public static List<AssetReference> FindReferencesInPrefabs(Object asset, string searchFilter, bool includePlugins)
        {
            return FindReferencesInAssetType<GameObject>("Prefab", asset, searchFilter, includePlugins);
        }

        public static List<AssetReference> FindReferencesInMaterials(Object asset, string searchFilter, bool includePlugins)
        {
            return FindReferencesInAssetType<Material>("Material", asset, searchFilter, includePlugins);
        }

        public static List<AssetReference> FindReferencesInScriptableObjects(Object asset, string searchFilter, bool includePlugins)
        {
            return FindReferencesInAssetType<ScriptableObject>("ScriptableObject", asset, searchFilter, includePlugins);
        }

        public static List<StructReference> FindStructsInGameObject(GameObject gameObject, Type structType)
        {
            List<StructReference> references = new List<StructReference>();
            MonoBehaviour[] behaviours = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
            SceneAsset scene = GetScene(gameObject);

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] != null)
                {
                    FieldInfo[] fields = behaviours[i].GetType().GetFieldsIncludingParentTypes(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    for (int j = 0; j < fields.Length; j++)
                    {
                        if (fields[j].FieldType == structType && (fields[j].IsPublic || fields[j].HasAttribute<SerializeField>()))
                        {
                            object value = fields[j].GetValue(behaviours[i]);
                            if (value != null)
                            {
                                references.Add(new StructReference(gameObject, fields[j], value, scene, behaviours[i].GetPath()));
                            }
                        }
                    }
                }
            }

            return references;
        }

        public static List<ValueReference> FindStructValueInGameObject(GameObject gameObject, Type structType, FieldInfo structField)
        {
            List<ValueReference> references = new List<ValueReference>();
            MonoBehaviour[] behaviours = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
            SceneAsset scene = GetScene(gameObject);

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] != null)
                {
                    FieldInfo[] fields = behaviours[i].GetType().GetFieldsIncludingParentTypes(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    for (int j = 0; j < fields.Length; j++)
                    {
                        if (fields[j].FieldType == structType && (fields[j].IsPublic || fields[j].HasAttribute<SerializeField>()))
                        {
                            object structValue = fields[j].GetValue(behaviours[i]);
                            if (structValue != null)
                            {
                                references.Add(new ValueReference(gameObject, structField.GetValue(structValue), scene, behaviours[i].GetPath()));
                            }
                        }
                    }
                }
            }

            return references;
        }


        public static List<StructReference> FindStructsInPrefabs(Type structType, bool includePlugins)
        {
            return ProcessAssets<StructReference>("Prefab", "", includePlugins, "Searching for " + GetStructName(structType) + " values", (path, references) =>
            {
                references.AddRange(FindStructsInGameObject((GameObject)AssetDatabase.LoadMainAssetAtPath(path), structType));
            });
        }

        public static List<StructReference> FindStructsInScriptableObjects(Type structType, bool includePlugins)
        {
            return ProcessAssets<StructReference>("ScriptableObject", "", includePlugins, "Searching for " + GetStructName(structType) + " values", (path, references) =>
            {
                ScriptableObject asset = (ScriptableObject)AssetDatabase.LoadMainAssetAtPath(path);

                FieldInfo[] fields = asset.GetType().GetFieldsIncludingParentTypes(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                for (int j = 0; j < fields.Length; j++)
                {
                    if (fields[j].FieldType == structType && (fields[j].IsPublic || fields[j].HasAttribute<SerializeField>()))
                    {
                        object value = fields[j].GetValue(asset);
                        if (value != null)
                        {
                            references.Add(new StructReference(asset, fields[j], value, null, path));
                        }
                    }
                }
            });

        }

        public static List<StructReference> FindStructsInCurrentScene(Type structType)
        {
            List<StructReference> references = new List<StructReference>();


            GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                references.AddRange(FindStructsInGameObject(gameObjects[i], structType));
            }

            return references;
        }

        public static List<StructReference> FindStructsInAllScenes(Type structType, bool includePlugins)
        {
            List<StructReference> references = new List<StructReference>();

            EditorUtils.PerformActionInScenes("Searching for " + GetStructName(structType) + " values", (path) => FilterPath(path, includePlugins), (scene, path) =>
            {
                GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    references.AddRange(FindStructsInGameObject(gameObjects[i], structType));
                }

            });

            return references;
        }

        public static List<StructReference> FindStructsInBuildSettingsScenes(Type structType)
        {
            List<StructReference> references = new List<StructReference>();

            EditorUtils.PerformActionInBuildSettingsScenes("Searching for " + GetStructName(structType) + " values", (scene, path) =>
            {
                GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    references.AddRange(FindStructsInGameObject(gameObjects[i], structType));
                }

            });

            return references;
        }

        public static List<ValueReference> FindStructFieldValuesInPrefabs(Type structType, FieldInfo field, bool includePlugins)
        {
            return ProcessAssets<ValueReference>("Prefab", "", includePlugins, "Searching for " + field.Name + " (" + GetStructName(structType) + ")" + " values", (path, references) =>
          {
              references.AddRange(FindStructValueInGameObject((GameObject)AssetDatabase.LoadMainAssetAtPath(path), structType, field));
          });
        }


        public static List<ValueReference> FindStructFieldValuesInScriptableObjects(Type structType, FieldInfo field, bool includePlugins)
        {
            return ProcessAssets<ValueReference>("ScriptableObject", "", includePlugins, "Searching for " + field.Name + " (" + GetStructName(structType) + ")" + " values", (path, references) =>
             {
                 ScriptableObject asset = (ScriptableObject)AssetDatabase.LoadMainAssetAtPath(path);

                 FieldInfo[] fields = asset.GetType().GetFieldsIncludingParentTypes(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                 for (int j = 0; j < fields.Length; j++)
                 {
                     if (fields[j].FieldType == structType && (fields[j].IsPublic || fields[j].HasAttribute<SerializeField>()))
                     {
                         object value = fields[j].GetValue(asset);
                         if (value != null)
                         {
                             references.Add(new ValueReference(asset, field.GetValue(value), null, path));
                         }
                     }
                 }
             });
        }

        public static List<ValueReference> FindStructFieldValuesInAllScenes(Type structType, FieldInfo field, bool includePlugins)
        {
            List<ValueReference> references = new List<ValueReference>();

            EditorUtils.PerformActionInScenes("Searching for " + field.Name + " (" + GetStructName(structType) + ")" + " values", (path) => FilterPath(path, includePlugins), (scene, path) =>
            {
                GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    references.AddRange(FindStructValueInGameObject(gameObjects[i], structType, field));
                }

            });

            return references;
        }

        public static List<ValueReference> FindStructFieldValuesInBuildSettingsScenes(Type structType, FieldInfo field)
        {
            List<ValueReference> references = new List<ValueReference>();

            EditorUtils.PerformActionInBuildSettingsScenes("Searching for " + field.Name + " (" + GetStructName(structType) + ")" + " values", (scene, path) =>
            {
                GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    references.AddRange(FindStructValueInGameObject(gameObjects[i], structType, field));
                }

            });

            return references;
        }

        public static List<ValueReference> FindStructFieldValuesInCurrentScene(Type structType, FieldInfo field)
        {
            List<ValueReference> references = new List<ValueReference>();

            GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                references.AddRange(FindStructValueInGameObject(gameObjects[i], structType, field));
            }

            return references;
        }

        public static List<ValueReference> FindValuesInPrefabs(Type componentType, FieldInfo field, bool includePlugins)
        {
            return ProcessAssets<ValueReference>("Prefab", "", includePlugins, "Searching for " + field.Name + " (" + componentType.Name + ")" + " values", (path, references) =>
             {
                 GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath(path);
                 Component[] components = prefab.GetComponentsInChildren(componentType, true);

                 for (int j = 0; j < components.Length; j++)
                 {

                     references.Add(new ValueReference(prefab, field.GetValue(components[j]), null, components[j].gameObject.GetPath()));
                 }
             });
        }

        public static List<ValueReference> FindValuesInPrefabs(Type componentType, FieldInfo field, FieldInfo subField, bool includePlugins)
        {
            return ProcessAssets<ValueReference>("Prefab", "", includePlugins, "Searching for " + field.Name + "." + subField.Name + " (" + componentType.Name + ")" + " values", (path, references) =>
            {
                GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath(path);
                Component[] components = prefab.GetComponentsInChildren(componentType, true);

                for (int j = 0; j < components.Length; j++)
                {
                    references.Add(new ValueReference(prefab, subField.GetValue(field.GetValue(components[j])), null, components[j].gameObject.GetPath()));
                }
            });
        }

        public static List<ValueReference> FindValuesInScriptableObjects(Type type, FieldInfo field, bool includePlugins)
        {
            return ProcessAssets<ValueReference>(type.Name, "", includePlugins, "Searching for " + field.Name + " (" + type.Name + ")" + " values", (path, references) =>
             {
                 ScriptableObject asset = (ScriptableObject)AssetDatabase.LoadMainAssetAtPath(path);
                 references.Add(new ValueReference(asset, field.GetValue(asset), null, path));
             });
        }

        public static List<ValueReference> FindValuesInScriptableObjects(Type type, FieldInfo field, FieldInfo subField, bool includePlugins)
        {
            return ProcessAssets<ValueReference>(type.Name, "", includePlugins, "Searching for " + field.Name + "." + subField.Name + " (" + type.Name + ")" + " values", (path, references) =>
           {
               ScriptableObject asset = (ScriptableObject)AssetDatabase.LoadMainAssetAtPath(path);
               references.Add(new ValueReference(asset, subField.GetValue(field.GetValue(asset)), null, path));
           });
        }


        public static List<ValueReference> FindValuesInMaterials(Shader shader, int propertyIndex, bool includePlugins)
        {
            string propertyName = shader.GetPropertyName(propertyIndex);
            int propertyID = shader.GetPropertyNameId(propertyIndex);
            ShaderPropertyType propertyType = shader.GetPropertyType(propertyIndex);

            return ProcessAssets<ValueReference>("Material", "", includePlugins, "Searching for " + propertyName + " values", (path, references) =>
            {
                Material material = AssetDatabase.LoadMainAssetAtPath(path) as Material;
                if (material != null && material.shader == shader)
                {
                    references.Add(new ValueReference(material, material.GetPropertyValue(propertyID, propertyType), null, path));
                }
            });
        }

        public static List<AssetReference> FindReferencesInAllScenes(Object obj, bool includePlugins)
        {

            if (obj is MonoScript script)
            {
                List<AssetReference> references = new List<AssetReference>();
                Type type = script.GetClass();

                EditorUtils.PerformActionInScenes("Searching for " + obj.name + " references", (path) => FilterPath(path, includePlugins), (scene, path) =>
                 {
                     references.AddRange(FindComponentReferencesInCurrentScene(type));
                 });

                return references;
            }

            if (obj is GameObject prefab)
            {
                if (prefab.IsPrefabAsset())
                {
                    List<AssetReference> references = new List<AssetReference>();

                    EditorUtils.PerformActionInScenes("Searching for " + obj.name + " references", (path) => FilterPath(path, includePlugins), (scene, path) =>
                      {
                          references.AddRange(FindPrefabInstancesInCurrentScene(prefab));
                      });

                    return references;
                }
            }

            throw new ArgumentException("Object is not a MonoScript or a prefab asset: " + obj);
        }

        public static List<AssetReference> FindReferencesInBuildSettingsScenes(Object obj)
        {

            if (obj is MonoScript script)
            {
                List<AssetReference> references = new List<AssetReference>();
                Type type = script.GetClass();

                EditorUtils.PerformActionInBuildSettingsScenes("Searching for " + obj.name + " references", (scene, path) =>
                {
                    references.AddRange(FindComponentReferencesInCurrentScene(type));
                });

                return references;
            }

            if (obj is GameObject prefab)
            {
                if (prefab.IsPrefabAsset())
                {
                    List<AssetReference> references = new List<AssetReference>();

                    EditorUtils.PerformActionInBuildSettingsScenes("Searching for " + obj.name + " references", (scene, path) =>
                   {
                       references.AddRange(FindPrefabInstancesInCurrentScene(prefab));
                   });

                    return references;
                }
            }

            throw new ArgumentException("Object is not a MonoScript or a prefab asset: " + obj);
        }

        public static List<ValueReference> FindComponentFieldValuesInAllScenes(Type type, FieldInfo field, bool includePlugins)
        {
            List<ValueReference> references = new List<ValueReference>();

            EditorUtils.PerformActionInScenes("Searching for " + field.Name + " (" + type.Name + ")" + " values", (path) => FilterPath(path, includePlugins), (scene, path) =>
               {
                   references.AddRange(FindComponentFieldValuesInCurrentScene(type, field));
               });

            return references;
        }

        public static List<ValueReference> FindComponentFieldValuesInAllScenes(Type type, FieldInfo field, FieldInfo subField, bool includePlugins)
        {
            List<ValueReference> references = new List<ValueReference>();

            EditorUtils.PerformActionInScenes("Searching for " + field.Name + "." + subField.Name + " (" + type.Name + ")" + " values", (path) => FilterPath(path, includePlugins), (scene, path) =>
            {
                references.AddRange(FindComponentFieldValuesInCurrentScene(type, field, subField));
            });

            return references;
        }


        public static List<ValueReference> FindComponentFieldValuesInBuildSettingsScenes(Type type, FieldInfo field)
        {
            List<ValueReference> references = new List<ValueReference>();

            EditorUtils.PerformActionInBuildSettingsScenes("Searching for " + field.Name + " (" + type.Name + ")" + " values", (scene, path) =>
            {
                references.AddRange(FindComponentFieldValuesInCurrentScene(type, field));
            });

            return references;
        }

        public static List<ValueReference> FindComponentFieldValuesInBuildSettingsScenes(Type type, FieldInfo field, FieldInfo subField)
        {
            List<ValueReference> references = new List<ValueReference>();

            EditorUtils.PerformActionInBuildSettingsScenes("Searching for " + field.Name + "." + subField.Name + " (" + type.Name + ")" + " values", (scene, path) =>
            {
                references.AddRange(FindComponentFieldValuesInCurrentScene(type, field, subField));
            });

            return references;
        }

        public static List<AssetReference> FindReferencesInCurrentScene(Object obj)
        {

            if (obj is MonoScript script)
            {
                return new List<AssetReference>(FindComponentReferencesInCurrentScene(script.GetClass()));
            }

            if (obj is GameObject prefab)
            {
                if (prefab.IsPrefabAsset())
                {
                    return new List<AssetReference>(FindPrefabInstancesInCurrentScene(prefab));
                }
            }

            throw new ArgumentException("Object is not a MonoScript or a prefab asset: " + obj);
        }


        public static List<AssetReference> FindPrefabInstancesInCurrentScene(GameObject prefab)
        {
            List<AssetReference> references = new List<AssetReference>();

            SceneAsset scene = GetCurrentScene();
            GameObject[] objects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            for (int i = 0; i < objects.Length; i++)
            {
                if (PrefabUtility.GetPrefabInstanceStatus(objects[i]) == PrefabInstanceStatus.Connected && PrefabUtility.GetCorrespondingObjectFromOriginalSource(objects[i]) == prefab)
                {
                    references.Add(new AssetReference(objects[i], scene, objects[i].GetPath()));
                }

            }

            return references;
        }

        public static List<AssetReference> FindComponentReferencesInCurrentScene(Type type)
        {
            List<AssetReference> references = new List<AssetReference>();

            SceneAsset scene = GetCurrentScene();
            Object[] usages = Object.FindObjectsByType(type, FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            for (int i = 0; i < usages.Length; i++)
            {
                references.Add(new AssetReference(usages[i], scene, (usages[i] as Component).gameObject.GetPath()));
            }

            return references;
        }

        public static List<ValueReference> FindComponentFieldValuesInCurrentScene(Type type, FieldInfo field)
        {
            List<ValueReference> references = new List<ValueReference>();

            SceneAsset scene = GetCurrentScene();
            Object[] usages = Object.FindObjectsByType(type, FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            for (int i = 0; i < usages.Length; i++)
            {
                references.Add(new ValueReference(usages[i], field.GetValue(usages[i]), scene, (usages[i] as Component).gameObject.GetPath()));
            }

            return references;
        }

        public static List<ValueReference> FindComponentFieldValuesInCurrentScene(Type type, FieldInfo field, FieldInfo subField)
        {
            List<ValueReference> references = new List<ValueReference>();

            SceneAsset scene = GetCurrentScene();
            Object[] usages = Object.FindObjectsByType(type, FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            for (int i = 0; i < usages.Length; i++)
            {
                references.Add(new ValueReference(usages[i], subField.GetValue(field.GetValue(usages[i])), scene, (usages[i] as Component).gameObject.GetPath()));
            }

            return references;
        }

        public static List<Tuple<SceneAsset, int>> CountReferencesInAllScenes(Object obj)
        {
            List<Tuple<SceneAsset, int>> counts = new List<Tuple<SceneAsset, int>>();

            EditorUtils.PerformActionInAllScenes("Searching for " + obj.name + " references", (scene, path) =>
            {
                List<AssetReference> references = FindReferencesInCurrentScene(obj);
                if (references.Count > 0)
                {
                    counts.Add(new Tuple<SceneAsset, int>(scene, references.Count));
                }
            });

            return counts;
        }

        static SceneAsset GetCurrentScene()
        {
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
        }

        static SceneAsset GetScene(GameObject gameObject)
        {
            return gameObject.IsPrefabAsset() ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
        }


        static string GetStructName(Type type)
        {
            return type.FullName.RemoveFromEnd("+<>c");
        }



        static List<AssetReference> FindReferencesInAssetType<T>(string assetTypeFilter, Object asset, string searchFilter, bool includePlugins) where T : Object
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long localid);
            HashSet<string> modelPaths = new HashSet<string>();
            bool isPrefab = assetTypeFilter == "Prefab";

            return ProcessAssets<AssetReference>(assetTypeFilter, searchFilter, includePlugins, "Searching for " + asset.name + " references", (path, references) =>
            {
                Object obj = AssetDatabase.LoadMainAssetAtPath(path);
                if (obj as T != null)
                {
                    if (FileContainsGUID(path, guid))
                    {
                        references.Add(new AssetReference(obj, null, path));
                        return;
                    }
                }

                if (isPrefab && obj is GameObject go)
                {
                    MeshFilter[] filters = go.GetComponentsInChildren<MeshFilter>();
                    for (int j = 0; j < filters.Length; j++)
                    {
                        if (filters[j].sharedMesh != null)
                        {
                            string modelPath = AssetDatabase.GetAssetPath(filters[j].sharedMesh);
                            if (!string.IsNullOrEmpty(modelPath) && !modelPaths.Contains(modelPath))
                            {
                                modelPaths.Add(modelPath);
                                string metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(modelPath);

                                if (!string.IsNullOrEmpty(metaPath) && metaPath.StartsWith("Assets"))
                                {
                                    string contents = File.ReadAllText(metaPath);
                                    if (contents.Contains(guid))
                                    {
                                        references.Add(new AssetReference(obj, null, path));
                                    }
                                }
                            }
                        }
                    }
                }
            });

        }



        /* public static bool PathHasReferenceToAsset<T>(string guidToSearchFor,string pathToSearch,bool pathIsPrefabAsset,out AssetReference reference)where T : Object
         {
             AssetDatabase.TryGetGUIDAndLocalFileIdentifier(assetToSearchFor, out string guid, out long localid);

             Object obj = AssetDatabase.LoadMainAssetAtPath(pathToSearch);
             if (obj as T != null)
             {
                 if (FileContainsGUID(pathToSearch, guid))
                 {
                     references.Add(new AssetReference(obj, null, pathToSearch));
                     return;
                 }
             }

             if (pathIsPrefabAsset && obj is GameObject go)
             {
                 MeshFilter[] filters = go.GetComponentsInChildren<MeshFilter>();
                 for (int j = 0; j < filters.Length; j++)
                 {
                     if (filters[j].sharedMesh != null)
                     {
                         string modelPath = AssetDatabase.GetAssetPath(filters[j].sharedMesh);
                         if (!string.IsNullOrEmpty(modelPath))
                         {
                             modelPaths.Add(modelPath);
                             string metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(modelPath);

                             if (!string.IsNullOrEmpty(metaPath) && metaPath.StartsWith("Assets"))
                             {
                                 string contents = File.ReadAllText(metaPath);
                                 if (contents.Contains(guid))
                                 {
                                     references.Add(new AssetReference(obj, null, pathToSearch));
                                 }
                             }
                         }
                     }
                 }
             }
         }
        */

        static bool FileContainsGUID(string path, string guid)
        {
            if (Directory.Exists(path)) return false;

            string contents = File.ReadAllText(path);
            if (contents.Contains(guid))
            {
                return true;
            }

            return false;
        }

        static List<T> ProcessAssets<T>(string typeFilter, string searchFilter, bool includePlugins, string searchTitle, Action<string, List<T>> pathFunction)
        {

            string search = string.IsNullOrWhiteSpace(typeFilter) ? "" : "t:" + typeFilter;

            if (!string.IsNullOrEmpty(searchFilter))
            {
                search += " " + searchFilter;
            }

            List<string> paths = new List<string>();
            string[] guids = AssetDatabase.FindAssets(search);

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (FilterPath(path, includePlugins))
                {
                    paths.Add(path);
                }
            }


            try
            {
                List<T> results = new List<T>();

                for (int i = 0; i < paths.Count; i++)
                {
                    string info = "Searching asset (" + (i + 1) + " / " + paths.Count + ")";
                    float progress = ((float)i) / paths.Count;

                    if (EditorUtility.DisplayCancelableProgressBar(searchTitle, info, progress)) break;

                    pathFunction(paths[i], results);
                }

                return results;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

        }

    }
}
