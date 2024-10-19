using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ross.EditorRuntimeCombatibility
{
        public static class Safe
        {
                #region Object Lifetime

                public static void Destroy(Object toDestroy)
                {
                        if (Safe.Application.IsPlaying)
                        {
                                Object.Destroy(toDestroy);
                        }
                        else
                        {
                                Object.DestroyImmediate(toDestroy);
                        }
                }

                public static T InstantiatePrefab<T>(T prefab, Vector3 position, Quaternion rotation)
                        where T : Component
                {
                        T instance = null;

                        if (Safe.Application.IsPlaying)
                        {
                                instance = Object.Instantiate(prefab, position, rotation);
                        }
                        else
                        {
#if UNITY_EDITOR
                                instance = (T)PrefabUtility.InstantiatePrefab(prefab);
                                instance.transform.position = position;
                                instance.transform.rotation = rotation;

#endif
                        }

                        return instance;
                }

                public static T InstantiatePrefab<T>(T prefab, Transform parent) where T : Component
                {
                        T instance = null;

                        if (Safe.Application.IsPlaying)
                        {
                                instance = Object.Instantiate(prefab, parent);
                        }
                        else
                        {
#if UNITY_EDITOR
                                instance = (T)PrefabUtility.InstantiatePrefab(prefab, parent);
#endif
                        }

                        return instance;
                }

                public static T InstantiatePrefab<T>(T prefab) where T : Component
                {
                        T instance = null;

                        if (Safe.Application.IsPlaying)
                        {
                                instance = Object.Instantiate(prefab);
                        }
                        else
                        {
#if UNITY_EDITOR
                                instance = (T)PrefabUtility.InstantiatePrefab(prefab);
#endif
                        }

                        return instance;
                }

                #endregion

                #region Time

#if UNITY_EDITOR
                [InitializeOnLoad]
#endif
                public static class Time
                {
                        static Time()
                        {
#if UNITY_EDITOR
                                EditorApplication.update += EditorUpdate;
#endif
                        }

                        private static float _editorLastTime;
                        private static float _editorDt;

                        private static void EditorUpdate()
                        {
#if UNITY_EDITOR
                                float currentTime = (float)EditorApplication.timeSinceStartup;
                                _editorDt = Mathf.Max(0, currentTime - _editorLastTime);
                                _editorLastTime = currentTime;
#endif
                        }

                        public static float deltaTime
                        {
                                get
                                {
                                        if (Safe.Application.IsPlaying)
                                        {
                                                return UnityEngine.Time.deltaTime;
                                        }

                                        if (_editorDt > 0)
                                        {
                                                return _editorDt;
                                        }

                                        //fallback dt for eg. before first editor update. Estimate for editor.
                                        return 1 / 30f;
                                }
                        }

                        public static float time
                        {
                                get
                                {
                                        if (Safe.Application.IsPlaying)
                                        {
                                                return UnityEngine.Time.time;
                                        }

#if UNITY_EDITOR
                                        return (float)EditorApplication.timeSinceStartup;
#endif
                                        return 0;
                                }
                        }
                }

                #endregion

                #region Application

                public static class Application
                {
                        private static Thread MainUnityThread = Thread.CurrentThread;
                        
                        public static bool IsPlaying
                        {
                                get
                                {
                                        bool isPlaying = false;

                                        if(Thread.CurrentThread == MainUnityThread && !SerializationTools.IsSerializing())
                                        {
                                                isPlaying = UnityEngine.Application.isPlaying;
                                        }

                                        return isPlaying;
                                }
                        }
                }

                #endregion
        }
}