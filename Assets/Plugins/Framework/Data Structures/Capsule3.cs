using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public struct Capsule3 : IPhysicsShape3D
    {
        public float Length => _length;
        public Quaternion Rotation => _rotation;
        public Vector3 Center => _center;
        public Vector3 Direction => _rotation.GetForwardVector();
        public Vector3 OuterEndPoint => _center + _rotation.GetForwardVector() * (_length * 0.5f + _radius);
        public Vector3 OuterStartPoint => _center - _rotation.GetForwardVector() * (_length * 0.5f + _radius);
        public Vector3 InnerStartPoint => _center - _rotation.GetForwardVector() * (_length * 0.5f);
        public Vector3 InnerEndPoint => _center + _rotation.GetForwardVector() * (_length * 0.5f);
        public Sphere3 StartSphere => new Sphere3(_center - _rotation.GetForwardVector() * (_length * 0.5f), _radius);
        public Sphere3 EndSphere => new Sphere3(_center + _rotation.GetForwardVector() * (_length * 0.5f), _radius);
        public float Radius => _radius;

        [SerializeField]
        private Vector3 _center;

        [SerializeField]
        private Quaternion _rotation;

        [SerializeField]
        private float _length;

        [SerializeField]
        private float _radius;

        public Capsule3(Vector3 center, Quaternion rotation, float length, float radius)
        {
            _radius = Math.Abs(radius);
            _length = Mathf.Abs(length);
            _center = center;
            _rotation = rotation;
        }

        public Capsule3(Vector3 startPoint, Vector3 endPoint, float radius)
        {
            _radius = Math.Abs(radius);
            _center = (startPoint + endPoint) * 0.5f;
            _length = (endPoint - startPoint).magnitude;
            _rotation = Quaternion.LookRotation(endPoint - startPoint);
        }

        public Capsule3(Vector3 startPoint, Vector3 direction, float length, float radius)
        {
            _radius = Math.Abs(radius);
            _length = Mathf.Abs(length);
            _center = startPoint + direction * _length * 0.5f;
            _rotation = Quaternion.LookRotation(direction);
        }

        public Capsule3(CapsuleCollider capsuleCollider)
        {
            _center = capsuleCollider.transform.TransformPoint(capsuleCollider.center);

            if (capsuleCollider.direction == 0)
            {
                _radius = capsuleCollider.radius * Mathf.Max(Mathf.Abs(capsuleCollider.transform.lossyScale.y), Mathf.Abs(capsuleCollider.transform.lossyScale.z));
                _rotation = capsuleCollider.transform.rotation * Quaternion.LookRotation(Vector3.right);
                _length = Mathf.Max(0, (capsuleCollider.height * capsuleCollider.transform.lossyScale.x) - (_radius * 2f));
            }
            else if (capsuleCollider.direction == 1)
            {
                _radius = capsuleCollider.radius * Mathf.Max(Mathf.Abs(capsuleCollider.transform.lossyScale.x), Mathf.Abs(capsuleCollider.transform.lossyScale.z));
                _rotation = capsuleCollider.transform.rotation * Quaternion.LookRotation(Vector3.up);
                _length = Mathf.Max(0, (capsuleCollider.height * capsuleCollider.transform.lossyScale.y) - (_radius * 2f));
            }
            else
            {
                _radius = capsuleCollider.radius * Mathf.Max(Mathf.Abs(capsuleCollider.transform.lossyScale.x), Mathf.Abs(capsuleCollider.transform.lossyScale.y));
                _rotation = capsuleCollider.transform.rotation * Quaternion.LookRotation(Vector3.forward);
                _length = Mathf.Max(0, (capsuleCollider.height * capsuleCollider.transform.lossyScale.z) - (_radius * 2f));
            }

        }

        public float DistanceTo(Vector3 point)
        {
            Vector3 closestPoint = MathUtils.ProjectPointOnLineSegment(new Line3(InnerStartPoint, InnerEndPoint), point);
            return Mathf.Max(0, (closestPoint - point).magnitude - _radius);
        }

        public bool ContainsPoint(Vector3 point)
        {
            Vector3 closestPoint = MathUtils.ProjectPointOnLineSegment(new Line3(InnerStartPoint, InnerEndPoint), point);
            return (closestPoint - point).sqrMagnitude <= _radius * _radius;

        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            Vector3 closestPoint = MathUtils.ProjectPointOnLineSegment(new Line3(InnerStartPoint, InnerEndPoint), point);
            Vector3 toPoint = (point - closestPoint);

            if (toPoint.sqrMagnitude > _radius * _radius)
            {
                return closestPoint + toPoint.normalized * _radius;
            }

            return point;
        }

        public Collider[] GetOverlappingColliders(LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            Vector3 forward = _rotation.GetForwardVector() * (_length * 0.5f);
            return Physics.OverlapCapsule(_center - forward, _center + forward, _radius, layerMask, triggerInteraction);
        }

        public int GetOverlappingCollidersNonAlloc(Collider[] results, LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            Vector3 forward = _rotation.GetForwardVector() * (_length * 0.5f);
            return Physics.OverlapCapsuleNonAlloc(_center - forward, _center + forward, _radius, results, layerMask, triggerInteraction);
        }

    }
}
