using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Framework
{
    public enum GetBoundsMode
    {
        RenderersOnly,
        CollidersOnly,
        TriggersOnly,
        CollidersAndTriggers,
        CollidersAndRenderers,
        RenderersAndTriggers,
        CollidersTriggerAndRenderers,
    }

    public enum GameObjectLabelIcon
    {
        Gray = 0,
        Blue,
        Teal,
        Green,
        Yellow,
        Orange,
        Red,
        Purple
    }

    public enum GameObjectShapeIcon
    {
        CircleGray = 0,
        CircleBlue,
        CircleTeal,
        CircleGreen,
        CircleYellow,
        CircleOrange,
        CircleRed,
        CirclePurple,
        DiamondGray,
        DiamondBlue,
        DiamondTeal,
        DiamondGreen,
        DiamondYellow,
        DiamondOrange,
        DiamondRed,
        DiamondPurple
    }

    /// <summary>
    /// Extension methods for GameObjects.
    /// </summary>
    public static class GameObjectExtensions
    {

        public static GameObject[] GetChildObjects(this GameObject gameObject, bool includeThisGameObject = false)
        {
            List<GameObject> objects = new List<GameObject>();

            if (includeThisGameObject)
            {
                objects.Add(gameObject);
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform[] children = gameObject.transform.GetChild(i).GetComponentsInChildren<Transform>(true);
                for (int j = 0; j < children.Length; j++)
                {
                    objects.Add(children[j].gameObject);
                }
            }

            return objects.ToArray();
        }

        public static GameObject[] GetChildObjectsOnLayer(this GameObject gameObject, int layer, bool includeThisGameObject = false)
        {
            List<GameObject> objects = new List<GameObject>();

            if (includeThisGameObject && gameObject.layer == layer)
            {
                objects.Add(gameObject);
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform[] children = gameObject.transform.GetChild(i).GetComponentsInChildren<Transform>(true);
                for (int j = 0; j < children.Length; j++)
                {
                    if (children[j].gameObject.layer == layer)
                    {
                        objects.Add(children[j].gameObject);
                    }
                }
            }

            return objects.ToArray();
        }


        public static GameObject[] GetChildObjectsOnLayers(this GameObject gameObject, LayerMask layerMask, bool includeThisGameObject = false)
        {
            List<GameObject> objects = new List<GameObject>();

            if (includeThisGameObject && layerMask.Contains(gameObject.layer))
            {
                objects.Add(gameObject);
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform[] children = gameObject.transform.GetChild(i).GetComponentsInChildren<Transform>(true);
                for (int j = 0; j < children.Length; j++)
                {
                    if (layerMask.Contains(children[j].gameObject.layer))
                    {
                        objects.Add(children[j].gameObject);
                    }
                }
            }

            return objects.ToArray();
        }

        public static bool SceneExists(this GameObject gameObject)
        {
            return gameObject.scene.IsValid() && gameObject.scene.isLoaded && !Runtime.IsShuttingDown;
        }

        public static bool IsInPrefabStage(this GameObject gameObject)
        {
#if UNITY_EDITOR
            UnityEditor.SceneManagement.PrefabStage stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null && stage.IsPartOfPrefabContents(gameObject))
            {
                return true;
            }
#endif

            return false;
        }

        public static bool IsPrefabAsset(this GameObject gameObject)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUtility.IsPersistent(gameObject)) return true;
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) != null) return true;

            return false;
#else
        return !gameObject.scene.IsValid();
