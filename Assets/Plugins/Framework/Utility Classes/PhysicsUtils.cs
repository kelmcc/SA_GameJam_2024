using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class PhysicsUtils
    {
        public static Collider[] ColliderBuffer
        {
            get
            {
                if (_colliderBuffer == null)
                {
                    _colliderBuffer = new Collider[256];
                }

                return _colliderBuffer;
            }
        }

        public static RaycastHit[] RaycastBuffer
        {
            get
            {
                if (_raycastBuffer == null)
                {
                    _raycastBuffer = new RaycastHit[256];
                }

                return _raycastBuffer;
            }
        }


        private static RaycastHit[] _raycastBuffer;
        private static Collider[] _colliderBuffer;
        private static List<Collider> _colliderList;
        private static SphereCollider _testSphere;

        public static bool RaycastIgnoring(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask, Collider ignoreCollider)
        {
            hit = new RaycastHit();
            float minDistance = Mathf.Infinity;

            int count = Physics.RaycastNonAlloc(origin, direction, RaycastBuffer, maxDistance, layerMask);
            for (int i = 0; i < count; i++)
            {
                if (RaycastBuffer[i].collider != ignoreCollider && RaycastBuffer[i].distance < minDistance)
                {
                    hit = RaycastBuffer[i];
                    minDistance = RaycastBuffer[i].distance;
                }
            }

            return minDistance < Mathf.Infinity;
        }

        public static bool RaycastIgnoring(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask, IList<Collider> ignoreColliders)
        {
            hit = new RaycastHit();
            float minDistance = Mathf.Infinity;

            int count = Physics.RaycastNonAlloc(origin, direction, RaycastBuffer, maxDistance, layerMask);
            for (int i = 0; i < count; i++)
            {
                if (!ignoreColliders.Contains(RaycastBuffer[i].collider) && RaycastBuffer[i].distance < minDistance)
                {
                    hit = RaycastBuffer[i];
                    minDistance = RaycastBuffer[i].distance;
                }
            }

            return minDistance < Mathf.Infinity;
        }

        public static bool RaycastFiltered(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask, Predicate<RaycastHit> filter)
        {
            hit = new RaycastHit();
            float minDistance = Mathf.Infinity;

            int count = Physics.RaycastNonAlloc(origin, direction, RaycastBuffer, maxDistance, layerMask);
            for (int i = 0; i < count; i++)
            {
                if (RaycastBuffer[i].distance < minDistance && filter(RaycastBuffer[i]))
                {
                    hit = RaycastBuffer[i];
                    minDistance = RaycastBuffer[i].distance;
                }
            }

            return minDistance < Mathf.Infinity;
        }

        public static bool Raycast(Vector3 from, Vector3 to, LayerMask layerMask)
        {
            Vector3 line = to - from;
            return Physics.Raycast(new Ray(from, line.normalized), line.magnitude, layerMask);
        }

        public static RaycastHit[] RaycastAll(Vector3 from, Vector3 to, LayerMask layerMask)
        {
            Vector3 line = to - from;
            return Physics.RaycastAll(new Ray(from, line.normalized), line.magnitude, layerMask);
        }


        public static T GetNearestInSphere<T>(Vector3 point, float radius, LayerMask layermask) where T : Component
        {
            Collider[] colliders = Physics.OverlapSphere(point, radius, layermask);
            float minDist = Mathf.Infinity;
            T nearest = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                T item = colliders[i].GetComponentInParent<T>();

                if (item != null)
                {
                    float dist = (item.transform.position - point).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = item;
                    }
                }
            }

            return nearest;
        }

        public static T GetNearestInSphereWhen<T>(Vector3 point, float radius, LayerMask layermask, Predicate<T> filter) where T : Component
        {
            Collider[] colliders = Physics.OverlapSphere(point, radius, layermask);
            float minDist = Mathf.Infinity;
            T nearest = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                T item = colliders[i].GetComponentInParent<T>();

                if (item != null && filter(item))
                {
                    float dist = (item.transform.position - point).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = item;
                    }
                }
            }

            return nearest;
        }

        public static LayerMask GetCollisionMatrixMask(int layer)
        {
            int mask = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(layer, i))
                {
                    mask |= 1 << i;
                }
            }

            return mask;
        }

        public static RaycastHit[] FanCast(Vector3 origin, Vector3 direction, Vector3 normal, float angle, float distance, int rays, LayerMask layerMask)
        {
            float interval = angle / rays;

            List<RaycastHit> hits = new List<RaycastHit>();
            Quaternion rotation = Quaternion.LookRotation(direction.normalized, normal) * Quaternion.Euler(0, -angle * 0.5f, 0);

            for (int i = 0; i <= rays; i++)
            {
                Ray ray = new Ray(origin, rotation * Quaternion.Euler(0, interval * i, 0) * Vector3.forward);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, distance, layerMask))
                {
                    hits.Add(hit);

                    //     DebugUtils.DrawLine(origin, origin + ray.direction, Color.red);
                }
                else
                {
                    //    DebugUtils.DrawLine(origin, origin + ray.direction);
                }


            }



            return hits.ToArray();
        }

        public static Vector3 CalculateDepenetratedSpherePosition(Vector3 center, float radius, LayerMask collisionMask, out Vector3 collisionResponse)
        {
            Vector3 position = center;
            collisionResponse = Vector3.zero;

            if (_testSphere == null)
            {
                _testSphere = new GameObject("Depenetration Test Sphere").AddComponent<SphereCollider>();
                _testSphere.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            _testSphere.gameObject.SetActive(true);
            _testSphere.transform.position = center;

            for (int i = 0; i < Physics.defaultSolverIterations; i++)
            {
                bool penetrated = false;
                Vector3 iterationResponse = Vector3.zero;
                int numColliders = Physics.OverlapSphereNonAlloc(position, radius, ColliderBuffer, collisionMask, QueryTriggerInteraction.Ignore);

                // Loop through all the nearby colliders and resolve penetrations with them
                for (int j = 0; j < numColliders; j++)
                {

                    Collider worldCollider = ColliderBuffer[j];

                    if (!worldCollider.gameObject.activeInHierarchy || !worldCollider.enabled || worldCollider.isTrigger) continue;


                    Vector3 direction;
                    float distance;

                    if (Physics.ComputePenetration(_testSphere, position, Quaternion.identity, worldCollider, worldCollider.transform.position, worldCollider.transform.rotation, out direction, out distance))
                    {
                        iterationResponse += direction * distance;
                        position += direction * distance;
                        collisionResponse += direction * distance;
                        penetrated = true;
                    }

                    if (!penetrated || iterationResponse == Vector3.zero) break;
                }
            }

            _testSphere.transform.position = position;
            _testSphere.gameObject.SetActive(false);

            return position;
        }

        public static Vector3 CalculateDepentratedPosition(GameObject obj, LayerMask layerMask, out Vector3 collisionResponse, Predicate<Collider> colliderFilter = null)
        {
            Vector3 position = obj.transform.position;
            Bounds bounds = new Bounds();
            collisionResponse = Vector3.zero;

            if (_colliderList == null) _colliderList = new List<Collider>();

            obj.GetComponentsInChildren(_colliderList);

            for (int i = 0; i < _colliderList.Count; i++)
            {
                if (!_colliderList[i].enabled || !_colliderList[i].gameObject.activeInHierarchy || _colliderList[i].isTrigger || _colliderList[i].bounds.extents == Vector3.zero)
                {
                    _colliderList.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (bounds.extents == Vector3.zero)
                    {
                        bounds = _colliderList[i].bounds;
                    }
                    else
                    {
                        bounds.Encapsulate(_colliderList[i].bounds);
                    }
                }
            }


            for (int i = 0; i < Physics.defaultSolverIterations; i++)
            {
                bool penetrated = false;
                Vector3 iterationResponse = Vector3.zero;
                int numColliders = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, ColliderBuffer, Quaternion.identity, layerMask, QueryTriggerInteraction.Ignore);

                // Loop through all the nearby colliders and resolve penetrations with them
                for (int j = 0; j < numColliders; j++)
                {

                    Collider worldCollider = ColliderBuffer[j];

                    if (!worldCollider.gameObject.activeInHierarchy || !worldCollider.enabled || worldCollider.isTrigger) continue;
                    if (colliderFilter != null && !colliderFilter(worldCollider)) continue;
                    if (_colliderList.Contains(worldCollider)) continue;

                    Vector3 direction;
                    float distance;


                    for (int k = 0; k < _colliderList.Count; k++)
                    {
                        Collider myCollider = _colliderList[k];

                        if (Physics.GetIgnoreCollision(worldCollider, myCollider)) continue;

                        if (Physics.ComputePenetration(myCollider, myCollider.transform.position + collisionResponse, myCollider.transform.rotation, worldCollider, worldCollider.transform.position, worldCollider.transform.rotation, out direction, out distance))
                        {

                            iterationResponse += direction * distance;
                            position += direction * distance;
                            collisionResponse += direction * distance;
                            penetrated = true;
                        }
                    }

                }

                if (!penetrated || iterationResponse == Vector3.zero) break;

            }

            return position;
        }

        public static Vector3 CalculateDepenetratedSpherePosition(Vector3 center, float radius, LayerMask collisionMask)
        {
            return CalculateDepenetratedSpherePosition(center, radius, collisionMask, out Vector3 response);
        }

        public static Vector3 CalculateDepentratedPosition(GameObject obj, LayerMask layerMask, Predicate<Collider> colliderFilter = null)
        {
            return CalculateDepentratedPosition(obj, layerMask, out Vector3 response, colliderFilter);
        }


        public static Vector3 GetClosestPoint(Collider collider, Vector3 point)
        {
            if (collider is MeshCollider meshCollider && !meshCollider.convex)
            {
                return meshCollider.bounds.ClosestPoint(point);
            }

            return collider.ClosestPoint(point);
        }

        public static bool CheckCollider(Collider collider, LayerMask layerMask)
        {
            if (collider is SphereCollider sphereCollider)
            {
                Sphere3 sphere = new Sphere3(sphereCollider);
                return Physics.CheckSphere(sphere.Center, sphere.Radius, layerMask);
            }

            if (collider is BoxCollider boxCollider)
            {
                Box3 box = new Box3(boxCollider);
                return Physics.CheckBox(box.Center, box.Extents, box.Rotation, layerMask);
            }
            if (collider is CapsuleCollider capsuleCollider)
            {
                Capsule3 capsule = new Capsule3(capsuleCollider);
                return Physics.CheckCapsule(capsule.InnerStartPoint, capsule.InnerEndPoint, capsule.Radius, layerMask);
            }

            if (collider is MeshCollider meshCollider)
            {
                Debug.LogWarning("Cannot accurately check collision with a MeshCollider: using bounds instead!");

                Box3 box = new Box3(meshCollider.bounds);
                return Physics.CheckBox(box.Center, box.Extents, box.Rotation, layerMask);
            }

            throw new ArgumentException("Collider cannot be checked: " + collider);
        }


    }




}
