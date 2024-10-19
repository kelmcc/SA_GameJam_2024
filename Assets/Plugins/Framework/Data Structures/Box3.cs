using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public struct Box3 : ISerializationCallbackReceiver, IPhysicsShape3D
    {
        public Matrix4x4 AsMatrix => Matrix4x4.TRS(_center, _rotation, _dimensions * 0.5f);
        public Vector3 Center { get => _center; set => _center = value; }
        public Quaternion Rotation { get => _rotation; set => _rotation = value; }
        public Vector3 Dimensions { get => _dimensions; set => _dimensions = value; }
        public Vector3 Extents { get => _dimensions * 0.5f; set => _dimensions = value * 2f; }
        public float Volume => _dimensions.x * _dimensions.y * _dimensions.z;

        public Vector3 ForwardFacePosition => _center + _rotation * new Vector3(0, 0, _dimensions.z * 0.5f);
        public Vector3 BackFacePosition => _center + _rotation * new Vector3(0, 0, _dimensions.z * -0.5f);
        public Vector3 LeftFacePosition => _center + _rotation * new Vector3(_dimensions.x * -0.5f, 0, 0);
        public Vector3 RightFacePosition => _center + _rotation * new Vector3(_dimensions.x * 0.5f, 0, 0);
        public Vector3 TopFacePosition => _center + _rotation * new Vector3(0, _dimensions.y * 0.5f, 0);
        public Vector3 BottomFacePosition => _center + _rotation * new Vector3(0, _dimensions.y * -0.5f, 0);

        public Vector3 ForwardDirection => Rotation.GetForwardVector();
        public Vector3 BackDirection => Rotation.GetBackVector();
        public Vector3 LeftDirection => Rotation.GetLeftVector();
        public Vector3 RightDirection => Rotation.GetRightVector();
        public Vector3 UpDirection => Rotation.GetUpVector();
        public Vector3 DownDirection => Rotation.GetDownVector();

        [SerializeField]
        private Vector3 _center;

        [SerializeField]
        private Quaternion _rotation;

        [SerializeField]
        private Vector3 _dimensions;

        public Box3(Vector3 center, Quaternion rotation, Vector3 dimensions)
        {
            _center = center;
            _rotation = rotation;
            _dimensions = dimensions;
        }

        public Box3(Vector3 center, Vector3 dimensions)
        {
            _center = center;
            _rotation = Quaternion.identity;
            _dimensions = dimensions;
        }

        public Box3(Matrix4x4 matrix)
        {
            _center = matrix.GetTranslation();
            _rotation = matrix.GetRotation();
            _dimensions = matrix.GetScale() * 2f;
        }

        public Box3(Transform transform)
        {
            _center = transform.position;
            _rotation = transform.rotation;
            _dimensions = transform.lossyScale;
        }

        public Box3(Bounds bounds)
        {
            _center = bounds.center;
            _rotation = Quaternion.identity;
            _dimensions = bounds.extents * 2f;
        }

        public Box3(BoxCollider boxCollider)
        {
            _center = boxCollider.transform.TransformPoint(boxCollider.center);
            _rotation = boxCollider.transform.rotation;
            _dimensions = boxCollider.transform.lossyScale.MultiplyComponentWise(boxCollider.size);
        }


        public Vector3[] GetCorners()
        {
            Vector3[] corners = new Vector3[8];
            Matrix4x4 matrix = Matrix4x4.TRS(_center, _rotation, _dimensions * 0.5f);

            corners[0] = matrix.MultiplyPoint3x4(new Vector3(-1, -1, 1));
            corners[1] = matrix.MultiplyPoint3x4(new Vector3(1, -1, 1));
            corners[2] = matrix.MultiplyPoint3x4(new Vector3(1, -1, -1));
            corners[3] = matrix.MultiplyPoint3x4(new Vector3(-1, -1, -1));
            corners[4] = matrix.MultiplyPoint3x4(new Vector3(-1, 1, 1));
            corners[5] = matrix.MultiplyPoint3x4(new Vector3(1, 1, 1));
            corners[6] = matrix.MultiplyPoint3x4(new Vector3(1, 1, -1));
            corners[7] = matrix.MultiplyPoint3x4(new Vector3(-1, 1, -1));

            return corners;
        }

        public Vector3 TransformPoint(Vector3 localPoint)
        {
            return Matrix4x4.TRS(_center, _rotation, _dimensions * 0.5f).MultiplyPoint3x4(localPoint);
        }

        public Vector3 InverseTransformPoint(Vector3 point)
        {
            return Matrix4x4.TRS(_center, _rotation, _dimensions * 0.5f).inverse.MultiplyPoint3x4(point);
        }

        public bool ContainsPoint(Vector3 point)
        {
            point = Matrix4x4.TRS(_center, _rotation, _dimensions * 0.5f).inverse.MultiplyPoint3x4(point);
            return point.x >= -1f && point.x <= 1f && point.y >= -1f && point.y <= 1f && point.z >= -1f && point.z <= 1f;
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            point = Matrix4x4.TRS(_center, _rotation, _dimensions * 0.5f).inverse.MultiplyPoint3x4(point);

            point.x = point.x < -1f ? -1f : (point.x > 1f ? 1f : point.x);
            point.y = point.y < -1f ? -1f : (point.y > 1f ? 1f : point.y);
            point.z = point.z < -1f ? -1f : (point.z > 1f ? 1f : point.z);

            return Matrix4x4.TRS(_center, _rotation, _dimensions * 0.5f).MultiplyPoint3x4(point);
        }

        public void OnBeforeSerialize()
        {
            if (!_rotation.IsValid())
            {
                if (_dimensions == Vector3.zero)
                {
                    _dimensions = Vector3.one;
                }

                _rotation = Quaternion.identity;
            }
        }

        public void OnAfterDeserialize()
        {

        }

        public Collider[] GetOverlappingColliders(LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.OverlapBox(_center, _dimensions * 0.5f, _rotation, layerMask, triggerInteraction);
        }

        public int GetOverlappingCollidersNonAlloc(Collider[] results, LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.OverlapBoxNonAlloc(_center, _dimensions * 0.5f, results, _rotation, layerMask, triggerInteraction);
        }

    }
}