#endif
        }

        public static void ClearIcon(this GameObject go)
        {
#if UNITY_EDITOR
            IconUtility.SetIcon(go, (Texture2D)null);
#endif
        }

        public static void SetIcon(this GameObject go, Texture2D texture)
        {
#if UNITY_EDITOR
            IconUtility.SetIcon(go, texture);
#endif
        }

        public static void SetIcon(this GameObject go, Sprite sprite)
        {
#if UNITY_EDITOR
            IconUtility.SetIcon(go, sprite);
#endif
        }

        public static void SetIcon(this GameObject go, GameObjectLabelIcon icon)
        {
#if UNITY_EDITOR
            IconUtility.SetIcon(go, icon);
#endif
        }

        public static void SetIcon(this GameObject go, GameObjectShapeIcon icon)
        {
#if UNITY_EDITOR
            IconUtility.SetIcon(go, icon);
#endif
        }

        public static void SetActive(this IList<GameObject> gameObjects, bool active)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].SetActive(active);
            }
        }

        public static void SetChildrenActive(this GameObject go, bool active)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                go.transform.GetChild(i).gameObject.SetActive(active);
            }
        }

        public static bool HasChildOnLayer(this GameObject obj, int layer, bool searchChildrenOnly = false)
        {
            if (!searchChildrenOnly && obj.layer == layer) return true;

            Transform[] children = obj.GetComponentsInChildrenOnly<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].gameObject.layer == layer) return true;
            }

            return false;
        }



        public static string GetLayerName(this GameObject go)
        {
            return LayerMask.LayerToName(go.layer);
        }

        public static BoundingSphere GetBoundingSphere(this GameObject go, GetBoundsMode mode = GetBoundsMode.CollidersAndRenderers)
        {
            return GetBounds(go, mode).GetBoundingSphere();
        }

        public static Bounds GetBounds(this GameObject go, GetBoundsMode mode = GetBoundsMode.CollidersAndRenderers)
        {
            Bounds bounds = new Bounds();

            bool includeRenderers = mode == GetBoundsMode.CollidersAndRenderers || mode == GetBoundsMode.RenderersOnly || mode == GetBoundsMode.RenderersAndTriggers || mode == GetBoundsMode.CollidersTriggerAndRenderers;
            bool includeColliders = mode == GetBoundsMode.CollidersAndRenderers || mode == GetBoundsMode.CollidersOnly || mode == GetBoundsMode.CollidersAndTriggers || mode == GetBoundsMode.CollidersTriggerAndRenderers;
            bool includeTriggers = mode == GetBoundsMode.CollidersAndTriggers || mode == GetBoundsMode.TriggersOnly || mode == GetBoundsMode.RenderersAndTriggers || mode == GetBoundsMode.CollidersTriggerAndRenderers;

            if (includeRenderers)
            {
                Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].enabled && (renderers[i].gameObject.activeInHierarchy || go.IsPrefabAsset()))
                    {


                        if (renderers[i].bounds.extents == Vector3.zero)
                        {
                            /*  if (!(renderers[i] is ParticleSystemRenderer))
                             {
                                 Debug.LogError("Renderer has invalid bounds: " + renderers[i].name + " (" + renderers[i].GetType() + ")\n" + renderers[i].GetPath(), renderers[i]);
                             }
                              */

                            continue;
                        }


                        if (bounds.extents == Vector3.zero)
                        {
                            bounds = renderers[i].bounds;
                        }
                        else
                        {
                            bounds.Encapsulate(renderers[i].bounds);
                        }
                    }
                }
            }

            if (includeTriggers || includeColliders)
            {
                Collider[] colliders = go.GetComponentsInChildren<Collider>();

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].enabled && colliders[i].gameObject.activeInHierarchy && ((colliders[i].isTrigger && includeTriggers) || (!colliders[i].isTrigger && includeColliders)))
                    {
                        if (colliders[i].bounds.extents == Vector3.zero)
                        {
                            Debug.LogError("Collider has invalid bounds: " + colliders[i].name + "\n" + colliders[i].GetPath(), colliders[i]);
                            continue;
                        }

                        if (bounds.extents == Vector3.zero)
                        {
                            bounds = colliders[i].bounds;
                        }
                        else
                        {
                            bounds.Encapsulate(colliders[i].bounds);
                        }
                    }
                }
            }

            return bounds;
        }


        /// <summary>
        /// Sets the layer of the GameObject and all its children
        /// </summary>
        /// <param name="layer">The new layer</param>
        public static void SetLayerRecursively(this GameObject go, int layer)
        {
            if (go.layer != layer)
            {
                go.layer = layer;
            }

            for (int i = 0; i < go.transform.childCount; i++)
            {
                go.transform.GetChild(i).gameObject.SetLayerRecursively(layer);
            }
        }

        public static void ChangeLayerRecursively(this GameObject go, LayerMask fromLayers, int toLayer)
        {
            if (go.layer != toLayer && fromLayers.Contains(go.layer))
            {
                go.layer = toLayer;
            }

            for (int i = 0; i < go.transform.childCount; i++)
            {
                go.transform.GetChild(i).gameObject.ChangeLayerRecursively(fromLayers, toLayer);
            }
        }

        /// <summary>
        /// Checks whether this object is on one of the layers contained in a layer mask.
        /// </summary>
        /// <param name="mask">The layer mask to check</param>
        /// <returns>True if the object is on a layer in the mask</returns>
        public static bool IsInLayerMask(this GameObject go, LayerMask mask)
        {
            return ((1 << go.layer) & mask.value) > 0;
        }

        public static bool IsSelectedInHierarchy(this GameObject go, bool includeChildren = false, bool includeParents = false)
        {
#if UNITY_EDITOR
            if (includeChildren || includeParents)
            {
                GameObject[] objects = UnityEditor.Selection.gameObjects;
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] == go) return true;
                    if (includeChildren && SceneUtils.IsAncestor(go.transform, objects[i].transform)) return true;
                    if (includeParents && SceneUtils.IsAncestor(objects[i].transform, go.transform)) return true;
                }
            }

            return UnityEditor.Selection.Contains(go);
