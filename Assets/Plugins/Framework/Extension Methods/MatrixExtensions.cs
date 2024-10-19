using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Extension methods for matrices
    /// </summary>
    public static class MatrixExtensions
    {

        /// <summary>
        /// Transforms a point from world-space to the local space of the matrix;
        /// </summary>
        /// <returns>local positiom</returns>
        public static Vector3 InverseTransformPoint(this Matrix4x4 m, Vector3 point)
        {
            return m.inverse.MultiplyPoint(point);
        }

        /// <summary>
        /// Gets the rotation component from a TRS matrix.
        /// </summary>
        /// <returns>The matrix's rotation</returns>
        public static Quaternion GetRotation(this Matrix4x4 m)
        {
            return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
        }

        /// <summary>
        /// Gets the position component from a TRS matrix.
        /// </summary>
        /// <returns>The matrix's position</returns>
        public static Vector3 GetTranslation(this Matrix4x4 m)
        {
            Vector4 vector4Position = m.GetColumn(3);
            return new Vector3(vector4Position.x, vector4Position.y, vector4Position.z);
        }


        /// <summary>
        /// Gets the scale component from a TRS matrix.
        /// </summary>
        /// <returns>The matrix's scale</returns>
        public static Vector3 GetScale(this Matrix4x4 m)
        {
            Vector3 scale;
            scale.x = new Vector4(m.m00, m.m10, m.m20, m.m30).magnitude;
            scale.y = new Vector4(m.m01, m.m11, m.m21, m.m31).magnitude;
            scale.z = new Vector4(m.m02, m.m12, m.m22, m.m32).magnitude;
            return scale;
        }

        public static Matrix4x4 WithTranslation(this Matrix4x4 TRSMatrix, Vector3 translation)
        {
            return Matrix4x4.TRS(translation, TRSMatrix.GetRotation(), TRSMatrix.GetScale());
        }

        public static Matrix4x4 WithRotation(this Matrix4x4 TRSMatrix, Quaternion rotation)
        {
            return Matrix4x4.TRS(TRSMatrix.GetTranslation(), rotation, TRSMatrix.GetScale());
        }

        public static Matrix4x4 WithScale(this Matrix4x4 TRSMatrix, Vector3 scale)
        {
            return Matrix4x4.TRS(TRSMatrix.GetTranslation(), TRSMatrix.GetRotation(), scale);
        }


    }
}
