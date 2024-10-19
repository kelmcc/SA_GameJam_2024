using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Framework
{
    /// <summary>
    /// Extension methods for miscellaneous types.
    /// </summary>
    public static class MiscExtensions
    {
        private static readonly Type _objectType = typeof(object);
        private static readonly Type _unityObjectType = typeof(Object);
        private static Vector3[] _cornerBuffer = new Vector3[4];

        public static FieldInfo[] GetFieldsWithAttribute<T>(this Type type, BindingFlags bindingFlags, bool inherit = false) where T : Attribute
        {
            List<FieldInfo> results = new List<FieldInfo>();
            Type attributeType = typeof(T);

            FieldInfo[] fields = type.GetFields(bindingFlags);
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].GetCustomAttributes(attributeType, inherit).Length > 0)
                {
                    results.Add(fields[i]);
                }
            }

            return results.ToArray();
        }


        public static FieldInfo GetFieldIncludingParentTypes(this Type type, string fieldName, BindingFlags bindingFlags)
        {
            Type objectType = typeof(object);

            if (type != objectType)
            {
                do
                {
                    FieldInfo field = type.GetField(fieldName, bindingFlags | BindingFlags.DeclaredOnly);
                    if (field != null)
                    {
                        return field;
                    }

                    type = type.BaseType;
                } while (type != objectType);
            }

            return null;
        }

        public static FieldInfo[] GetFieldsIncludingParentTypes(this Type type, BindingFlags bindingFlags, bool dontIncludeUnityObjectFields = true)
        {
            List<FieldInfo> fields = new List<FieldInfo>();


            if (type != _objectType && (!dontIncludeUnityObjectFields || type != _unityObjectType))
            {
                do
                {
                    fields.AddRange(type.GetFields(bindingFlags | BindingFlags.DeclaredOnly));
                    type = type.BaseType;
                } while (type != _objectType && (!dontIncludeUnityObjectFields || type != _unityObjectType));
            }

            return fields.ToArray();
        }

        public static PropertyInfo GetPropertyIncludingParentTypes(this Type type, string fieldName, BindingFlags bindingFlags)
        {
            Type objectType = typeof(object);

            if (type != objectType)
            {
                do
                {
                    PropertyInfo property = type.GetProperty(fieldName, bindingFlags | BindingFlags.DeclaredOnly);
                    if (property != null)
                    {
                        return property;
                    }

                    type = type.BaseType;
                } while (type != objectType);
            }

            return null;
        }

        public static PropertyInfo[] GetPropertiesIncludingParentTypes(this Type type, BindingFlags bindingFlags, bool dontIncludeUnityObjectFields = true)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();

            if (type != _objectType && (!dontIncludeUnityObjectFields || type != _unityObjectType))
            {
                do
                {
                    properties.AddRange(type.GetProperties(bindingFlags | BindingFlags.DeclaredOnly));
                    type = type.BaseType;
                } while (type != _objectType && (!dontIncludeUnityObjectFields || type != _unityObjectType));
            }

            return properties.ToArray();
        }

        public static bool HasAttribute<T>(this Type type, bool inherit = true) where T : Attribute
        {
            return type.GetCustomAttribute<T>(inherit) != null;
        }

        public static T GetAttribute<T>(this Type type, bool inherit = true) where T : Attribute
        {
            return type.GetCustomAttribute<T>(inherit);
        }




        public static bool HasAttribute<T>(this FieldInfo field, bool inherit = false) where T : Attribute
        {
            return field.GetCustomAttributes(typeof(T), inherit).Length > 0;
        }

        public static T GetAttribute<T>(this FieldInfo field, bool inherit = false) where T : Attribute
        {
            object[] attributes = field.GetCustomAttributes(typeof(T), inherit);
            if (attributes.Length > 0)
            {
                return (T)attributes[0];
            }

            return null;
        }

        public static bool IsCloseTo(this float value, float other, float tolerance = MathUtils.ROUNDING_ERROR_TOLERENCE)
        {
            return Mathf.Abs(value - other) <= tolerance;
        }

        public static BoundingSphere GetBoundingSphere(this Bounds bounds)
        {
            return new BoundingSphere(bounds.center, bounds.extents.magnitude);
        }

        public static float GetVolume(this Bounds bounds)
        {
            return bounds.size.x * bounds.size.y * bounds.size.z;
        }



        public static bool Raycast(this Plane plane, Ray ray, out Vector3 intersectionPoint)
        {
            float dist;
            intersectionPoint = Vector3.zero;

            if (plane.Raycast(ray, out dist))
            {
                intersectionPoint = ray.GetPoint(dist);
                return true;
            }
            return false;
        }

        public static int GetLayer(this RaycastHit hit)
        {
            return hit.collider.gameObject.layer;
        }

        public static bool HitColliderOnLayer(this RaycastHit hit, LayerMask layerMask)
        {
            return layerMask.Contains(hit.collider.gameObject.layer);
        }

        public static bool AreBytesTheSame(this byte[] bytes, byte[] otherBytes)
        {
            if (bytes == otherBytes) return true;
            if (bytes.Length != otherBytes.Length) return false;

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != otherBytes[i]) return false;
            }

            return true;
        }

        public static TemporaryRandomState UseTemporarily(this Random.State randomState)
        {
            return new TemporaryRandomState(randomState);
        }

        public static string ToStringUnformatted(this Guid guid)
        {
            return guid.ToString().ToLower().Replace("-", string.Empty);
        }

        public static int WithBitSet(this int value, int bitPosition, bool set)
        {
            return set ? value | 1 << bitPosition : value & ~(1 << bitPosition);
        }

        public static bool IsBitSet(this int value, int bitPosition)
        {
            return (value & (1 << bitPosition)) != 0;
        }

        public static void Emit(this ParticleSystem particleSystem, Vector3 position, Quaternion rotation, int count = 1)
        {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

            emitParams.position = position;
            emitParams.rotation3D = rotation.eulerAngles;
            emitParams.applyShapeToPosition = true;

            particleSystem.Emit(emitParams, count);

        }


        public static void SetEmissionEnabled(this ParticleSystem particleSystem, bool emit)
        {
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.enabled = emit;
        }

        public static bool HasArrivedAtDestination(this NavMeshAgent agent)
        {

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }

            return false;
        }




        public static void SetConstraint(this Rigidbody rigidbody, RigidbodyConstraints constraint, bool enabled)
        {
            if (enabled)
            {
                rigidbody.constraints |= constraint;
            }
            else
            {
                rigidbody.constraints &= ~constraint;
            }
        }

        public static bool GetConstraint(this Rigidbody rigidbody, RigidbodyConstraints constraint)
        {
            return ((1 << (int)constraint) & (int)rigidbody.constraints) > 0;
        }

        public static Rigidbody GetRootBody(this Rigidbody rigidbody)
        {
            Transform transform = rigidbody.transform.parent;
            while (transform != null)
            {
                Rigidbody body = transform.GetComponent<Rigidbody>();
                if (body != null)
                {
                    rigidbody = body;
                }

                transform = transform.parent;
            }
            return rigidbody;
        }

        public static void AddAlignmentTorque(this Rigidbody rigidbody, Vector3 targetForwardDirection, Vector3 targetUpDirection, float alignmentSpeed, float alignmentDamping, ForceMode forceMode)
        {
            Quaternion deltaRotation = Quaternion.Inverse(rigidbody.transform.rotation) * Quaternion.LookRotation(targetForwardDirection, targetUpDirection);
            Vector3 deltaAngles = deltaRotation.eulerAngles;

            if (deltaAngles.x > 180f) deltaAngles.x -= 360f;
            if (deltaAngles.y > 180f) deltaAngles.y -= 360f;
            if (deltaAngles.z > 180f) deltaAngles.z -= 360f;

            rigidbody.AddTorque(alignmentSpeed * rigidbody.transform.TransformDirection(deltaAngles) - alignmentDamping * rigidbody.angularVelocity);
        }

        public static void AddAlignmentTorque(this Rigidbody rigidbody, Vector3 targetDirection, float alignmentTorque, ForceMode forceMode)
        {
            Vector3 rotationAxis = Vector3.Cross(rigidbody.transform.forward, targetDirection.normalized);
            Vector3 angularVelocityChange = rotationAxis.normalized * Mathf.Asin(rotationAxis.magnitude);

            Quaternion worldSpaceTensor = rigidbody.transform.rotation * rigidbody.inertiaTensorRotation;
            Vector3 alignment = worldSpaceTensor * Vector3.Scale(rigidbody.inertiaTensor, Quaternion.Inverse(worldSpaceTensor) * angularVelocityChange);

            if (!alignment.HasNaNComponent())
            {
                rigidbody.AddTorque((alignment * alignmentTorque), forceMode);
            }
        }

        public static void AddAlignmentTorque(this Rigidbody rigidbody, Vector3 targetDirection, float alignmentTorque, float uprightingTorque, ForceMode forceMode)
        {
            Vector3 rotationAxis = Vector3.Cross(rigidbody.transform.forward, targetDirection.normalized);
            Vector3 angularVelocityChange = rotationAxis.normalized * Mathf.Asin(rotationAxis.magnitude);

            Quaternion worldSpaceTensor = rigidbody.transform.rotation * rigidbody.inertiaTensorRotation;
            Vector3 alignment = worldSpaceTensor * Vector3.Scale(rigidbody.inertiaTensor, Quaternion.Inverse(worldSpaceTensor) * angularVelocityChange);

            float uprightingAngle = rigidbody.transform.rotation.eulerAngles.z;
            uprightingAngle = uprightingAngle > 180 ? uprightingAngle - 360f : uprightingAngle;
            Vector3 uprighting = -rigidbody.transform.forward * (uprightingAngle / 90f);

            if (alignment.HasNaNComponent()) alignment = Vector3.zero;
            if (uprighting.HasNaNComponent()) uprighting = Vector3.zero;

            rigidbody.AddTorque((uprighting * uprightingTorque) + (alignment * alignmentTorque), forceMode);
        }

        public static bool LerpTowards(this Rigidbody rigidbody, Vector3 target, float speed, float maxDistance = Mathf.Infinity)
        {
            Vector3 toTarget = target - rigidbody.worldCenterOfMass;
            rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, toTarget * speed, maxDistance);

            return rigidbody.velocity.sqrMagnitude >= toTarget.sqrMagnitude;
        }

        public static bool LerpTowards(this Rigidbody rigidbody, Vector3 target, Vector3 worldPosition, float speed, float maxDistance = Mathf.Infinity)
        {
            Vector3 toTarget = target - worldPosition;
            rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, toTarget * speed, maxDistance);

            return rigidbody.velocity.sqrMagnitude >= toTarget.sqrMagnitude;
        }

        public static void AddForceAtLocalPosition(this Rigidbody rigidbody, Vector3 force, Vector3 localPosition, bool relativeToCenterOfMass, ForceMode forceMode = ForceMode.Force)
        {

            if (relativeToCenterOfMass)
            {
                rigidbody.AddForceAtPosition(force, rigidbody.transform.TransformPoint(localPosition + rigidbody.centerOfMass), forceMode);
            }

            rigidbody.AddForceAtPosition(force, rigidbody.transform.TransformPoint(localPosition), forceMode);
        }

        public static Vector2 GetDimensions(this TextMesh textMesh)
        {
            float currentLineWidth = 0;
            float currentLineheight = 0;
            float longestLineWidth = 0;
            float totalHeight = 0;
            int numLines = 1;
            CharacterInfo info;

            for (int i = 0; i < textMesh.text.Length; i++)
            {
                char c = textMesh.text[i];

                if (c == '\n')
                {
                    numLines++;
                    totalHeight += currentLineheight;
                    currentLineheight = 0;
                    currentLineWidth = 0;
                    continue;
                }

                if (textMesh.font.GetCharacterInfo(c, out info, textMesh.fontSize, textMesh.fontStyle))
                {
                    currentLineWidth += info.advance;
                    if (currentLineWidth > longestLineWidth)
                    {
                        longestLineWidth = currentLineWidth;
                    }
                    if (info.glyphHeight > currentLineheight)
                    {
                        currentLineheight = info.glyphHeight;
                    }
                }
            }
            totalHeight += currentLineheight;

            if (textMesh.lineSpacing < 1f)
            {
                return new Vector2(longestLineWidth, textMesh.font.lineHeight * ((numLines * textMesh.lineSpacing) + (1 - textMesh.lineSpacing))) * textMesh.characterSize * 0.1f;
            }

            //        return new Vector2(longestLineWidth, numLines * textMesh.font.lineHeight * textMesh.lineSpacing) * textMesh.characterSize * 0.1f;

            return new Vector2(longestLineWidth, totalHeight * textMesh.lineSpacing * 1.5f) * textMesh.characterSize * 0.1f;
        }


