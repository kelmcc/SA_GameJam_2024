using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Framework
{
    /// <summary>
    /// Class that makes it possible to serialize a reference to a scene object. Note that it will be serialized as a string name in builds.
    /// </summary>
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver, IEquatable<SceneReference>
    {
        /// <summary>
        /// Whether or not this contains a valid reference to a scene.
        /// </summary>
        public bool IsValid
        {
            get
            {
#if UNITY_EDITOR
                return _sceneAsset != null;
#else
            return !string.IsNullOrEmpty(_sceneName);
#endif
            }
        }

        /// <summary>
        /// The name of the scene.
        /// </summary>
        public string SceneName => _sceneName;

#if UNITY_EDITOR

        public UnityEditor.SceneAsset EditorSceneAsset => _sceneAsset;

        public string GetEditorAssetPath()
        {
            return UnityEditor.AssetDatabase.GetAssetPath(_sceneAsset);
        }

        [SerializeField]
        private UnityEditor.SceneAsset _sceneAsset;
#endif

        [SerializeField]
        private string _sceneName;

        /// <summary>
        /// Loads the scene.
        /// </summary>
        /// <param name="mode">The scene loading mode to use</param>
        public void Load(LoadSceneMode mode = LoadSceneMode.Single)
        {
            Assert.IsTrue(IsValid, "Invalid scene");

            SceneManager.LoadScene(_sceneName, mode);
        }

        /// <summary>
        /// Loads the scene asynchronously.
        /// </summary>
        /// <param name="mode">The scene loading mode to use</param>
        public AsyncOperation LoadAsync(LoadSceneMode mode = LoadSceneMode.Single)
        {
            Assert.IsTrue(IsValid, "Invalid scene");

            return SceneManager.LoadSceneAsync(_sceneName, mode);
        }

        /// <summary>
        /// Sets this secne to be active
        /// </summary>
        /// <returns>True if the reference is valid and the scene is loaded</returns>
        public bool SetAsActiveScene()
        {
            if (IsValid)
            {
                return SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
            }
            return false;
        }

        public bool IsLoaded()
        {
            Assert.IsTrue(IsValid, "Invalid scene");

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
#if UNITY_EDITOR
                if (SceneManager.GetSceneAt(i).path == GetEditorAssetPath()) return true;
#else
            if (SceneManager.GetSceneAt(i).name == _sceneName) return true;
#endif
            }

            return false;
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (_sceneAsset == null)
            {
                _sceneName = string.Empty;
            }
            else
            {
                if (string.IsNullOrEmpty(_sceneAsset.name))
                {
                    _sceneName = System.IO.Path.GetFileNameWithoutExtension(UnityEditor.AssetDatabase.GetAssetPath(_sceneAsset));
                }
                else
                {
                    _sceneName = _sceneAsset.name;
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {

        }

        public bool Equals(SceneReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
#if UNITY_EDITOR
            return Equals(_sceneAsset, other._sceneAsset);
#else
            return _sceneName == other._sceneName;
#endif
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SceneReference)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
#if UNITY_EDITOR
                return _sceneAsset != null ? _sceneAsset.GetHashCode() : 0;
#else
            return _sceneName != null ? _sceneName.GetHashCode() : 0;
#endif
            }
        }

        public static bool operator ==(SceneReference left, SceneReference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SceneReference left, SceneReference right)
        {
            return !Equals(left, right);
        }
    }
}
