using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public struct Sphere3 : IPhysicsShape3D
    {
        public Vector3 Center { get => _center; set => _center = value; }
        public float Radius { get => _radius; set => _radius = value; }

        [SerializeField]
        private Vector3 _center;

        [SerializeField]
        private float _radius;

        public Sphere3(Vector3 center, float radius)
        {
            _radius = Math.Abs(radius);
            _center = center;
        }

        public Sphere3(SphereCollider sphereCollider)
        {
            _center = sphereCollider.transform.TransformPoint(sphereCollider.center);
            _radius = sphereCollider.radius * Mathf.Max(sphereCollider.transform.lossyScale.x, Mathf.Max(sphereCollider.transform.lossyScale.y, sphereCollider.transform.lossyScale.z));
        }

        public Sphere3(BoundingSphere sphere)
        {
            _radius = sphere.radius;
            _center = sphere.position;
        }

        public float DistanceTo(Vector3 point)
        {
            return Mathf.Max(0, (_center - point).magnitude - _radius);
        }

        public bool ContainsPoint(Vector3 point)
        {
            return (_center - point).sqrMagnitude <= _radius * _radius;
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            Vector3 toPoint = (point - _center);
            if (toPoint.sqrMagnitude > _radius * _radius)
            {
                return _center + toPoint.normalized * _radius;
            }

            return point;
        }

        public Collider[] GetOverlappingColliders(LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.OverlapSphere(_center, _radius, layerMask, triggerInteraction);
        }

        public int GetOverlappingCollidersNonAlloc(Collider[] results, LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.OverlapSphereNonAlloc(_center, _radius, results, layerMask, triggerInteraction);
        }
    }
}
