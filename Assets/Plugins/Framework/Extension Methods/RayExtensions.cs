using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Extension methods for rays.
    /// </summary>
    public static class RayExtensions
    {

        /// <summary>
        /// Gets a line segment that starts at this ray's origin and continues in its direction for a specified ditance.
        /// </summary>
        /// <param name="length">The length of the line segment along the ray</param>
        /// <returns>The line segment</returns>
        public static Line3 ToLine(this Ray ray, float length)
        {
            return new Line3(ray.origin, ray.GetPoint(length));
        }

        /// <summary>
        /// Gets a line segment that starts at this ray's origin and continues in its direction for a specified ditance.
        /// </summary>
        /// <param name="length">The length of the line segment along the ray</param>
        /// <returns>The line segment</returns>
        public static Line2 ToLine(this Ray2D ray, float length)
        {
            return new Line2(ray.origin, ray.GetPoint(length));
        }

        /// <summary>
        /// Check if this ray intersects a plane.
        /// </summary>
        /// <param name="plane">The plane to test</param>
        /// <param name="intersectionPoint">The point at which the ray intersects the plane (if it exists)</param>
        /// <returns>True if the ray intersect the plane</returns>
        public static bool IntersectsPlane(this Ray ray, Plane plane, out Vector3 intersectionPoint)
        {
            return MathUtils.CheckRayPlaneIntersection(ray, plane, out intersectionPoint);
        }

        /// <summary>
        /// Checks whether this ray intersects another. Note that in 3d, two rays do not intersect most of the time. So if the two rays are not in the same plane, use MathUtils.CheckClosestPointsOnTwoRays() instead.
        /// </summary>
        /// <param name="otherRay">The ray to test</param>
        /// <param name="intersectionPoint">The point at which the ray intersect (if it exists)</param>
        /// <returns>True if the lines intersect</returns>
        public static bool CheckRayRayIntersection(Ray ray, Ray otherRay, out Vector3 intersectionPoint)
        {
            return MathUtils.CheckRayRayIntersection(ray, otherRay, out intersectionPoint);
        }

        /// <summary>
        /// This function finds a point on the ray that is closes to another ray. Unless they are parallel.
        /// </summary>
        /// <param name="other">The ray to test</param>
        /// <param name="closestPoint">The point on this ray that is closest to the other ray</param>
        /// <returns>True if the rays are not parallel, false if they are</returns>
        public static bool CheckClosestPointToRay(this Ray ray, Ray otherRay, out Vector3 closestPoint)
        {
            Vector3 temp;
            return MathUtils.CheckClosestPointsOnTwoRays(ray, otherRay, out closestPoint, out temp);
        }
    }
}
