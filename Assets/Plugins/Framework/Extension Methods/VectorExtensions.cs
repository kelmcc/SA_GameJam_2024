using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Extension methods for vectors.
    /// </summary>
    public static class VectorExtensions
    {
        public static bool IsCloseTo(this Vector3 vector, Vector3 other, float threshold = 0.0001f)
        {
            if (Mathf.Abs(vector.x - other.x) > threshold) return false;
            if (Mathf.Abs(vector.y - other.y) > threshold) return false;
            if (Mathf.Abs(vector.z - other.z) > threshold) return false;

            return true;
        }

        public static bool IsCloseTo(this Vector2 vector, Vector2 other, float threshold = 0.0001f)
        {
            if (Mathf.Abs(vector.x - other.x) > threshold) return false;
            if (Mathf.Abs(vector.y - other.y) > threshold) return false;

            return true;
        }

        public static float DistanceXZ(this Vector3 vector, Vector3 other)
        {
            return new Vector3(vector.x - other.x, 0, vector.z - other.z).magnitude;
        }

        public static float MagnitudeXZ(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z).magnitude;
        }

        public static float SquareMagnitude(this Vector3 vector, Vector3 other)
        {
            return (vector - other).sqrMagnitude;
        }

        public static float SquareMagnitude(this Vector2 vector, Vector2 other)
        {
            return (vector - other).sqrMagnitude;
        }

        public static Vector3 PlusX(this Vector3 vector, float value)
        {
            return new Vector3(vector.x + value, vector.y, vector.z);
        }

        public static Vector3 PlusY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y + value, vector.z);
        }

        public static Vector3 PlusZ(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y, vector.z + value);
        }

        public static Vector2 PlusX(this Vector2 vector, float value)
        {
            return new Vector2(vector.x + value, vector.y);
        }

        public static Vector2 PlusY(this Vector2 vector, float value)
        {
            return new Vector2(vector.x, vector.y + value);
        }

        public static Vector2 Inverse(this Vector2 vector)
        {
            return new Vector2(1f / vector.x, 1f / vector.y);
        }

        public static Vector3 MultiplyX(this Vector3 vector, float value)
        {
            return new Vector3(vector.x * value, vector.y, vector.z);
        }

        public static Vector3 MultiplyY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y * value, vector.z);
        }

        public static Vector3 MultiplyZ(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y, vector.z * value);
        }

        public static Vector2 MultiplyX(this Vector2 vector, float value)
        {
            return new Vector2(vector.x * value, vector.y);
        }

        public static Vector2 MultiplyY(this Vector2 vector, float value)
        {
            return new Vector2(vector.x, vector.y * value);
        }

        public static Vector3 DivideX(this Vector3 vector, float value)
        {
            return new Vector3(vector.x / value, vector.y, vector.z);
        }

        public static Vector3 DivideY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y / value, vector.z);
        }

        public static Vector3 DivideZ(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y, vector.z / value);
        }

        public static Vector2 DivideX(this Vector2 vector, float value)
        {
            return new Vector2(vector.x / value, vector.y);
        }

        public static Vector2 DivideY(this Vector2 vector, float value)
        {
            return new Vector2(vector.x, vector.y / value);
        }


        public static Vector3 Inverse(this Vector3 vector)
        {
            return new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
        }

        public static float Cross(this Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.y - lhs.y * rhs.x;
        }

        /// <summary>
        /// Returns the vector that is from this vector to another vector.
        /// </summary>
        /// <param name="to">The target vector</param>
        /// <returns>A vector from this vector to the target vector</returns>
        public static Vector3 To(this Vector3 from, Vector3 to)
        {
            return to - from;
        }

        /// <summary>
        /// Returns the vector that is from this vector to another vector.
        /// </summary>
        /// <param name="to">The target vector</param>
        /// <returns>A vector from this vector to the target vector</returns>
        public static Vector2 To(this Vector2 from, Vector2 to)
        {
            return to - from;
        }

        public static Vector2 SubtractConstant(this Vector2 vector, float value)
        {
            return new Vector2(vector.x - value, vector.y - value);
        }

        public static Vector2 AddConstant(this Vector2 vector, float value)
        {
            return new Vector2(vector.x + value, vector.y + value);
        }

        public static Vector3 SubtractConstant(this Vector3 vector, float value)
        {
            return new Vector3(vector.x - value, vector.y - value, vector.z - value);
        }

        public static Vector3 AddConstant(this Vector3 vector, float value)
        {
            return new Vector3(vector.x + value, vector.y + value, vector.z + value);
        }

        /// <summary>
        /// Multiplies the components of this vector with the components of another.
        /// </summary>
        /// <param name="operand">The vector to multiply by</param>
        /// <returns>The multiplied vector</returns>
        public static Vector3 MultiplyComponentWise(this Vector3 vector, Vector3 operand)
        {
            return new Vector3(vector.x * operand.x, vector.y * operand.y, vector.z * operand.z);
        }

        /// <summary>
        /// Multiplies each component of this vector with a separate float value.
        /// </summary>
        /// <param name="x">The value to multiply the X component by</param>
        /// <param name="y">The value to multiply the Y component by</param>
        /// <param name="z">The value to multiply the Z component by</param>
        /// <returns>The multiplied vector</returns>
        public static Vector3 MultiplyComponentWise(this Vector3 vector, float x, float y, float z)
        {
            return new Vector3(vector.x * x, vector.y * y, vector.z * z);
        }

        /// <summary>
        /// Multiples the components of this vector with the components of another.
        /// </summary>
        /// <param name="operand">The vector to multiply by</param>
        /// <returns>The multiplied vector</returns>
        public static Vector2 MultiplyComponentWise(this Vector2 vector, Vector2 operand)
        {
            return new Vector2(vector.x * operand.x, vector.y * operand.y);
        }

        /// <summary>
        /// Multiplies each component of this vector with a separate float value.
        /// </summary>
        /// <param name="x">The value to multiply the X component by</param>
        /// <param name="y">The value to multiply the Y component by</param>
        /// <returns>The multiplied vector</returns>
        public static Vector2 MultiplyComponentWise(this Vector2 vector, float x, float y)
        {
            return new Vector2(vector.x * x, vector.y * y);
        }

        /// <summary>
        /// Divides the components of this vector with the components of another.
        /// </summary>
        /// <param name="operand">The vector to divide by</param>
        /// <returns>The divided vector</returns>
        public static Vector3 DivideComponentWise(this Vector3 vector, Vector3 operand)
        {
            return new Vector3(vector.x / operand.x, vector.y / operand.y, vector.z / operand.z);
        }

        /// <summary>
        /// Divides the components of this vector with the components of another.
        /// </summary>
        /// <param name="operand">The vector to divide by</param>
        /// <returns>The divided vector</returns>
        public static Vector2 DivideComponentWise(this Vector2 vector, Vector2 operand)
        {
            return new Vector2(vector.x / operand.x, vector.y / operand.y);
        }

        /// <summary>
        /// Rounds the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The rounded vector</returns>
        public static Vector3 Rounded(this Vector3 vector)
        {
            return new Vector3(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
        }

        /// <summary>
        /// Rounds the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The rounded vector</returns>
        public static Vector2 Rounded(this Vector2 vector)
        {
            return new Vector2(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        /// <summary>
        /// Floors the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The floored vector</returns>
        public static Vector3 Floored(this Vector3 vector)
        {
            return new Vector3(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
        }

        /// <summary>
        /// Ceils the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The ceiled vector</returns>
        public static Vector3 Ceiled(this Vector3 vector)
        {
            return new Vector3(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y), Mathf.CeilToInt(vector.z));
        }

        /// <summary>
        /// Floors the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The floored vector</returns>
        public static Vector2 Floored(this Vector2 vector)
        {
            return new Vector2(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        }

        /// <summary>
        /// Ceils the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The ceiled vector</returns>
        public static Vector2 Ceiled(this Vector2 vector)
        {
            return new Vector2(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
        }

        /// <summary>
        /// Rounds the components of this vector to the nearest interval.
        /// </summary>
        /// <param name="interval">The interval to round to</param>
        /// <returns>The rounded vector</returns>
        public static Vector3 RoundedToInterval(this Vector3 vector, float interval)
        {
            return new Vector3(MathUtils.RoundToNearest(vector.x, interval), MathUtils.RoundToNearest(vector.y, interval), MathUtils.RoundToNearest(vector.z, interval));
        }

        /// <summary>
        /// Rounds the components of this vector to the nearest interval.
        /// </summary>
        /// <param name="interval">The interval to round to</param>
        /// <returns>The rounded vector</returns>
        public static Vector2 RoundedToInterval(this Vector2 vector, float interval)
        {
            return new Vector2(MathUtils.RoundToNearest(vector.x, interval), MathUtils.RoundToNearest(vector.y, interval));
        }


        ////
        ///
        /// 

        /// <summary>
        /// Rounds the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The rounded vector</returns>
        public static Vector3Int RoundedToInt(this Vector3 vector)
        {
            return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
        }

        /// <summary>
        /// Rounds the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The rounded vector</returns>
        public static Vector2Int RoundedToInt(this Vector2 vector)
        {
            return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        /// <summary>
        /// Floors the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The floored vector</returns>
        public static Vector3Int FlooredToInt(this Vector3 vector)
        {
            return new Vector3Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
        }

        /// <summary>
        /// Ceils the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The ceiled vector</returns>
        public static Vector3Int CeiledToInt(this Vector3 vector)
        {
            return new Vector3Int(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y), Mathf.CeilToInt(vector.z));
        }

        /// <summary>
        /// Floors the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The floored vector</returns>
        public static Vector2Int FlooredToInt(this Vector2 vector)
        {
            return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        }

        /// <summary>
        /// Ceils the components of this vector to the nearest integer.
        /// </summary>
        /// <returns>The ceiled vector</returns>
        public static Vector2Int CeiledToInt(this Vector2 vector)
        {
            return new Vector2Int(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
        }

        /// <summary>
        /// Finds the hex coordinate that is nearest to this position. 
        /// </summary>
        /// <returns>The nearest hex coordinate to this cartesian position</returns>
        public static HexCoordinate ToHexCoordinate(this Vector2 vector)
        {
            return HexCoordinate.FromCartesian(vector);
        }


        /// <summary>
        /// Finds the Cardinal4 that is nearest to this vector's direction.
        /// </summary>
        /// <returns>The nearest Cardinal4 to this vector</returns>
        public static Cardinal4 ToCardinal4(this Vector2 vector)
        {
            int angle = (int)MathUtils.RoundToNearest(Mathf.RoundToInt(vector.Angle()), 90);

            switch (angle)
            {
                case 0: return Cardinal4.Up;
                case 90: return Cardinal4.Right;
                case 180: return Cardinal4.Down;
                case 270: return Cardinal4.Left;
            }

            return Cardinal4.Up;
        }

        /// <summary>
        /// Finds the Cardinal8 that is nearest to this vector's direction.
        /// </summary>
        /// <returns>The nearest Cardinal8 to this vector</returns>
        public static Cardinal8 ToCardinal8(this Vector2 vector)
        {
            int angle = (int)MathUtils.RoundToNearest(Mathf.RoundToInt(vector.Angle()), 45);
            switch (angle)
            {
                case 0: return Cardinal8.North;
                case 45: return Cardinal8.NorthEast;
                case 90: return Cardinal8.East;
                case 135: return Cardinal8.SouthEast;
                case 180: return Cardinal8.South;
                case 225: return Cardinal8.SouthWest;
                case 270: return Cardinal8.West;
                case 315: return Cardinal8.NorthWest;
            }

            return Cardinal8.North;
        }


        /// <summary>
        /// Returns a Vector2 that has been snapped to the nearest cardinal direction (N,E,S,W).
        /// </summary>
        /// <returns>The snapped Vector2</returns>
        public static Vector2 SnapToCardinal4(this Vector2 direction)
        {
            return direction.ToCardinal4().ToVector() * direction.magnitude;
        }

        /// <summary>
        /// Returns a Vector2 that has been snapped to the nearest 8-way cardinal direction (N,NE,E,SE,S,SW,W,NW).
        /// </summary>
        /// <returns>The snapped Vector2</returns>
        public static Vector2 SnapToCardinal8(this Vector2 direction)
        {
            return direction.ToCardinal8().ToVector() * direction.magnitude;
        }

        /// <summary>
        /// Returns a vector that has the same direction, but a new magnitude.
        /// </summary>
        /// <param name="length">The desired magnitude</param>
        /// <returns>The "new" vector</returns>
        public static Vector3 WithMagnitude(this Vector3 vector, float length)
        {
            return vector.normalized * length;
        }

        /// <summary>
        /// Returns this vector rotated by a number of degrees.
        /// </summary>
        /// <param name="angle">The angle to rotate by. (Degrees and clockwise)</param>
        /// <returns>The rotated vector</returns>
        public static Vector2 Rotated(this Vector2 vector, float angle)
        {
            float sin = Mathf.Sin(-angle * Mathf.Deg2Rad);
            float cos = Mathf.Cos(-angle * Mathf.Deg2Rad);

            float tx = vector.x;
            float ty = vector.y;

            return new Vector2((cos * tx) - (sin * ty), (sin * tx) + (cos * ty));
        }

        public static Vector3 RotatedXZ(this Vector3 vector, float angle)
        {
            return MathUtils.AngleToVector(vector.AngleXZ() + angle).ToVector3XZ() * vector.magnitude;
        }



        /// <summary>
        /// The angle in a clockwise direction between this vector and the up vector.
        /// </summary>
        /// <returns>The vector's direction, in degrees</returns>
        public static float Angle(this Vector2 vector)
        {
            if (vector.x < 0) return (Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg) + 360f;

            return Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// The angle in a clockwise direction (on the XY plane) between this vector and the up vector.
        /// </summary>
        /// <returns>The vector's direction, in degrees</returns>
        public static float Angle(this Vector3 vector)
        {
            return Angle(vector.ToVector2());
        }

        /// <summary>
        /// The angle in a clockwise direction (on the XZ plane) between this vector and the up vector.
        /// </summary>
        /// <returns>The vector's direction, in degrees</returns>
        public static float AngleXZ(this Vector3 vector)
        {
            return Angle(vector.ToVector2XZ());
        }


        /// <summary>
        /// Finds a point which is a projection from this point to a ray.
        /// </summary>
        /// <param name="ray">The ray to project to</param>
        /// <returns>The projected point</returns>
        public static Vector3 ProjectedOnRay(this Vector3 point, Ray ray)
        {
            return MathUtils.ProjectPointOnRay(ray, point);
        }

        /// <summary>
        /// Finds a point which is a projection from this point to a line segment. If the projected point lies outside of the line segment, the projected point will be clamped to the appropriate line end.
        /// </summary>
        /// <param name="line">The line to project to</param>
        /// <returns>The projected point</returns>
        public static Vector3 ProjectedOnLineSegment(this Vector3 point, Line3 line)
        {
            return MathUtils.ProjectPointOnLineSegment(line, point);
        }

        /// <summary>
        /// Calculates the angle between this vector and a plane.
        /// </summary>
        /// <param name="plane">The plane</param>
        /// <returns>The angle between this vector and the plane. In radians.</returns>
        public static float AngleWithPlane(this Vector3 vector, Plane plane)
        {
            return MathUtils.VectorPlaneAngle(vector, plane);
        }


        /// <summary>
        /// Checks whether this point is on a line segment.
        /// </summary>
        /// <param name="line">The line segment</param>
        /// <returns>True if the point lies on the line segment</returns>
        public static bool IsOnLineSegment(this Vector3 point, Line3 line)
        {
            return MathUtils.IsPointOnLineSegment(line, point);
        }

        /// <summary>
        /// Checks whether this point is on a ray.
        /// </summary>
        /// <param name="ray">The ray</param>
        /// <returns>True if the point lies on the ray</returns>
        public static bool IsOnRay(this Vector3 point, Ray ray)
        {
            return MathUtils.IsPointOnRay(point, ray);
        }

        /// <summary>
        /// Calculates the perpendicular distance from this point to a ray.
        /// </summary>
        /// <param name="ray">The ray</param>
        /// <returns>The shortest distance to the ray</returns>
        public static float DistanceToRay(Vector3 point, Ray ray)
        {
            return MathUtils.ShortestDistanceFromPointToRay(point, ray);
        }

        /// <summary>
        /// Explicitly converts a Vector3 to a Vector2.
        /// </summary>
        /// <returns>The converted Vector3</returns>
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        /// <summary>
        /// Explicitly converts a Vector3 to a Vector2.
        /// </summary>
        /// <returns>The converted Vector3</returns>
        public static Vector2 ToVector2XZ(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        /// <summary>
        /// Explicitly converts a Vector2 to a Vector3.
        /// </summary>
        /// <returns>The converted Vector3</returns>
        public static Vector3 ToVector3(this Vector2 vector)
        {
            return new Vector3(vector.x, vector.y);
        }

        /// <summary>
        /// Explicitly converts a Vector2 to a Vector3.
        /// </summary>
        /// <returns>The converted Vector3</returns>
        public static Vector3 ToVector3XZ(this Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }

        /// <summary>
        /// Returns the vector, but with a new x value.
        /// </summary>
        /// <param name="x">The new x value</param>
        /// <returns>The "new" vector</returns>
        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        /// <summary>
        /// Returns the vector, but with a new y value.
        /// </summary>
        /// <param name="y">The new y value</param>
        /// <returns>The "new" vector</returns>
        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        /// <summary>
        /// Returns the vector, but with a new z value.
        /// </summary>
        /// <param name="z">The new z value</param>
        /// <returns>The "new" vector</returns>
        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        /// <summary>
        /// Returns the vector, but with a new x value.
        /// </summary>
        /// <param name="x">The new x value</param>
        /// <returns>The "new" vector</returns>
        public static Vector2 WithX(this Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }

        /// <summary>
        /// Returns the vector, but with a new y value.
        /// </summary>
        /// <param name="y">The new y value</param>
        /// <returns>The "new" vector</returns>
        public static Vector2 WithY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }


        /// <summary>
        /// Clamps this vector between two other vectors.
        /// </summary>
        /// <param name="min">The minimum bound</param>
        /// <param name="max">The maximum bound</param>
        /// <returns>The clamped vector</returns>
        public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(vector.x, min.x, max.x), Mathf.Clamp(vector.y, min.y, max.y), Mathf.Clamp(vector.z, min.z, max.z));
        }

        /// <summary>
        /// Returns the vector with the absolute value of each axis
        /// </summary>
        /// <returns>The absolute value of the vector</returns>
        public static Vector3 Abs(this Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static bool HasNaNComponent(this Vector3 vector)
        {
            return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
        }
    }
}
