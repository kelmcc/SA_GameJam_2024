using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// A structure representing a line segment in 2D space. If you want an infinite line (not a segment), use a Ray2D instead.
    /// </summary>
    [Serializable]
    public struct Line2
    {

        [SerializeField]
        private Vector2 _start;

        [SerializeField]
        private Vector2 _end;

        /// <summary>
        /// A point on the line. The start of the line.
        /// </summary>
        public Vector2 Start => _start;

        /// <summary>
        /// A second point on the line. The end of the line.
        /// </summary>
        public Vector2 End => _end;

        /// <summary>
        /// The point directly between the point A and B.
        /// </summary>
        public Vector2 MidPoint => (_end + _start) * 0.5f;

        /// <summary>
        /// The normalized direction of the line.
        /// </summary>
        public Vector2 Direction => (_end - _start).normalized;

        /// <summary>
        /// The length of the line segment (distance between point A and B).
        /// </summary>
        public float Length => (_end - _start).magnitude;

        /// <summary>
        /// The length of the line segment (distance between point A and B), but squared.
        /// </summary>
        public float LengthSquared => (_end - _start).sqrMagnitude;

        /// <summary>
        /// Half the length of the line segment (distance between point A and B).
        /// </summary>
        public float HalfLength => (_end - _start).magnitude * 0.5f;

        /// <summary>
        /// A line that start at the origin and is aligned with the positive Y axis. Has a length of one.
        /// </summary>
        public static Line2 Up => new Line2(Vector2.zero, Vector2.up);

        /// <summary>
        /// A line that start at the origin and is aligned with the positive X axis. Has a length of one.
        /// </summary>
        public static Line2 Right => new Line2(Vector2.zero, Vector2.right);

        /// <summary>
        /// Creates a new line from two points.
        /// </summary>
        /// <param name="start">The first point</param>
        /// <param name="end">The second point</param>
        public Line2(Vector2 start, Vector2 end)
        {
            _start = start;
            _end = end;

        }

        /// <summary>
        /// Returns a ray of infinite length that has the same direction as this line segment.
        /// </summary>
        /// <returns>The ray</returns>
        public Ray2D ToRay()
        {
            return new Ray2D(_start, Direction);
        }

        /// <summary>
        /// Returns a vector from the beginning of the line to the end.
        /// </summary>
        /// <returns>A vector from the beginning of the line to the end.</returns>
        public Vector2 ToVector()
        {
            return _end - _start;
        }

        public Vector2 Lerp(float t)
        {
            Vector2 vector = _end - _start;
            return _start + (vector.normalized * vector.magnitude * t);
        }

    }
}