#else
            return false;
#endif
        }

        /// <summary>
        /// Finds all the components of a type that are in the GameObject's children, but not on the object itself.
        /// </summary>
        /// <returns>An array of components</returns>
        public static T[] GetComponentsInChildrenOnly<T>(this GameObject go, bool includeInactive = false)
        {
            List<T> childComponents = new List<T>();
            for (int i = 0; i < go.transform.childCount; i++)
            {
                childComponents.AddRange(go.transform.GetChild(i).GetComponentsInChildren<T>(includeInactive));
            }

            return childComponents.ToArray();
        }

        /// <summary>
        /// Finds the first component of a type that is in the GameObject's children, but not on the object itself.
        /// </summary>
        /// <returns>The first child component of the type specified</returns>
        public static T GetComponentInChildrenOnly<T>(this GameObject go, bool includeInactive = false)
        {

            for (int i = 0; i < go.transform.childCount; i++)
            {
                T component = go.transform.GetChild(i).GetComponentInChildren<T>(includeInactive);
                if (component != null && !component.Equals(null)) return component;
            }

            return default;
        }

        public static T GetComponentInImmediateChildren<T>(this GameObject go, bool includeInactive = false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform child = go.transform.GetChild(i);
                if (includeInactive || child.gameObject.activeInHierarchy)
                {

                    T component = child.GetComponent<T>();

                    if (component != null && !component.Equals(null))
                    {
                        return component;
                    }
                }
            }

            return default;
        }


        public static T GetComponentInParentOnly<T>(this GameObject go, bool includeInactive = false)
        {

            Transform parent = go.transform.parent;
            while (parent != null && (includeInactive || parent.gameObject.activeInHierarchy))
            {
                T component = parent.gameObject.GetComponent<T>();
                if (component != null && !component.Equals(null))
                {
                    return component;
                }

                parent = parent.parent;
            }

            return default;
        }


        public static T[] GetComponentsInParentOnly<T>(this GameObject go, bool includeInactive = false)
        {
            Transform parent = go.transform.parent;
            if (parent != null && (includeInactive || parent.gameObject.activeInHierarchy))
            {
                return parent.GetComponentsInParent<T>(includeInactive);
            }

            return new T[0];
        }

        /// <summary>
        /// Gets a component if it exists on the object, otherwise adds one.
        /// </summary>
        /// <typeparam name="T">The component type to check</typeparam>
        /// <returns>The component on the object</returns>
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        public static T GetComponent<T>(this GameObject go, Predicate<T> predicate)
        {
            T[] components = go.GetComponents<T>();

            for (int i = 0; i < components.Length; i++)
            {
                if (predicate(components[i])) return components[i];
            }

            return default;
        }

        public static T GetComponentsInChildren<T>(this GameObject go, Predicate<T> predicate, bool includeInactive = false)
        {
            T[] components = go.GetComponentsInChildren<T>(includeInactive);

            for (int i = 0; i < components.Length; i++)
            {
                if (predicate(components[i])) return components[i];
            }

            return default;
        }

        public static void SetActive(this IEnumerable<GameObject> gameObjects, bool active)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.SetActive(active);
            }
        }

        public static void SetGameObjectsActive(this IEnumerable<Component> components, bool active)
        {
            foreach (Component component in components)
            {
                component.gameObject.SetActive(active);
            }
        }

        public static void SetEnabled(this IEnumerable<Behaviour> behaviours, bool enabled)
        {
            foreach (Behaviour behaviour in behaviours)
            {
                behaviour.enabled = enabled;
            }
        }


        [Conditional("UNITY_EDITOR")]
        public static void ReorderComponents(this GameObject gameObject)
        {
            ReorderComponents(gameObject, null, DefaultComponentComparison);
        }

        [Conditional("UNITY_EDITOR")]
        public static void ReorderComponents(this GameObject gameObject, Comparison<Component> comparisonFunction)
        {
            ReorderComponents(gameObject, null, comparisonFunction);
        }

        [Conditional("UNITY_EDITOR")]
        public static void ReorderComponents(this GameObject gameObject, Type[] componentPriority)
        {
            ReorderComponents(gameObject, componentPriority, DefaultComponentComparison);
        }

        [Conditional("UNITY_EDITOR")]
        public static void ReorderComponents(this GameObject gameObject, Type[] componentPriority, Comparison<Component> comparisonFunction)
        {
#if UNITY_EDITOR
            int componentCount = gameObject.GetComponents<Component>().Length;

            for (int j = 0; j <= componentCount - 2; j++)
            {
                for (int i = 0; i <= componentCount - 2; i++)
                {
                    Component a = gameObject.GetComponents<Component>()[i];
                    Component b = gameObject.GetComponents<Component>()[i + 1];

                    if (componentPriority == null)
                    {
                        if (comparisonFunction(a, b) > 0)
                        {
                            UnityEditorInternal.ComponentUtility.MoveComponentDown(a);
                        }
                    }
                    else
                    {
                        int indexA = GetComponentOrderIndex(a, componentPriority);
                        int indexB = GetComponentOrderIndex(b, componentPriority);

                        if (indexA >= 0 && indexB >= 0 && indexA > indexB)
                        {
                            UnityEditorInternal.ComponentUtility.MoveComponentDown(a);
                        }
                        else if (indexA < 0 && indexB >= 0)
                        {
                            UnityEditorInternal.ComponentUtility.MoveComponentDown(a);
                        }
                        else if (indexA < 0 && indexB < 0 && comparisonFunction(a, b) > 0)
                        {
                            UnityEditorInternal.ComponentUtility.MoveComponentDown(a);
                        }
                    }
                }
            }
#endif
        }

        static int GetComponentOrderIndex(Component component, Type[] componentOrder)
        {
            Type type = component.GetType();

            for (int i = 0; i < componentOrder.Length; i++)
            {
                if (componentOrder[i] == type) return i;
            }

            for (int i = 0; i < componentOrder.Length; i++)
            {
                if (componentOrder[i].IsAssignableFrom(type)) return i;
            }

            return -1;
        }

        static int DefaultComponentComparison(Component x, Component y)
        {
            if (x.IsTypeOf<Transform>()) return 1;
            if (y.IsTypeOf<Transform>()) return -1;

            bool xIsMonoBehaviour = x.IsTypeOf<MonoBehaviour>();
            bool yIsMonoBehaviour = y.IsTypeOf<MonoBehaviour>();

            if (yIsMonoBehaviour && !xIsMonoBehaviour) return -1;
            if (xIsMonoBehaviour && !yIsMonoBehaviour) return 1;

            return x.GetType().Name.CompareTo(y.GetType().Name);
        }