#if UNITY_EDITOR
        [HideInCallstack]
        public static void LogIfSelected(this Component component, object message, Object context = null)
        {
            if (UnityEditor.Selection.activeGameObject == component.gameObject) Debug.Log(message, context);
        }
#endif

        public static T GetComponentInParent<T>(this MonoBehaviour component, bool includeInactive) where T : Component
        {

            Transform current = component.transform;
            while (current != null)
            {
                Component[] components = current.GetComponents<Component>();

                if (current.gameObject.activeInHierarchy || includeInactive)
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] is T) return components[i] as T;
                    }
                }

                current = current.parent;
            }

            return null;
        }

        public static void ScrollToShow(this ScrollRect scrollRect, RectTransform rectTransform, float normalizedMargin = 0f)
        {
            void GetBounds(RectTransform rect, out float top, out float bottom)
            {
                rect.GetWorldCorners(_cornerBuffer);
                top = _cornerBuffer[1].y;
                bottom = _cornerBuffer[0].y;
            }


            GetBounds(scrollRect.content, out float contentTop, out float contentBottom);
            GetBounds(scrollRect.viewport, out float viewportTop, out float viewportBottom);
            GetBounds(rectTransform, out float itemTop, out float itemBottom);

            float viewportHeight = viewportTop - viewportBottom;
            float scrollHeight = contentTop - contentBottom - viewportHeight;

            if (itemTop > viewportTop)
            {
                scrollRect.verticalNormalizedPosition = 1f - ((contentTop - itemTop) / scrollHeight) + normalizedMargin;
            }
            else if (itemBottom < viewportBottom)
            {
                scrollRect.verticalNormalizedPosition = 1f - ((contentTop - (itemBottom + viewportHeight)) / scrollHeight) - normalizedMargin;
            }

        }


        public static Vector2 GetNormalizedPosition(this Rect rect, Vector2 position)
        {
            return new Vector2((position.x - rect.xMin) / rect.width, (position.y - rect.yMin) / rect.height);
        }

        public static bool ContainsCaseInsensitive(this string s, string substring)
        {
            return s.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Checks whether this object extends or implements a type.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <returns>True if the type is asignable from this object's type</returns>
        public static bool IsTypeOf<T>(this object obj)
        {
            return typeof(T).IsAssignableFrom(obj.GetType());
        }

        /// <summary>
        /// Returns whether or not the object is between two comparable bounds. Inclusive on the lower bound, exclusive on the upper one.
        /// </summary>
        /// <param name="lower">The lower bound</param>
        /// <param name="upper">The upper bound</param>
        /// <returns>Whether or not the object is between the bounds</returns>
        public static bool IsBetween<T>(this T actual, T lower, T upper) where T : IComparable<T>
        {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }

        public static bool IsBetween(this float actual, float lower, float upper, bool inclusive = true)
        {
            if (inclusive) return actual >= lower && actual <= upper;
            return actual > lower && actual < upper;
        }

        public static string GetPath(this GameObject gameObject)
        {
            if (gameObject == null)
            {
                return "NULL";
            }

            Transform transform = gameObject.transform;
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(gameObject))
            {
                return UnityEditor.AssetDatabase.GetAssetPath(gameObject) + "\n" + path;
            }
#endif


            return gameObject.scene.name + "/" + path;
        }

        public static Coroutine InvokeWhen(this MonoBehaviour mono, Action action, Func<bool> condition)
        {
            return mono.StartCoroutine(InvokeWhenRoutine(action, condition));
        }


        public static Coroutine InvokeRepeating(this MonoBehaviour mono, Action action, float delay, bool firstTimeIsRandom = false, bool useUnscaledTime = false)
        {
            return mono.StartCoroutine(InvokeRepeatingRoutine(action, delay, firstTimeIsRandom, useUnscaledTime));
        }

        public static Coroutine InvokeRandomly(this MonoBehaviour mono, Action action, float minDelay, float maxDelay, bool useUnscaledTime = false)
        {
            return mono.StartCoroutine(InvokeRandomlyRoutine(action, minDelay, maxDelay, useUnscaledTime));
        }

        public static Coroutine InvokeRandomly(this MonoBehaviour mono, Action action, FloatRange delayRange, bool useUnscaledTime = false)
        {
            return mono.StartCoroutine(InvokeRandomlyRoutine(action, delayRange.Min, delayRange.Max, useUnscaledTime));
        }

        public static Coroutine InvokeDelayed(this MonoBehaviour mono, float delay, Action action)
        {
            return mono.StartCoroutine(InvokeDelayedRoutine(action, delay, false));
        }

        public static Coroutine InvokeDelayedUnscaled(this MonoBehaviour mono, float delay, Action action)
        {
            return mono.StartCoroutine(InvokeDelayedRoutine(action, delay, true));
        }

        static IEnumerator InvokeRepeatingRoutine(Action action, float delay, bool firstTimeIsRandom, bool useUnscaledTime)
        {

            if (useUnscaledTime)
            {
                yield return new WaitForSecondsRealtime(firstTimeIsRandom ? delay * Random.value : delay);
            }
            else
            {
                yield return new WaitForSeconds(firstTimeIsRandom ? delay * Random.value : delay);
            }


            while (true)
            {
                action();

                if (useUnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(delay);
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }
            }
        }

        static IEnumerator InvokeRandomlyRoutine(Action action, float minDelay, float maxDelay, bool useUnscaledTime)
        {
            while (true)
            {
                if (useUnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(Random.Range(minDelay, maxDelay));
                }
                else
                {
                    yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
                }

                action();
            }
        }

        /// <summary>
        /// Will execute imediately if delay is 0
        /// </summary>
        static IEnumerator InvokeDelayedRoutine(Action action, float delay, bool useUnscaledTime)
        {
            if (delay == 0)
            {
                action();
            }
            else
            {

                if (useUnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(delay);
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }

                action();
            }
        }

        public static Coroutine InvokeFor(this MonoBehaviour behaviour, float duration, Action action)
        {
            return behaviour.StartCoroutine(InvokeForRoutine(duration, action));
        }

        static IEnumerator InvokeForRoutine(float duration, Action action)
        {
            float timer = 0f;

            while (timer < duration)
            {
                action();
                timer += Time.deltaTime;
                yield return null;
            }

            action();
        }

        public static Coroutine InvokeFor(this MonoBehaviour behaviour, float duration, Action<float> action)
        {
            return behaviour.StartCoroutine(InvokeForRoutine(duration, action));
        }

        static IEnumerator InvokeForRoutine(float duration, Action<float> action)
        {
            float timer = 0f;

            while (timer < duration)
            {
                action(timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            action(1f);
        }

        public static Coroutine InvokeForNormalized(this MonoBehaviour behaviour, float duration, Action<float> action)
        {
            return behaviour.StartCoroutine(InvokeForRoutine(duration, action));
        }

        static IEnumerator InvokeForNormalizedRoutine(float duration, Action<float> action)
        {
            float timer = 0f;

            while (timer < duration)
            {
                action(timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            action(1f);
        }


        static IEnumerator InvokeWhenRoutine(Action action, Func<bool> condition)
        {
            while (!condition())
            {
                yield return null;
            }

            action();
        }

        public static string GetPath(this RaycastHit hit)
        {
            return hit.collider.gameObject.GetPath();
        }

        public static string GetPath(this Component component)
        {
            return component.gameObject.GetPath();
        }

        public static void SetMaterial(this Renderer renderer, Material material, int index)
        {
            Material[] materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = i == index ? material : renderer.materials[i];
            }

            renderer.materials = materials;
        }

        public static void SetSharedMaterial(this Renderer renderer, Material material, int index)
        {
            Material[] materials = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = i == index ? material : renderer.sharedMaterials[i];
            }

            renderer.sharedMaterials = materials;
        }

        public static bool IsPointInInfluenceArea(this Light light, Vector3 point)
        {
            switch (light.type)
            {
                case LightType.Spot:
                    point = light.transform.InverseTransformPoint(point);
                    return Vector3.Angle(Vector3.forward, point) <= light.spotAngle * 0.5f && point.z <= light.range;
                case LightType.Directional: return true;
                case LightType.Point: return (point - light.transform.position).sqrMagnitude <= light.range * light.range;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static Vector3 GetContactPoint(this Collision collision)
        {
            return collision.GetContact(0).point;
        }


        public static Vector3 GetContactNormal(this Collision collision)
        {
            return collision.GetContact(0).normal;
        }

        public static void GetFirstContact(this Collision collision, out Vector3 point, out Vector3 normal)
        {
            ContactPoint contact = collision.GetContact(0);
            point = contact.point;
            normal = contact.normal;
        }

        public static Collider GetThisCollider(this Collision collision)
        {
            return collision.GetContact(0).thisCollider;
        }

        public static bool ColliderOnLayer(this Collision collision, LayerMask layerMask)
        {
            return layerMask.Contains(collision.collider.gameObject.layer);
        }

        public static GUIStyle WithFontSize(this GUIStyle stlye, int fontSize)
        {
            GUIStyle newStyle = new GUIStyle(stlye);
            newStyle.fontSize = fontSize;
            return newStyle;
        }

        [HideInCallstack]
        public static void LogDetails(this Collision collision, string additionalContext = null)
        {
            string debug = collision.collider.name + " x " + collision.GetContact(0).thisCollider.name;

            if (!string.IsNullOrEmpty(additionalContext))
            {
                debug += "\n" + additionalContext + "\n";
            }

            debug += "\n" + collision.collider.GetPath();
            debug += "\n" + collision.GetContact(0).thisCollider.GetPath();
            debug += "\n\nVelocity: " + collision.relativeVelocity.ToString("F3") + " (" + collision.relativeVelocity.magnitude.ToString("F3");
            debug += ")\nPosition: " + collision.contacts[0].point.ToString("F3");
            debug += "\nNormal: " + collision.contacts[0].normal.ToString("F3");

            Debug.Log(debug, collision.collider);
        }

        public static Vector3[] GetCorners(this Bounds bounds)
        {
            return new[]
            {
                new Vector3(bounds.min.x,bounds.min.y,bounds.min.z),
                new Vector3(bounds.min.x,bounds.min.y,bounds.max.z),
                new Vector3(bounds.max.x,bounds.min.y,bounds.max.z),
                new Vector3(bounds.max.x,bounds.min.y,bounds.min.z),
                new Vector3(bounds.min.x,bounds.max.y,bounds.min.z),
                new Vector3(bounds.min.x,bounds.max.y,bounds.max.z),
                new Vector3(bounds.max.x,bounds.max.y,bounds.max.z),
                new Vector3(bounds.max.x,bounds.max.y,bounds.min.z),
            };
        }




        /// <summary>
        /// Returns the value of the float squared.
        /// </summary>
        /// <returns>Value * value</returns>
        public static float Squared(this float f)
        {
            return f * f;
        }

        public static float ToFloat(this bool b)
        {
            return b ? 1f : 0f;
        }

        /// <summary>
        /// Checks whether this Type is a descendant of some other type.
        /// </summary>
        /// <param name="parentType">The base Type to check</param>
        /// <returns>True if this Type is assiagnable from the parentType</returns>
        public static bool IsTypeOf(this Type childType, Type parentType)
        {
            return parentType.IsAssignableFrom(childType);
        }

        /// <summary>
        /// Checks whether this Type is a descendant of some other type.
        /// </summary>
        /// <param name="T">The base Type to check</param>
        /// <returns>True if this Type is assiagnable from the parentType</returns>
        public static bool IsTypeOf<T>(this Type childType)
        {
            return typeof(T).IsAssignableFrom(childType);
        }

        public static Type[] GetAllSubtypesInUnityAssemblies(this Type type, bool includeAbstractType = false, bool includeGenericTypes = false)
        {
            List<Type> subtypes = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                if (!assemblies[i].FullName.StartsWith("Assembly-CSharp")) continue;

                Type[] types = assemblies[i].GetTypes();

                for (int j = 0; j < types.Length; j++)
                {
                    if (type.IsAssignableFrom(types[j]) && types[j] != type && (includeAbstractType || !types[j].IsAbstract) && (includeGenericTypes || !types[j].IsGenericType))
                    {
                        subtypes.Add(types[j]);
                    }
                }
            }


            return subtypes.ToArray();
        }

        public static Type[] GetAllSubtypesInCurrentAssembly(this Type type, bool includeAbstractType = false, bool includeGenericTypes = false)
        {
            List<Type> subtypes = new List<Type>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                if (type.IsAssignableFrom(types[i]) && types[i] != type && (includeAbstractType || !types[i].IsAbstract) && (includeGenericTypes || !types[i].IsGenericType))
                {
                    subtypes.Add(types[i]);
                }
            }

            return subtypes.ToArray();
        }
        public static GUIContent WithTooltip(this GUIContent content, string tooltip)
        {
            content.tooltip = tooltip;
            return content;
        }

        [HideInCallstack]
        public static void LogSubsystems(this PlayerLoopSystem system)
        {
            string GetString(PlayerLoopSystem subSystem, int depth, int index)
            {
                string text = "";

                for (int i = 0; i < depth; i++)
                {
                    text += "\t";
                }

                if (subSystem.type == null)
                {
                    depth--;
                }
                else
                {
                    text += index + ": " + subSystem.type.Name;
                }


                if (subSystem.subSystemList != null)
                {
                    for (int i = 0; i < subSystem.subSystemList.Length; i++)
                    {
                        text += "\n" + GetString(subSystem.subSystemList[i], depth + 1, i);
                    }
                }

                return text;
            }

            Debug.Log(GetString(system, 0, 0).Trim());
        }

        public static void RemoveSubstem(this ref PlayerLoopSystem system, int index)
        {

            PlayerLoopSystem[] newList = new PlayerLoopSystem[system.subSystemList.Length - 1];
            for (int i = 0; i < newList.Length; i++)
            {
                newList[i] = system.subSystemList[i >= index ? i + 1 : i];
            }

            system.subSystemList = newList;
        }

        /// <summary>
        /// Destroy the object immediately or delayed based on current editor playing state.
        /// </summary>
        public static void SmartDestroy(this UnityEngine.Object target)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                UnityEngine.Object.Destroy(target);
            else
                UnityEngine.Object.DestroyImmediate(target);
#else
        UnityEngine.Object.Destroy (target);
#endif
        }

#if UNITY_EDITOR

        public static string[] ToStringArray(this IList<GUIContent> labels)
        {
            string[] strings = new string[labels.Count];

            for (int i = 0; i < strings.Length; i++)
            {
                strings[i] = labels[i].text;
            }

            return strings;
        }




        public static void CenterInScreen(this UnityEditor.EditorWindow window)
        {
            window.position = new Rect((Screen.currentResolution.width - window.position.width) * 0.5f, (Screen.currentResolution.height - window.position.height) * 0.5f, window.position.width, window.position.height);
        }

        public static void CenterInScreen(this UnityEditor.EditorWindow window, float width, float height)
        {
            window.position = new Rect((Screen.currentResolution.width - width) * 0.5f, (Screen.currentResolution.height - height) * 0.5f, width, height);
        }

        public static object GetPropertyValue(this Material material, int nameID, ShaderPropertyType type)
        {
            switch (type)
            {
                case ShaderPropertyType.Range:
                case ShaderPropertyType.Float: return material.GetFloat(nameID);
                case ShaderPropertyType.Color: return material.GetColor(nameID);
                case ShaderPropertyType.Vector: return material.GetVector(nameID);
                case ShaderPropertyType.Texture: return material.GetTexture(nameID);
            }

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }



#endif

    }
}
