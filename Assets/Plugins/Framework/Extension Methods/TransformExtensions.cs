using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Extension methods for transforms.
    /// </summary>
    public static class TransformExtensions
    {

        public static int GetDepth(this Transform transform)
        {
            int depth = 0;
            Transform parent = transform.parent;
            while (parent != null)
            {
                parent = parent.parent;
                depth++;
            }

            return depth;
        }

        public static Transform GetFirstUniformlyScaledParent(this Transform transform, float tolerance)
        {
            while (transform != null && !IsUniformlyScaled(transform, tolerance))
            {
                transform = transform.parent;
            }

            return transform;
        }

        public static Transform GetFirstUniformlyScaledParent(this Transform transform)
        {
            return GetFirstUniformlyScaledParent(transform, Mathf.Epsilon);
        }

        public static bool IsUniformlyScaled(this Transform transform, float tolerance)
        {
            return Mathf.Abs(transform.lossyScale.x - transform.lossyScale.y) < tolerance && Mathf.Abs(transform.lossyScale.x - transform.lossyScale.z) < tolerance;
        }

        public static bool IsUniformlyScaled(this Transform transform)
        {
            return IsUniformlyScaled(transform, Mathf.Epsilon);
        }

        public static void SetTRS(this Transform transform, Matrix4x4 matrix)
        {
            transform.position = matrix.GetTranslation();
            transform.rotation = matrix.GetRotation();
            transform.localScale = matrix.GetScale();
        }

        public static void SetPositionAndRotation(this Transform transform, Vector3 position, Quaternion rotation, bool dontChangeChildren)
        {
            if (dontChangeChildren)
            {
                Vector3 positionalOffset = position - transform.position;
                Quaternion rotationalOffset = rotation * Quaternion.Inverse(transform.rotation);

                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    child.position = child.position - positionalOffset;
                    child.rotation = Quaternion.Inverse(rotationalOffset) * child.rotation;
                }

            }

            transform.SetPositionAndRotation(position, rotation);
        }

        public static void DestroyChildren(this Transform transform)
        {
            Transform[] children = transform.GetChildren();
            for (int i = 0; i < children.Length; i++)
            {
                children[i].gameObject.SmartDestroy();
            }
        }

        /// <summary>
        /// Increases the transform's X position.
        /// </summary>
        /// <param name="amount">The amount to add</param>
        public static void AddX(this Transform transform, float amount)
        {
            transform.position = new Vector3(transform.position.x + amount, transform.position.y, transform.position.z);
        }

        /// <summary>
        /// Increases the transform's X position.
        /// </summary>
        /// <param name="amount">The amount to add</param>
        public static void AddY(this Transform transform, float amount)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + amount, transform.position.z);
        }

        /// <summary>
        /// Increases the transform's X position.
        /// </summary>
        /// <param name="amount">The amount to add</param>
        public static void AddZ(this Transform transform, float amount)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + amount);
        }

        /// <summary>
        /// Sets the transform's global X position.
        /// </summary>
        /// <param name="value">The new position</param>
        public static void SetX(this Transform transform, float value)
        {
            transform.position = new Vector3(value, transform.position.y, transform.position.z);
        }

        /// <summary>
        /// Sets the transform's global Y position.
        /// </summary>
        /// <param name="value">The new position</param>
        public static void SetY(this Transform transform, float value)
        {
            transform.position = new Vector3(transform.position.x, value, transform.position.z);
        }

        /// <summary>
        /// Sets the transform's global Z position.
        /// </summary>
        /// <param name="value">The new position</param>
        public static void SetZ(this Transform transform, float value)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, value);
        }

        /// <summary>
        /// Sets the transform's local X position.
        /// </summary>
        /// <param name="value">The new position</param>
        public static void SetLocalX(this Transform transform, float value)
        {
            transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z);
        }

        /// <summary>
        /// Sets the transform's local Y position.
        /// </summary>
        /// <param name="value">The new position</param>
        public static void SetLocalY(this Transform transform, float value)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, value, transform.localPosition.z);
        }

        /// <summary>
        /// Sets the transform's local Z position.
        /// </summary>
        /// <param name="value">The new position</param>
        public static void SetLocalZ(this Transform transform, float value)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value);
        }

        /// <summary>
        /// Sets the transform's local scale on the X axis.
        /// </summary>
        /// <param name="value">The new scale</param>
        public static void SetScaleX(this Transform transform, float value)
        {
            transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);
        }

        /// <summary>
        /// Sets the transform's local scale on the Y axis.
        /// </summary>
        /// <param name="value">The new scale</param>
        public static void SetScaleY(this Transform transform, float value)
        {
            transform.localScale = new Vector3(transform.localScale.x, value, transform.localScale.z);
        }

        /// <summary>
        /// Sets the transform's local scale on the Z axis.
        /// </summary>
        /// <param name="value">The new scale</param>
        public static void SetScaleZ(this Transform transform, float value)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, value);
        }

        /// <summary>
        /// Sets the transform's local scale on the all axes.
        /// </summary>
        /// <param name="value">The new position</param>
        public static void SetScale(this Transform transform, float value)
        {
            transform.localScale = new Vector3(value, value, value);
        }

        public static void SetLossyScale(this Transform transform, Vector3 scale)
        {
            transform.localScale = new Vector3(scale.x / transform.lossyScale.x, scale.y / transform.lossyScale.y, scale.z / transform.lossyScale.z);
        }

        /// <summary>
        /// Resets the local transform to its default values.
        /// </summary>
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static Matrix4x4 GetTRSMatrix(this Transform transform)
        {
            return Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        }

        public static Matrix4x4 GetLocalTRSMatrix(this Transform transform)
        {
            return Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        }



        public static Transform[] GetChildren(this Transform transform, bool includeRoot = false, bool includeDeeperChildren = false)
        {
            if (includeDeeperChildren)
            {
                if (includeRoot)
                {
                    return transform.GetComponentsInChildren<Transform>(true);
                }

                return transform.gameObject.GetComponentsInChildrenOnly<Transform>(true);
            }


            if (includeRoot)
            {
                Transform[] children = new Transform[transform.childCount + 1];
                children[0] = transform;

                for (int i = 1; i < children.Length; i++)
                {
                    children[i] = transform.GetChild(i - 1);
                }

                return children;
            }
            else
            {
                Transform[] children = new Transform[transform.childCount];
                for (int i = 0; i < children.Length; i++)
                {
                    children[i] = transform.GetChild(i);
                }

                return children;
            }

        }

        public static void ForEachChild(this Transform transform, Action<Transform> action, bool includeRoot = false, bool includeDeeperChildren = false)
        {
            Transform[] children = GetChildren(transform, includeRoot, includeDeeperChildren);
            for (int i = 0; i < children.Length; i++)
            {
                action(children[i]);
            }
        }

        public static Transform FindInChildren(this Transform transform, string name)
        {
            if (transform.name == name) return transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name == name) return transform.GetChild(i);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform result = transform.GetChild(i).FindInChildren(name);
                if (result != null) return result;
            }

            return null;
        }

    }
}