#if UNITY_EDITOR
        static class IconUtility
        {

            private static GUIContent[] _labelIcons;
            private static GUIContent[] _largeIcons;
            private static MethodInfo _setIconMethod;

            public static void SetIcon(GameObject go, Sprite sprite)
            {

                Texture2D croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] pixels = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
                croppedTexture.SetPixels(pixels);
                croppedTexture.Apply();

                SetIcon(go, croppedTexture);
            }

            public static void SetIcon(GameObject go, Texture2D texture)
            {
                if (_setIconMethod == null)
                {
                    _setIconMethod = typeof(UnityEditor.EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
                }

                _setIconMethod.Invoke(null, new object[] { go, texture });
            }

            public static void SetIcon(GameObject go, GameObjectLabelIcon icon)
            {
                if (_labelIcons == null)
                {
                    _labelIcons = GetTextures("sv_label_", string.Empty, 0, 8);
                }

                SetIcon(go, _labelIcons[(int)icon].image as Texture2D);
            }

            public static void SetIcon(GameObject go, GameObjectShapeIcon icon)
            {

                if (_largeIcons == null)
                {
                    _largeIcons = GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
                }

                SetIcon(go, _largeIcons[(int)icon].image as Texture2D);
            }

            private static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
            {
                GUIContent[] guiContentArray = new GUIContent[count];
                MethodInfo iconMethod = typeof(UnityEditor.EditorGUIUtility).GetMethod("IconContent", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);

                for (int index = 0; index < count; ++index)
                {
                    guiContentArray[index] = iconMethod.Invoke(null, new object[] { baseName + (object)(startIndex + index) + postFix }) as GUIContent;
                }

                return guiContentArray;
            }

        }
#endif
    }
}