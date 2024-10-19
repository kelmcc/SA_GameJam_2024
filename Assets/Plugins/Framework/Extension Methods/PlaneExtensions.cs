using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Extension methods for planes
    /// </summary>
    public static class PlaneExtensions
    {

        /// <summary>
        /// Returns a point that is on the plane.
        /// </summary>
        /// <returns>A point on the plane</returns>
        public static Vector3 GetOrigin(this Plane plane)
        {
            return -plane.normal * plane.distance;
        }

        /// <summary>
        /// Find the line of intersection between this plane and another.
        /// </summary>
        /// <param name="otherPlane">The plane to test</param>
        /// <param name="intersectingRay">The line of intersection, if it exists</param>
        /// <returns>True if the planes intersect (they are not parallel)</returns>
        public static bool IntersectsPlane(this Plane plane, Plane otherPlane, out Ray intersectingRay)
        {
            return MathUtils.CheckPlanePlaneIntersection(plane, otherPlane, out intersectingRay);
        }

    }
}
