using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public struct Cylinder3
    {

        public float HalfLength => _length * 0.5f;
        public float Length => _length;
        public Quaternion Rotation => _rotation;
        public Vector3 Center => _center;
        public Vector3 StartPoint => _center - _rotation.GetForwardVector() * (_length * 0.5f);
        public Vector3 EndPoint => _center + _rotation.GetForwardVector() * (_length * 0.5f);
        public float Radius => _radius;

        [SerializeField]
        private Vector3 _center;

        [SerializeField]
        private Quaternion _rotation;

        [SerializeField]
        private float _length;

        [SerializeField]
        private float _radius;


        public Cylinder3(Vector3 center, Quaternion rotation, float length, float radius)
        {
            _radius = Math.Abs(radius);
            _length = Mathf.Abs(length);
            _center = center;
            _rotation = rotation;
        }

        public Cylinder3(Vector3 startPoint, Vector3 endPoint, float radius)
        {
            _radius = Math.Abs(radius);
            _center = (startPoint + endPoint) * 0.5f;
            _length = (endPoint - startPoint).magnitude;
            _rotation = Quaternion.LookRotation(endPoint - startPoint);
        }

        public Cylinder3(Vector3 startPoint, Vector3 direction, float length, float radius)
        {
            _radius = Math.Abs(radius);
            _length = Mathf.Abs(length);
            _center = startPoint + direction * _length * 0.5f;
            _rotation = Quaternion.LookRotation(direction);
        }

        public bool Contains(Vector3 point)
        {
            Vector3 pointOnRay = MathUtils.ProjectPointOnRay(new Ray(_center, _rotation.GetForwardVector()), point);
            float halfLength = _length * 0.5f;

            if (_center.To(pointOnRay).sqrMagnitude < halfLength * halfLength)
            {
                if ((point - pointOnRay).sqrMagnitude < _radius * _radius)
                {
                    return true;
                }
            }

            return false;
        }

        public float DistanceTo(Vector3 point)
        {
            Vector3 pointOnRay = MathUtils.ProjectPointOnRay(new Ray(_center, _rotation.GetForwardVector()), point);
            float perpendicular = (point - pointOnRay).magnitude;
            float toPointOnRay = (pointOnRay - _center).magnitude;

            float halfLength = _length * 0.5f;

            if (_center.To(pointOnRay).sqrMagnitude < halfLength * halfLength)
            {

                if (perpendicular < _radius)
                {
                    return 0f;
                }

                return perpendicular - _radius;
            }

            if (perpendicular < _radius)
            {
                return toPointOnRay - halfLength;
            }

            perpendicular -= _radius;
            toPointOnRay -= halfLength;

            return Mathf.Sqrt((perpendicular * perpendicular) + (toPointOnRay * toPointOnRay));
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            Vector3 pointOnRay = MathUtils.ProjectPointOnRay(new Ray(_center, _rotation.GetForwardVector()), point);
            Vector3 perpendicular = point - pointOnRay;
            Vector3 direction = (pointOnRay - _center).normalized;

            float halfLength = _length * 0.5f;

            if (_center.To(pointOnRay).sqrMagnitude < halfLength * halfLength)
            {
                if (perpendicular.sqrMagnitude < _radius * _radius)
                {
                    return point;
                }

                return pointOnRay + perpendicular.normalized * _radius;
            }

            if (perpendicular.sqrMagnitude < _radius * _radius)
            {
                return _center + (direction * halfLength) + perpendicular;
            }

            return _center + (direction * halfLength) + (perpendicular.normalized * _radius);
        }

    }
}
