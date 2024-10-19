using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Framework
{
    public abstract class ScriptableEnum : ScriptableObject
    {
#if UNITY_EDITOR
        public static float LastAssetReloadTime => _lastAssetLoadTime;
        private static float _lastAssetLoadTime = -1000;
#endif

        private static bool _assetsAreLoaded;
        private static List<Type> _assetTypes;
        private static Dictionary<Type, List<ScriptableEnum>> _assetLists;
        private static Dictionary<Type, Dictionary<string, ScriptableEnum>> _assetsByName;
        private static Dictionary<string, ScriptableEnum> _assetsByGUID;

        public int Index
        {
            get
            {
                if (_assetIndex < 0)
                {
                    _assetIndex = GetIndex(this);
                }

                return _assetIndex;
            }
        }

        public string GUID => _guid;

        private int _assetIndex = -1;
        [SerializeField, HideField] private string _guid;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _assetsAreLoaded = false;
        }

        protected virtual void OnEnable()
        {
            _assetsAreLoaded = false;
        }

        protected virtual void OnDestroy()
        {
            _assetsAreLoaded = false;
        }


        public static void MarkAssetsForReload()
        {
            _assetsAreLoaded = false;
        }

        public static void EnsureInstancesAreLoaded()
        {
            if (!_assetsAreLoaded)
            {
#if UNITY_EDITOR
                _lastAssetLoadTime = (float)UnityEditor.EditorApplication.timeSinceStartup;
#endif

                ScriptableEnum[] assets = Resources.LoadAll<ScriptableEnum>("");

                _assetTypes = new List<Type>();
                _assetLists = new Dictionary<Type, List<ScriptableEnum>>();
                _assetsByName = new Dictionary<Type, Dictionary<string, ScriptableEnum>>();
                _assetsByGUID = new Dictionary<string, ScriptableEnum>();

                // Add all ScriptableEnum assets in the resources folder to asset lists
                for (int i = 0; i < assets.Length; i++)
                {

#if UNITY_EDITOR
                    assets[i].EnsureCorrectName();

                    if (string.IsNullOrEmpty(assets[i]._guid))
                    {
                        assets[i].EnsureCorrectGUID();
                    }
#endif
                    assets[i]._assetIndex = -1;

                    Type type = assets[i].GetType();
                    List<ScriptableEnum> list;
                    if (_assetLists.TryGetValue(type, out list))
                    {
                        Assert.IsFalse(list.Contains(assets[i]));

                        list.Add(assets[i]);
                    }
                    else
                    {
                        _assetTypes.Add(type);
                        _assetLists.Add(type, new List<ScriptableEnum> { assets[i] });
                    }

#if UNITY_EDITOR
                    if (_assetsByGUID.TryGetValue(assets[i]._guid, out ScriptableEnum duplicate))
                    {
                        _assetsByGUID.Remove(assets[i]._guid);

                        duplicate.EnsureCorrectGUID();
                        assets[i].EnsureCorrectGUID();

                        _assetsByGUID.Add(assets[i]._guid, assets[i]);
                        _assetsByGUID.Add(duplicate._guid, duplicate);
                    }
                    else
                    {
                        _assetsByGUID.Add(assets[i]._guid, assets[i]);
                    }
#else
                    _assetsByGUID.Add(assets[i]._guid, assets[i]);
#endif

                }


                // Make sure when register any types that have no assets, but do have subtypes with assets
                Type scriptableEnumType = typeof(ScriptableEnum);
                List<Type> newAssetTypes = new List<Type>();
                for (int i = 0; i < _assetTypes.Count; i++)
                {
                    Type baseType = _assetTypes[i].BaseType;
                    while (baseType != scriptableEnumType && scriptableEnumType.IsAssignableFrom(baseType))
                    {
                        if (!newAssetTypes.Contains(baseType) && !_assetTypes.Contains(baseType))
                        {
                            newAssetTypes.Add(baseType);
                        }

                        baseType = baseType.BaseType;
                    }
                }

                for (int i = 0; i < newAssetTypes.Count; i++)
                {
                    _assetTypes.Add(newAssetTypes[i]);
                    _assetLists.Add(newAssetTypes[i], new List<ScriptableEnum>());
                }



                // Make sure we add all assets to lists of parent types
                for (int i = 0; i < assets.Length; i++)
                {
                    Type type = assets[i].GetType();
                    foreach (KeyValuePair<Type, List<ScriptableEnum>> assetList in _assetLists)
                    {
                        if (type != assetList.Key && assetList.Key.IsAssignableFrom(type))
                        {
                            assetList.Value.Add(assets[i]);
                        }
                    }
                }

                // Populate the name lookup dictionaries
                foreach (KeyValuePair<Type, List<ScriptableEnum>> assetList in _assetLists)
                {
                    Dictionary<string, ScriptableEnum> nameDictionary = new Dictionary<string, ScriptableEnum>();

                    for (int i = 0; i < assetList.Value.Count; i++)
                    {
                        ScriptableEnum asset = assetList.Value[i];
                        if (!nameDictionary.ContainsKey(asset.name))
                        {
                            nameDictionary.Add(asset.name, asset);
                        }
                        else
                        {
                            UnityEngine.Debug.LogError("Duplicate ScriptableEnum name: " + asset, asset);
                        }
                    }

                    _assetsByName.Add(assetList.Key, nameDictionary);
                }



                _assetsAreLoaded = true;
            }
        }

        public static T CreateRuntimeValue<T>(string name) where T : ScriptableEnum
        {
            Assert.IsTrue(Application.isPlaying);

            EnsureInstancesAreLoaded();

            Type type = typeof(T);
            T value = CreateInstance<T>();
            value.name = name;
            _assetsAreLoaded = true;

            List<ScriptableEnum> list;
            if (_assetLists.TryGetValue(type, out list))
            {
                list.Add(value);
            }
            else
            {
                _assetLists.Add(type, new List<ScriptableEnum> { value });
            }


            foreach (KeyValuePair<Type, List<ScriptableEnum>> assetList in _assetLists)
            {
                if (type != assetList.Key && assetList.Key.IsAssignableFrom(type))
                {
                    assetList.Value.Add(value);
                }
            }

            Dictionary<string, ScriptableEnum> nameDictionary;
            if (_assetsByName.TryGetValue(type, out nameDictionary))
            {
                if (!nameDictionary.ContainsKey(value.name))
                {
                    nameDictionary.Add(value.name, value);
                }
                else
                {
                    UnityEngine.Debug.LogError("Duplicate ScriptableEnum name: " + value, value);
                }
            }
            else
            {
                Dictionary<string, ScriptableEnum> newDictionary = new Dictionary<string, ScriptableEnum>();
                newDictionary.Add(value.name, value);
                _assetsByName.Add(type, newDictionary);
            }

            return value;
        }

        public static Type[] GetAllTypes()
        {
            EnsureInstancesAreLoaded();

            return _assetTypes.ToArray();
        }

        public static int GetCount(Type type)
        {
            Assert.IsTrue(typeof(ScriptableEnum).IsAssignableFrom(type));

            EnsureInstancesAreLoaded();

            List<ScriptableEnum> assets;
            if (_assetLists.TryGetValue(type, out assets))
            {
                return assets.Count;
            }

            return 0;
        }


        public static T GetRandomValue<T>() where T : ScriptableEnum
        {
            return (T)GetRandomValue(typeof(T));
        }


        public static ScriptableEnum GetRandomValue(Type type)
        {
            Assert.IsTrue(typeof(ScriptableEnum).IsAssignableFrom(type));

            EnsureInstancesAreLoaded();

            List<ScriptableEnum> assets;
            if (_assetLists.TryGetValue(type, out assets))
            {
                return assets[Random.Range(0, assets.Count)];
            }

            throw new ArgumentException("No ScriptableEnums of Type: " + type, nameof(type));
        }

        public static ScriptableEnum[] GetValues(Type type)
        {
            Assert.IsTrue(typeof(ScriptableEnum).IsAssignableFrom(type));

            EnsureInstancesAreLoaded();

            List<ScriptableEnum> assets;
            if (_assetLists.TryGetValue(type, out assets))
            {
                return assets.ToArray();
            }

            return new ScriptableEnum[0];
        }

        public static ScriptableEnum GetValue(Type type, int index)
        {
            Assert.IsTrue(typeof(ScriptableEnum).IsAssignableFrom(type));

            EnsureInstancesAreLoaded();

            List<ScriptableEnum> assets;
            if (_assetLists.TryGetValue(type, out assets))
            {
                return assets[index];
            }

            throw new ArgumentException("No ScriptableEnum values of type: " + type, nameof(type));
        }

        public static ScriptableEnum GetValue(Type type, string name)
        {
            Assert.IsTrue(typeof(ScriptableEnum).IsAssignableFrom(type));

            EnsureInstancesAreLoaded();

            Dictionary<string, ScriptableEnum> nameDictionary;
            if (_assetsByName.TryGetValue(type, out nameDictionary))
            {
                ScriptableEnum value;
                if (nameDictionary.TryGetValue(name, out value))
                {
                    return value;
                }

                throw new KeyNotFoundException("No ScriptableEnum value with name: " + name);
            }

            throw new ArgumentException("No ScriptableEnum values of type: " + type, nameof(type));
        }


        public static T[] GetValues<T>() where T : ScriptableEnum
        {
            EnsureInstancesAreLoaded();

            List<ScriptableEnum> assets;
            if (_assetLists.TryGetValue(typeof(T), out assets))
            {
                return assets.Cast<T>().ToArray();
            }

            return new T[0];
        }

        public static List<T> GetValueList<T>() where T : ScriptableEnum
        {
            EnsureInstancesAreLoaded();

            List<ScriptableEnum> assets;
            if (_assetLists.TryGetValue(typeof(T), out assets))
            {
                return assets.Cast<T>().ToList();
            }

            return new List<T>();
        }

        static int GetIndex(ScriptableEnum value)
        {
            Assert.IsNotNull(value, "ScriptableEnum value is null");

            EnsureInstancesAreLoaded();

            List<ScriptableEnum> list;
            if (_assetLists.TryGetValue(value.GetType(), out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == value) return i;
                }
            }

            throw new KeyNotFoundException("No ScriptableEnum value in asset list for type: " + value.GetType() + ", with value: " + value.name);
        }

        public static int GetCount<T>() where T : ScriptableEnum
        {
            return GetCount(typeof(T));
        }

        public static T GetValue<T>(int index) where T : ScriptableEnum
        {
            return (T)GetValue(typeof(T), index);
        }

        public static T GetValue<T>(string name) where T : ScriptableEnum
        {
            return (T)GetValue(typeof(T), name);
        }

        public static ScriptableEnum GetValueFromGUID(string guid)
        {
            EnsureInstancesAreLoaded();

            return _assetsByGUID[guid];
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            EnsureCorrectGUID();
            EnsureCorrectName();
        }

        void EnsureCorrectName()
        {

            string fileName = FileUtils.GetFileName(UnityEditor.AssetDatabase.GetAssetPath(this));
            if (fileName != name)
            {
                name = fileName;
                UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
                serializedObject.FindProperty("m_Name").stringValue = name;
                serializedObject.ApplyModifiedProperties();
            }
        }

        void EnsureCorrectGUID()
        {
            if (UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out string guid, out long localID))
            {
                if (_guid != guid)
                {
                    _guid = guid;
                    UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
                    serializedObject.FindProperty("_guid").stringValue = guid;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif

    }
}
