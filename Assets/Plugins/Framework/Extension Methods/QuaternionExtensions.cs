using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Extension methods for quaternions.
    /// </summary>
    public static class QuaternionExtensions
    {
        public static void Decompose(this Quaternion quaternion, Vector3 direction, out Quaternion swing, out Quaternion twist)
        {
            Vector3 vector = new Vector3(quaternion.x, quaternion.y, quaternion.z);
            Vector3 projection = Vector3.Project(vector, direction);

            twist = new Quaternion(projection.x, projection.y, projection.z, quaternion.w).normalized;
            swing = quaternion * Quaternion.Inverse(twist);
        }

        public static Quaternion Constrain(this Quaternion quaternion, float angle)
        {
            float magnitude = Mathf.Sin(0.5F * angle);
            float sqrMagnitude = magnitude * magnitude;

            Vector3 vector = new Vector3(quaternion.x, quaternion.y, quaternion.z);

            if (vector.sqrMagnitude > sqrMagnitude)
            {
                vector = vector.normalized * magnitude;

                quaternion.x = vector.x;
                quaternion.y = vector.y;
                quaternion.z = vector.z;
                quaternion.w = Mathf.Sqrt(1.0F - sqrMagnitude) * Mathf.Sign(quaternion.w);
            }

            return quaternion;
        }

        public static Quaternion Reflect(this Quaternion rotation, Vector3 normal)
        {
            Vector3 targetForward = rotation * Vector3.forward;
            Vector3 targetUp = rotation * Vector3.up;

            Vector3 reflectedUp = Vector3.Reflect(targetUp, normal);
            Vector3 reflectedForward = Vector3.Reflect(targetForward, normal);

            return Quaternion.LookRotation(reflectedForward, reflectedUp);
        }

        public static bool IsValid(this Quaternion q)
        {
            if (Single.IsNaN(q.x) || Single.IsNaN(q.y) || Single.IsNaN(q.z) || Single.IsNaN(q.w))
            {
                return false;
            }

            return Mathf.Approximately(Mathf.Sqrt((q.x * q.x) + (q.y * q.y) + (q.z * q.z) + (q.w * q.w)), 1f);
        }

        public static Quaternion Normalized(this Quaternion q)
        {
            float factor = 1f / Mathf.Sqrt((((q.x * q.x) + (q.y * q.y)) + (q.z * q.z)) + (q.w * q.w));
            q.x *= factor;
            q.y *= factor;
            q.z *= factor;
            q.w *= factor;

            return q;
        }

        public static Quaternion Inverse(this Quaternion q)
        {
            return Quaternion.Inverse(q);
        }

        /// <summary>
        /// Calcuates the resulting quaternion when this quaternion is rotated by another.
        /// </summary>
        /// <param name="rotation">The quaternion rotation to apply</param>
        /// <returns>The rotated quaternion</returns>
        public static Quaternion AddRotation(this Quaternion q, Quaternion rotation)
        {
            return q * rotation;
        }

        /// <summary>
        /// Calcuates the resulting quaternion when this quaternion is unrotated by another.
        /// </summary>
        /// <param name="rotation">The quaternion rotation to inversly apply</param>
        /// <returns>The unrotated quaternion</returns>
        public static Quaternion SubtractRotation(this Quaternion q, Quaternion rotation)
        {
            return q * Quaternion.Inverse(rotation);
        }


        /// <summary>
        /// Calculate the quaternion's forward vector.
        /// </summary>
        /// <returns>The quaternion's forward vector</returns>
        public static Vector3 GetForwardVector(this Quaternion q)
        {
            return q * Vector3.forward;
        }

        /// <summary>
        /// Calculate the quaternion's up vector.
        /// </summary>
        /// <returns>The quaternion's up vector</returns>
        public static Vector3 GetUpVector(this Quaternion q)
        {
            return q * Vector3.up;
        }

        /// <summary>
        /// Calculate the quaternion's right vector.
        /// </summary>
        /// <returns>The quaternion's right vector</returns>
        public static Vector3 GetRightVector(this Quaternion q)
        {
            return q * Vector3.right;
        }

        /// <summary>
        /// Calculate the quaternion's back vector.
        /// </summary>
        /// <returns>The quaternion's back vector</returns>
        public static Vector3 GetBackVector(this Quaternion q)
        {
            return q * -Vector3.forward;
        }

        /// <summary>
        /// Calculate the quaternion's down vector.
        /// </summary>
        /// <returns>The quaternion's down vector</returns>
        public static Vector3 GetDownVector(this Quaternion q)
        {
            return q * -Vector3.up;
        }

        /// <summary>
        /// Calculate the quaternion's left vector.
        /// </summary>
        /// <returns>The quaternion's left vector</returns>
        public static Vector3 GetLeftVector(this Quaternion q)
        {
            return q * -Vector3.right;
        }
    }
}
