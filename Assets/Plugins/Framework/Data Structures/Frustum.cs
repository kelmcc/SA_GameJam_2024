using UnityEngine;

namespace Framework
{
    public struct Frustum
    {
        public Plane FarPlane => _farPlane;
        public Plane NearPlane => _nearPlane;
        public Plane LeftPlane => _leftPlane;
        public Plane RightPlane => _rightPlane;
        public Plane TopPlane => _topPlane;
        public Plane BottomPlane => _bottomPlane;

        public Vector3 FarTopLeftCorner => _farTopLeftCorner;
        public Vector3 FarTopRightCorner => _farTopRightCorner;
        public Vector3 FarBottomLeftCorner => _farBottomLeftCorner;
        public Vector3 FarBottomRightCorner => _farBottomRightCorner;

        public Vector3 NearTopLeftCorner => _nearTopLeftCorner;
        public Vector3 NearTopRightCorner => _nearTopRightCorner;
        public Vector3 NearBottomLeftCorner => _nearBottomLeftCorner;
        public Vector3 NearBottomRightCorner => _nearBottomRightCorner;

        public Ray TopLeftRay => new Ray(_nearTopLeftCorner, _farTopLeftCorner - _nearTopLeftCorner);
        public Ray TopRightRay => new Ray(_nearTopRightCorner, _farTopRightCorner - _nearTopRightCorner);
        public Ray BottomLeftRay => new Ray(_nearBottomLeftCorner, _farBottomLeftCorner - _nearBottomLeftCorner);
        public Ray BottomRightRay => new Ray(_nearBottomRightCorner, _farBottomRightCorner - _nearBottomRightCorner);

        Plane _farPlane;
        Plane _nearPlane;
        Plane _leftPlane;
        Plane _rightPlane;
        Plane _topPlane;
        Plane _bottomPlane;

        private Vector3 _farTopLeftCorner;
        private Vector3 _farTopRightCorner;
        private Vector3 _farBottomLeftCorner;
        private Vector3 _farBottomRightCorner;

        private Vector3 _nearTopLeftCorner;
        private Vector3 _nearTopRightCorner;
        private Vector3 _nearBottomLeftCorner;
        private Vector3 _nearBottomRightCorner;


        public Frustum(Vector3 position, Quaternion rotation, float nearClipDistance, float farClipDistance, float fieldOfView, float aspectRatio)
        {
            Vector3 forward = rotation.GetForwardVector();
            Vector3 right = rotation.GetRightVector();
            Vector3 up = rotation.GetUpVector();

            Vector3 nearCenter = position + forward * nearClipDistance;
            Vector3 farCenter = position + forward * farClipDistance;

            float halfNearHeight = Mathf.Tan(fieldOfView * Mathf.Deg2Rad / 2) * nearClipDistance;
            float halfFarHeight = Mathf.Tan(fieldOfView * Mathf.Deg2Rad / 2) * farClipDistance;
            float halfNearWidth = halfNearHeight * aspectRatio;
            float halfFarWidth = halfFarHeight * aspectRatio;

            _farTopLeftCorner = farCenter + up * halfFarHeight - right * halfFarWidth;
            _farTopRightCorner = farCenter + up * halfFarHeight + right * halfFarWidth;
            _farBottomLeftCorner = farCenter - up * halfFarHeight - right * halfFarWidth;
            _farBottomRightCorner = farCenter - up * halfFarHeight + right * halfFarWidth;

            _nearTopLeftCorner = nearCenter + up * halfNearHeight - right * halfNearWidth;
            _nearTopRightCorner = nearCenter + up * halfNearHeight + right * halfNearWidth;
            _nearBottomLeftCorner = nearCenter - up * halfNearHeight - right * halfNearWidth;
            _nearBottomRightCorner = nearCenter - up * halfNearHeight + right * halfNearWidth;

            _farPlane = new Plane(_farTopLeftCorner, _farTopRightCorner, _farBottomRightCorner);
            _nearPlane = new Plane(_nearBottomRightCorner, _nearTopRightCorner, _nearTopLeftCorner);
            _leftPlane = new Plane(_farTopLeftCorner, _farBottomLeftCorner, _nearBottomLeftCorner);
            _rightPlane = new Plane(_farTopRightCorner, _nearTopRightCorner, _nearBottomRightCorner);
            _topPlane = new Plane(_farTopRightCorner, _farTopLeftCorner, _nearTopRightCorner);
            _bottomPlane = new Plane(_farBottomLeftCorner, _farBottomRightCorner, _nearBottomRightCorner);
        }

        public Frustum(Camera camera) : this(camera.transform.position, camera.transform.rotation, camera.nearClipPlane, camera.farClipPlane, camera.fieldOfView, camera.aspect) { }


        public bool ContainsBounds(Bounds bounds, float tolerance = 0)
        {
            if (!ContainsPoint(bounds.min, tolerance)) return false;
            if (!ContainsPoint(bounds.max, tolerance)) return false;
            if (!ContainsPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), tolerance)) return false;
            if (!ContainsPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), tolerance)) return false;
            if (!ContainsPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), tolerance)) return false;
            if (!ContainsPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), tolerance)) return false;
            if (!ContainsPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), tolerance)) return false;
            if (!ContainsPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), tolerance)) return false;

            return true;
        }

        public bool ContainsPoint(Vector3 point, float tolerance = 0)
        {
            if (_leftPlane.GetDistanceToPoint(point) + tolerance < 0) return false;
            if (_rightPlane.GetDistanceToPoint(point) + tolerance < 0) return false;
            if (_topPlane.GetDistanceToPoint(point) + tolerance < 0) return false;
            if (_bottomPlane.GetDistanceToPoint(point) + tolerance < 0) return false;
            if (_nearPlane.GetDistanceToPoint(point) + tolerance < 0) return false;
            if (_farPlane.GetDistanceToPoint(point) + tolerance < 0) return false;

            return true;
        }

        public float GetSignedDistanceToPoint(Vector3 point)
        {
            float minDistance = _leftPlane.GetDistanceToPoint(point);

            minDistance = Mathf.Min(minDistance, _rightPlane.GetDistanceToPoint(point));
            minDistance = Mathf.Min(minDistance, _topPlane.GetDistanceToPoint(point));
            minDistance = Mathf.Min(minDistance, _bottomPlane.GetDistanceToPoint(point));
            minDistance = Mathf.Min(minDistance, _nearPlane.GetDistanceToPoint(point));
            minDistance = Mathf.Min(minDistance, _farPlane.GetDistanceToPoint(point));

            return -minDistance;
        }

        public bool IntersectsPlane(Plane plane, out Vector3 bottomLeftIntersection, out Vector3 topLeftIntersection, out Vector3 bottomRightIntersection, out Vector3 topRightIntersection)
        {
            if (plane.Raycast(BottomLeftRay, out bottomLeftIntersection) && plane.Raycast(TopLeftRay, out topLeftIntersection) && plane.Raycast(BottomRightRay, out bottomRightIntersection) && plane.Raycast(TopRightRay, out topRightIntersection))
            {
                return true;
            }

            bottomLeftIntersection = Vector3.zero;
            topLeftIntersection = Vector3.zero;
            bottomRightIntersection = Vector3.zero;
            topRightIntersection = Vector3.zero;

            return false;
        }
    }
}
