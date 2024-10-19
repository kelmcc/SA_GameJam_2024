using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Framework
{

    /// <summary>
    /// Utility class to provide extra debug functionality.
    /// </summary>
    public static class DebugUtils
    {

#if UNITY_EDITOR
        private static List<Action> _drawActions = new List<Action>();
        private static GizmoDrawer _gizmoDrawer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _drawActions = new List<Action>();

            UnityEditor.EditorApplication.playModeStateChanged -= PlayStateChanged;
            UnityEditor.EditorApplication.pauseStateChanged -= PauseStateChanged;

            UnityEditor.EditorApplication.playModeStateChanged += PlayStateChanged;
            UnityEditor.EditorApplication.pauseStateChanged += PauseStateChanged;
        }

        private class GizmoDrawer : MonoBehaviour
        {
            void OnDrawGizmos()
            {
                if (!Runtime.IsShuttingDown)
                {
                    for (int i = 0; i < _drawActions.Count; i++)
                    {
                        _drawActions[i].Invoke();
                    }
                }

                _drawActions.Clear();
            }
        }
#endif

        static void DrawGizmo(Action drawAction)
        {
#if UNITY_EDITOR

            if (!Application.isPlaying)
            {
                drawAction();
            }
            else
            {

                if (_gizmoDrawer == null)
                {
                    _gizmoDrawer = new GameObject("Debug Gizmo Drawer").AddComponent<GizmoDrawer>();
                    _gizmoDrawer.gameObject.hideFlags = HideFlags.DontSave;

                    UnityEngine.Object.DontDestroyOnLoad(_gizmoDrawer.gameObject);

                }

                _drawActions.Add(drawAction);
            }
#endif
        }

#if UNITY_EDITOR

        static void PlayStateChanged(UnityEditor.PlayModeStateChange playState)
        {
            _drawActions.Clear();
        }

        static void PauseStateChanged(UnityEditor.PauseState pauseState)
        {
            _drawActions.Clear();
        }

        public static Vector3 GetPointWithScreenSpaceOffset(Vector3 point, float xOffset, float yOffset)
        {

            Camera camera = null;
            if (UnityEditor.SceneView.currentDrawingSceneView != null)
            {
                camera = UnityEditor.SceneView.currentDrawingSceneView.camera;
            }
            else
            {
                camera = SceneUtils.MainCamera;
            }

            point = camera.WorldToScreenPoint(point);
            return camera.ScreenToWorldPoint(point + new Vector3(xOffset, yOffset));
        }
#endif

        // --- LINE --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color colour)
        {
#if UNITY_EDITOR

            bool UseHandles()
            {
                if (Application.isPlaying && UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null) return false;
                if (!Application.isPlaying) return true;

                return false;
            }


            if (UseHandles())
            {
                UnityEditor.Handles.color = colour;
                UnityEditor.Handles.DrawLine(start, end);

            }
            else
            {
                Debug.DrawLine(start, end, colour);
            }
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end)
        {
            DrawLine(start, end, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawLine(Line3 line)
        {
            DrawLine(line.Start, line.End, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawLine(Line3 line, Color colour)
        {
            DrawLine(line.Start, line.End, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawLine(Line2 line)
        {
            DrawLine(line.Start, line.End, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawLine(Line2 line, Color colour)
        {
            DrawLine(line.Start, line.End, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCurve(Vector3[] points, Color colour)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                DrawLine(points[i], points[i + 1], colour);
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCurve(Vector3[] points)
        {
            DrawCurve(points, Color.white);
        }

        // --- RAY --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 origin, Vector3 direction, Color colour)
        {
            Debug.DrawRay(origin, direction, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 origin, Vector3 direction)
        {
            DrawRay(origin, direction, Color.white);
        }

        // --- TEXT --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawText(Vector3 position, string text, Color colour)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                GUI.contentColor = colour;
                UnityEditor.Handles.Label(position, text);
                GUI.contentColor = Color.white;
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawText(Vector3 position, string text)
        {
            DrawText(position, text, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawText(Vector3 position, object obj)
        {
            DrawText(position, obj.ToString(), Color.white);
        }


        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawText(Vector3 position, object obj, Color colour)
        {
            DrawText(position, obj.ToString(), colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawTextWithOffset(Vector3 position, Vector2 screenSpaceOffset, string text, Color colour)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                GUI.contentColor = colour;
                UnityEditor.Handles.Label(GetPointWithScreenSpaceOffset(position, screenSpaceOffset.x, screenSpaceOffset.y), text);
                GUI.contentColor = Color.white;
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawTextWithOffset(Vector3 position, Vector2 screenSpaceOffset, string text)
        {
            DrawTextWithOffset(position, screenSpaceOffset, text, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawTextWithOffset(Vector3 position, Vector2 screenSpaceOffset, object obj)
        {
            DrawTextWithOffset(position, screenSpaceOffset, obj.ToString(), Color.white);
        }


        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawTextWithOffset(Vector3 position, Vector2 screenSpaceOffset, object obj, Color colour)
        {
            DrawTextWithOffset(position, screenSpaceOffset, obj.ToString(), colour);
        }

        // --- SPHERE --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSphere(Vector3 position, float radius, Color colour)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                Gizmos.color = colour;
                Gizmos.DrawWireSphere(position, radius);
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSphere(Vector3 position, float radius)
        {
            DrawSphere(position, radius, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSphere(Sphere3 sphere)
        {
            DrawSphere(sphere.Center, sphere.Radius, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSphere(BoundingSphere sphere)
        {
            DrawSphere(sphere.position, sphere.radius, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSphere(BoundingSphere sphere, Color colour)
        {
            DrawSphere(sphere.position, sphere.radius, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSphere(Sphere3 sphere, Color colour)
        {
            DrawSphere(sphere.Center, sphere.Radius, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidSphere(Vector3 position, float radius, Color colour)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                Gizmos.color = colour;
                Gizmos.DrawSphere(position, radius);
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidSphere(Vector3 position, float radius)
        {
            DrawSolidSphere(position, radius, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidSphere(Sphere3 sphere)
        {
            DrawSolidSphere(sphere.Center, sphere.Radius, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidSphere(Sphere3 sphere, Color colour)
        {
            DrawSolidSphere(sphere.Center, sphere.Radius, colour);
        }

        // --- CONVEX POLYGON --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawConvexPolygon(Vector3[] points, Color colour)
        {
#if UNITY_EDITOR
            for (int i = 0; i < points.Length - 1; i++)
            {
                DrawLine(points[i], points[i + 1], colour);
            }

            DrawLine(points[points.Length - 1], points[0], colour);
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawConvexPolygon(Vector3[] points)
        {
            DrawConvexPolygon(points, Color.white);
        }


        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidConvexPolygon(Vector3[] points, Color colour)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                UnityEditor.Handles.color = colour;
                UnityEditor.Handles.DrawAAConvexPolygon(points);
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidConvexPolygon(Vector3[] points)
        {
            DrawSolidConvexPolygon(points, Color.white);
        }

        // --- BEZIER CURVE --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawBezierCurve(Vector3 startPoint, Vector3 endPoint, Vector3 startControlPoint, Vector3 endControlPoint, Color colour, Color controlPointColour)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                UnityEditor.Handles.DrawBezier(startPoint, endPoint, startControlPoint, endControlPoint, colour, null, 1.5f);
                DrawLine(startPoint, startControlPoint, controlPointColour);
                DrawLine(endPoint, endControlPoint, controlPointColour);
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawBezierCurve(Vector3 startPoint, Vector3 endPoint, Vector3 startControlPoint, Vector3 endControlPoint, Color colour)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                UnityEditor.Handles.DrawBezier(startPoint, endPoint, startControlPoint, endControlPoint, colour, null, 1.5f);
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawBezierCurve(Vector3 startPoint, Vector3 endPoint, Vector3 startControlPoint, Vector3 endControlPoint)
        {
            DrawBezierCurve(startPoint, endPoint, startControlPoint, endControlPoint, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawBezierCurve(BezierCurve curve, Color colour, Color controlPointColour)
        {
            DrawBezierCurve(curve.StartPoint, curve.EndPoint, curve.StartControlPoint, curve.EndControlPoint, colour, controlPointColour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawBezierCurve(BezierCurve curve, Color colour)
        {
            DrawBezierCurve(curve.StartPoint, curve.EndPoint, curve.StartControlPoint, curve.EndControlPoint, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawBezierCurve(BezierCurve curve)
        {
            DrawBezierCurve(curve.StartPoint, curve.EndPoint, curve.StartControlPoint, curve.EndControlPoint, Color.white);
        }

        // --- MESH --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color colour, int subMeshIndex = 0)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                Gizmos.color = colour;
                Gizmos.DrawWireMesh(mesh, subMeshIndex, position, rotation, scale);
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, int subMeshIndex = 0)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireMesh(mesh, subMeshIndex, position, rotation, scale);
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color colour, int subMeshIndex = 0)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                Gizmos.color = colour;
                Gizmos.DrawMesh(mesh, subMeshIndex, position, rotation, scale);
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, int subMeshIndex = 0)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                Gizmos.color = Color.white;
                Gizmos.DrawMesh(mesh, subMeshIndex, position, rotation, scale);
            });
#endif
        }

        // --- FRUSTUM --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawFrustum(Frustum frustum)
        {
            DrawFrustum(frustum, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawFrustum(Frustum frustum, Color color)
        {
            DrawLine(frustum.FarTopLeftCorner, frustum.FarTopRightCorner, color);
            DrawLine(frustum.FarTopRightCorner, frustum.FarBottomRightCorner, color);
            DrawLine(frustum.FarBottomRightCorner, frustum.FarBottomLeftCorner, color);
            DrawLine(frustum.FarBottomLeftCorner, frustum.FarTopLeftCorner, color);

            DrawLine(frustum.NearTopLeftCorner, frustum.NearTopRightCorner, color);
            DrawLine(frustum.NearTopRightCorner, frustum.NearBottomRightCorner, color);
            DrawLine(frustum.NearBottomRightCorner, frustum.NearBottomLeftCorner, color);
            DrawLine(frustum.NearBottomLeftCorner, frustum.NearTopLeftCorner, color);

            DrawLine(frustum.NearTopLeftCorner, frustum.FarTopLeftCorner, color);
            DrawLine(frustum.NearTopRightCorner, frustum.FarTopRightCorner, color);
            DrawLine(frustum.NearBottomLeftCorner, frustum.FarBottomLeftCorner, color);
            DrawLine(frustum.NearBottomRightCorner, frustum.FarBottomRightCorner, color);

        }

        // --- CAPSULE --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCylinder(Vector3 center, float radius, float height, Quaternion rotation, Color colour)
        {
            Vector3 up = rotation * Vector3.up * 0.5f;
            Vector3 right = rotation * Vector3.right * radius;
            Vector3 forward = rotation * Vector3.forward * radius;

            Vector3 top = center + up * height;
            Vector3 bottom = center - up * height;

            DrawCircle(top, radius, up, colour);
            DrawCircle(bottom, radius, up, colour);

            DrawLine(bottom + right, top + right, colour);
            DrawLine(bottom - right, top - right, colour);
            DrawLine(bottom + forward, top + forward, colour);
            DrawLine(bottom - forward, top - forward, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCylinder(Vector3 center, float radius, float height, Quaternion rotation)
        {
            DrawCylinder(center, radius, height, rotation, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCylinder(Vector3 bottomCenter, Vector3 topCenter, float radius, Color colour)
        {
            Vector3 toTop = topCenter - bottomCenter;
            DrawCylinder((bottomCenter + topCenter) * 0.5f, radius, toTop.magnitude, Quaternion.LookRotation(Vector3.forward, toTop.normalized), colour);
        }


        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCylinder(Vector3 bottomCenter, Vector3 topCenter, float radius)
        {
            DrawCylinder(bottomCenter, topCenter, radius, Color.white);
        }
        // --- CAPSULE --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCapsule(Capsule3 capsule, Color colour)
        {
            Quaternion rotation = Quaternion.LookRotation(capsule.Direction);
            Vector3 arcNormal = rotation * Vector3.right;
            Vector3 cross = Vector3.Cross(arcNormal, capsule.Direction);

            DrawCircle(capsule.InnerStartPoint, capsule.Radius, capsule.Direction, colour);
            DrawCircle(capsule.InnerEndPoint, capsule.Radius, capsule.Direction, colour);

            DrawCircularArc(capsule.InnerStartPoint, -capsule.Direction, capsule.Radius, 180f, arcNormal, colour);
            DrawCircularArc(capsule.InnerStartPoint, -capsule.Direction, capsule.Radius, 180f, cross, colour);

            DrawCircularArc(capsule.InnerEndPoint, capsule.Direction, capsule.Radius, 180f, arcNormal, colour);
            DrawCircularArc(capsule.InnerEndPoint, capsule.Direction, capsule.Radius, 180f, cross, colour);

            DrawLine(capsule.InnerStartPoint + (rotation * Vector3.up) * capsule.Radius, capsule.InnerEndPoint + (rotation * Vector3.up) * capsule.Radius, colour);
            DrawLine(capsule.InnerStartPoint + (rotation * Vector3.down) * capsule.Radius, capsule.InnerEndPoint + (rotation * Vector3.down) * capsule.Radius, colour);
            DrawLine(capsule.InnerStartPoint + (rotation * Vector3.left) * capsule.Radius, capsule.InnerEndPoint + (rotation * Vector3.left) * capsule.Radius, colour);
            DrawLine(capsule.InnerStartPoint + (rotation * Vector3.right) * capsule.Radius, capsule.InnerEndPoint + (rotation * Vector3.right) * capsule.Radius, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCapsule(Capsule3 capsule)
        {
            DrawCapsule(capsule, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCapsule(Vector3 start, Vector3 end, float radius)
        {
            DrawCapsule(new Capsule3(start, end, radius), Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCapsule(Vector3 start, Vector3 end, float radius, Color colour)
        {
            DrawCapsule(new Capsule3(start, end, radius), colour);
        }

        // --- BOX --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawBox(Box3 box, Color colour)
        {
#if UNITY_EDITOR
            Vector3[] corners = box.GetCorners();

            // Bottom
            DrawLine(corners[0], corners[1], colour);
            DrawLine(corners[1], corners[2], colour);
            DrawLine(corners[2], corners[3], colour);
            DrawLine(corners[3], corners[0], colour);

            // Top
            DrawLine(corners[4], corners[5], colour);
            DrawLine(corners[5], corners[6], colour);
            DrawLine(corners[6], corners[7], colour);
            DrawLine(corners[7], corners[4], colour);

            // Uprights
            DrawLine(corners[0], corners[4], colour);
            DrawLine(corners[1], corners[5], colour);
            DrawLine(corners[2], corners[6], colour);
            DrawLine(corners[3], corners[7], colour);
#endif
        }


        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawBox(Vector3 center, Vector3 dimensions, Quaternion rotation, Color colour)
        {
#if UNITY_EDITOR
            Vector3 extents = dimensions * 0.5f;

            Vector3 frontTopLeft = rotation * new Vector3(-extents.x, extents.y, -extents.z) + center;
            Vector3 frontTopRight = rotation * new Vector3(extents.x, extents.y, -extents.z) + center;
            Vector3 frontBottomLeft = rotation * new Vector3(-extents.x, -extents.y, -extents.z) + center;
            Vector3 frontBottomRight = rotation * new Vector3(extents.x, -extents.y, -extents.z) + center;
            Vector3 backTopLeft = rotation * new Vector3(-extents.x, extents.y, extents.z) + center;
            Vector3 backTopRight = rotation * new Vector3(extents.x, extents.y, extents.z) + center;
            Vector3 backBottomLeft = rotation * new Vector3(-extents.x, -extents.y, extents.z) + center;
            Vector3 backBottomRight = rotation * new Vector3(extents.x, -extents.y, extents.z) + center;

            DrawLine(frontTopLeft, frontTopRight, colour);
            DrawLine(frontTopRight, frontBottomRight, colour);
            DrawLine(frontBottomRight, frontBottomLeft, colour);
            DrawLine(frontBottomLeft, frontTopLeft, colour);

            DrawLine(backTopLeft, backTopRight, colour);
            DrawLine(backTopRight, backBottomRight, colour);
            DrawLine(backBottomRight, backBottomLeft, colour);
            DrawLine(backBottomLeft, backTopLeft, colour);

            DrawLine(frontTopLeft, backTopLeft, colour);
            DrawLine(frontTopRight, backTopRight, colour);
            DrawLine(frontBottomRight, backBottomRight, colour);
            DrawLine(frontBottomLeft, backBottomLeft, colour);
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawBox(Vector3 center, Vector3 dimensions, Quaternion rotation)
        {
            DrawBox(center, dimensions, rotation, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawBox(Box3 box)
        {
            DrawBox(box, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidBox(Vector3 center, Vector3 dimensions, Quaternion rotation, Color colour)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                using (new TemporaryGizmoMatrix(Matrix4x4.TRS(center, rotation, dimensions)))
                {
                    Gizmos.color = colour;
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);
                }
            });
#endif
        }


        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidBox(Box3 box, Color colour)
        {
            DrawSolidBox(box.Center, box.Dimensions, box.Rotation, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidBox(Vector3 center, Vector3 dimensions, Quaternion rotation)
        {
            DrawSolidBox(center, dimensions, rotation, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidBox(Box3 box)
        {
            DrawSolidBox(box, Color.white);
        }

        // --- SHAPE --- ///

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawShape(IPhysicsShape3D shape, Color colour)
        {
            if (shape is Box3 box)
            {
                DrawBox(box, colour);
            }
            else if (shape is Sphere3 sphere)
            {
                DrawSphere(sphere);
            }
            else if (shape is Capsule3 capsule)
            {
                DrawCapsule(capsule);
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawShape(IPhysicsShape3D shape)
        {
            DrawShape(shape, Color.white);
        }

        // --- RECTANGLE --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRectangle(Vector3 center, Vector2 dimensions, Quaternion rotation, Color colour)
        {
#if UNITY_EDITOR
            Vector3 extents = dimensions * 0.5f;

            Vector3 frontTopLeft = rotation * new Vector3(-extents.x, extents.y, 0) + center;
            Vector3 frontTopRight = rotation * new Vector3(extents.x, extents.y, 0) + center;
            Vector3 frontBottomLeft = rotation * new Vector3(-extents.x, -extents.y, 0) + center;
            Vector3 frontBottomRight = rotation * new Vector3(extents.x, -extents.y, 0) + center;

            DrawLine(frontTopLeft, frontTopRight, colour);
            DrawLine(frontTopRight, frontBottomRight, colour);
            DrawLine(frontBottomRight, frontBottomLeft, colour);
            DrawLine(frontBottomLeft, frontTopLeft, colour);
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRectangle(Vector3 center, Vector2 dimensions, Color colour)
        {
#if UNITY_EDITOR
            if (UnityEditor.SceneView.lastActiveSceneView != null)
            {
                if (UnityEditor.SceneView.lastActiveSceneView.in2DMode)
                {
                    DrawRectangle(center, dimensions, UnityEditor.SceneView.lastActiveSceneView.rotation, colour);
                }
                else
                {
                    DrawRectangle(center, dimensions, Quaternion.LookRotation(UnityEditor.SceneView.lastActiveSceneView.camera.transform.position - center), colour);
                }
            }
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRectangle(Vector3 center, Vector2 dimensions)
        {
            DrawRectangle(center, dimensions, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRectangle(Vector3 center, Vector2 dimensions, Quaternion rotation)
        {
            DrawRectangle(center, dimensions, rotation, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRectangle(Rect rect, Color colour)
        {
            DrawRectangle(rect.center, rect.size, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRectangle(Rect rect)
        {
            DrawRectangle(rect.center, rect.size, Color.white);
        }

        // --- SQUARE --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSquare(Vector3 center, float width, Quaternion rotation, Color colour)
        {
            DrawRectangle(center, new Vector2(width, width), rotation, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSquare(Vector3 center, float width, Quaternion rotation)
        {
            DrawRectangle(center, new Vector2(width, width), rotation, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSquare(Vector3 center, float width, Color colour)
        {
            DrawRectangle(center, new Vector2(width, width), colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSquare(Vector3 center, float width)
        {
            DrawRectangle(center, new Vector2(width, width), Color.white);
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidSquare(Vector3 center, float width, Vector3 normal, Color colour)
        {
            DrawSolidRectangle(center, new Vector2(width, width), normal, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidSquare(Vector3 center, float width, Vector3 normal)
        {
            DrawSolidRectangle(center, new Vector2(width, width), normal, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidSquare(Vector3 center, float width, Color colour)
        {
            DrawSolidRectangle(center, new Vector2(width, width), colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidSquare(Vector3 center, float width)
        {
            DrawSolidRectangle(center, new Vector2(width, width), Color.white);
        }

        // --- ARROW --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrow(Vector3 start, Vector3 end, Vector3 normal, float capLength, Color colour)
        {
#if UNITY_EDITOR
            if (start != end)
            {
                Quaternion rotation = Quaternion.LookRotation((end - start).normalized, normal);
                Vector3 capOffset = capLength * new Vector3(0.57735026919f, 0, 0); // Tan(30)
                Vector3 capStart = end - ((end - start).normalized * capLength);

                DrawLine(start, end, colour);
                DrawLine(capStart + rotation * capOffset, end, colour);
                DrawLine(capStart - rotation * capOffset, end, colour);
            }
#endif
        }




        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrow(Vector3 start, Vector3 end, float capLength, Color colour)
        {
#if UNITY_EDITOR
            if (UnityEditor.SceneView.lastActiveSceneView != null && start != end)
            {
                Quaternion rotation;

                if (UnityEditor.SceneView.lastActiveSceneView.in2DMode)
                {
                    rotation = Quaternion.LookRotation((end - start).normalized) * Quaternion.Euler(0, 0, 90) * UnityEditor.SceneView.lastActiveSceneView.rotation;
                }
                else
                {
                    rotation = Quaternion.LookRotation((end - start).normalized, UnityEditor.SceneView.lastActiveSceneView.rotation.GetForwardVector());
                }

                Vector3 capOffset = capLength * new Vector3(0.57735026919f, 0, 0); // Tan(30)
                Vector3 capStart = end - ((end - start).normalized * capLength);

                DrawLine(start, end, colour);
                DrawLine(capStart + rotation * capOffset, end, colour);
                DrawLine(capStart + rotation * -capOffset, end, colour);
            }
#endif
        }


        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrow(Vector3 start, Vector3 end, Vector3 normal, Color colour)
        {
            DrawArrow(start, end, normal, 0.1f, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrow(Vector3 start, Vector3 end, Color colour)
        {
            DrawArrow(start, end, 0.1f, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrow(Vector3 start, Vector3 end, float capLength)
        {
            DrawArrow(start, end, capLength, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrow(Vector3 start, Vector3 end, float capLength, Vector3 normal)
        {
            DrawArrow(start, end, normal, capLength, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrow(Vector3 start, Vector3 end)
        {
            DrawArrow(start, end, 0.1f, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrow(Vector3 start, Vector3 end, Vector3 normal)
        {
            DrawArrow(start, end, normal, 0.1f, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrowRay(Vector3 start, Vector3 direction, Color colour)
        {
            DrawArrow(start, start + direction, 0.1f, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrowRay(Vector3 start, Vector3 direction, Vector3 normal, Color colour)
        {
            DrawArrow(start, start + direction, normal, 0.1f, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrowRay(Vector3 start, Vector3 direction)
        {
            DrawArrow(start, start + direction, 0.1f, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrowRay(Vector3 start, Vector3 direction, Vector3 normal)
        {
            DrawArrow(start, start + direction, normal, 0.1f, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrowRay(Vector3 start, Vector3 direction, float length, Color colour)
        {
            DrawArrow(start, start + direction * length, Mathf.Clamp(length * 0.1f, 0.01f, 0.1f), colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrowRay(Vector3 start, Vector3 direction, float length, Vector3 normal, Color colour)
        {
            DrawArrow(start, start + direction * length, normal, Mathf.Clamp(length * 0.1f, 0.01f, 0.1f), colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrowRay(Vector3 start, Vector3 direction, float length)
        {
            DrawArrow(start, start + direction * length, Mathf.Clamp(length * 0.1f, 0.01f, 0.1f), Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrowRay(Vector3 start, Vector3 direction, float length, Vector3 normal)
        {
            DrawArrow(start, start + direction * length, normal, Mathf.Clamp(length * 0.1f, 0.01f, 0.1f), Color.white);
        }

        // --- CIRCLE --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircle(Vector3 center, float radius, Vector3 normal, Color colour)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                UnityEditor.Handles.color = colour;
                UnityEditor.Handles.DrawWireDisc(center, normal, radius);
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircle(Vector3 center, float radius, Color colour)
        {
#if UNITY_EDITOR
            if (UnityEditor.SceneView.lastActiveSceneView != null)
            {
                if (UnityEditor.SceneView.lastActiveSceneView.in2DMode)
                {
                    DrawCircle(center, radius, UnityEditor.SceneView.lastActiveSceneView.rotation.GetForwardVector(), colour);
                }
                else
                {
                    DrawCircle(center, radius, UnityEditor.SceneView.lastActiveSceneView.camera.transform.position - center, colour);
                }
            }
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircle(Vector3 center, float radius, Vector3 normal)
        {
            DrawCircle(center, radius, normal, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircle(Vector3 center, float radius)
        {
            DrawCircle(center, radius, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidCircle(Vector3 center, float radius, Vector3 normal, Color colour)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                UnityEditor.Handles.color = colour;
                UnityEditor.Handles.DrawSolidDisc(center, normal, radius);
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidCircle(Vector3 center, float radius, Color colour)
        {
#if UNITY_EDITOR
            if (UnityEditor.SceneView.lastActiveSceneView != null)
            {
                if (UnityEditor.SceneView.lastActiveSceneView.in2DMode)
                {
                    DrawSolidCircle(center, radius, UnityEditor.SceneView.lastActiveSceneView.rotation.GetForwardVector(), colour);
                }
                else
                {
                    DrawSolidCircle(center, radius, UnityEditor.SceneView.lastActiveSceneView.camera.transform.position - center, colour);
                }
            }
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidCircle(Vector3 center, float radius, Vector3 normal)
        {
            DrawSolidCircle(center, radius, normal, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidCircle(Vector3 center, float radius)
        {
            DrawSolidCircle(center, radius, Color.white);
        }

        // --- CIRCULAR ARC --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircularArc(Vector3 origin, Vector3 direction, float radius, float angle, Vector3 normal, Color colour)
        {
#if UNITY_EDITOR
            if (angle >= 360f)
            {
                DrawCircle(origin, radius, normal, colour);
            }
            else if (angle > 0)
            {
                int numSegments = Mathf.FloorToInt(Mathf.Clamp(radius * (angle / 100f), 8f, 360f)) * 2;
                float interval = angle / numSegments;

                Quaternion rotation = Quaternion.LookRotation(direction.normalized, normal) * Quaternion.Euler(0, -angle * 0.5f, 0);
                Vector3 forward = Vector3.forward * radius;
                Vector3 lastVertex = origin + rotation * forward;

                for (int i = 0; i <= numSegments; i++)
                {
                    Vector3 vertex = origin + rotation * Quaternion.Euler(0, interval * i, 0) * forward;
                    DrawLine(lastVertex, vertex, colour);
                    lastVertex = vertex;
                }
            }
#endif
        }


        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircularArc(Vector3 origin, Vector3 direction, float radius, float angle, Vector3 normal)
        {
            DrawCircularArc(origin, direction, radius, angle, normal, Color.white);
        }



        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidCircularArc(Vector3 origin, Vector3 direction, float radius, float angle, Vector3 normal, Color colour)
        {
#if UNITY_EDITOR
            DrawGizmo(() =>
            {
                UnityEditor.Handles.color = colour;
                UnityEditor.Handles.DrawSolidArc(origin, normal, direction, angle, radius);
            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidCircularArc(Vector3 origin, Vector3 direction, float radius, float angle, Vector3 normal)
        {
            DrawSolidCircularArc(origin, direction, radius, angle, normal, Color.white);
        }

        // --- CIRCULAR SECTOR --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircularSector(Vector3 origin, Vector3 direction, float radius, float angle, Vector3 normal, Color colour)
        {
#if UNITY_EDITOR
            if (angle >= 360f)
            {
                DrawCircle(origin, radius, normal, colour);
            }
            else if (angle > 0)
            {
                DrawCircularArc(origin, direction, radius, angle, normal, colour);

                Quaternion rotation = Quaternion.LookRotation(direction.normalized, normal);
                DrawLine(origin, origin + rotation * Quaternion.Euler(0, -angle * 0.5f, 0) * Vector3.forward * radius, colour);
                DrawLine(origin, origin + rotation * Quaternion.Euler(0, angle * 0.5f, 0) * Vector3.forward * radius, colour);
            }
#endif
        }


        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircularSector(Vector3 origin, Vector3 direction, float radius, float angle, Vector3 normal)
        {
            DrawCircularSector(origin, direction, radius, angle, normal, Color.white);
        }


        // --- ANNULUS SECTOR --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawAnnulusSector(Vector3 origin, Vector3 direction, float innerRadius, float outerRadius, float angle, Vector3 normal, Color colour)
        {
#if UNITY_EDITOR
            if (angle >= 360f)
            {
                DrawCircle(origin, innerRadius, normal, colour);
                DrawCircle(origin, outerRadius, normal, colour);
            }
            else if (angle > 0)
            {
                DrawCircularArc(origin, direction, innerRadius, angle, normal, colour);
                DrawCircularArc(origin, direction, outerRadius, angle, normal, colour);

                Quaternion rotationA = Quaternion.LookRotation(direction.normalized, normal) * Quaternion.Euler(0, -angle * 0.5f, 0);
                Quaternion rotationB = Quaternion.LookRotation(direction.normalized, normal) * Quaternion.Euler(0, angle * 0.5f, 0);

                DrawLine(origin + rotationA * Vector3.forward * innerRadius, origin + rotationA * Vector3.forward * outerRadius, colour);
                DrawLine(origin + rotationB * Vector3.forward * innerRadius, origin + rotationB * Vector3.forward * outerRadius, colour);
            }
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawAnnulusSector(Vector3 origin, Vector3 direction, float innerRadius, float outerRadius, float angle, Vector3 normal)
        {
            DrawAnnulusSector(origin, direction, innerRadius, outerRadius, angle, normal, Color.white);
        }


        // --- DOTTED LINE --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawDottedLine(Vector3 start, Vector3 end, float dashLength, float gapLength, Color colour)
        {
#if UNITY_EDITOR
            if (dashLength <= 0 || gapLength <= 0) throw new ArgumentException();

            float length = (end - start).magnitude;
            Vector3 increment = (end - start) / length;

            for (float t = 0; t < length; t += dashLength + gapLength)
            {
                DrawLine(start + increment * t, start + increment * Mathf.Min(t + dashLength, length), colour);
            }
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawDottedLine(Vector3 start, Vector3 end, float dashLength, float gapLength)
        {
            DrawDottedLine(start, end, dashLength, gapLength, Color.white);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawDottedLine(Vector3 start, Vector3 end, Color colour)
        {
            DrawDottedLine(start, end, 0.25f, 0.25f, colour);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawDottedLine(Vector3 start, Vector3 end)
        {
            DrawDottedLine(start, end, 0.25f, 0.25f, Color.white);
        }

        // --- SOLID RECTANGLE --- //

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidRectangle(Vector3 position, Vector2 dimensions, Vector3 normal, Color colour)
        {
#if UNITY_EDITOR

            DrawGizmo(() =>
            {
                Matrix4x4 originalMatrix = Gizmos.matrix;
                Gizmos.color = colour;
                Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.LookRotation(normal), Vector3.one);
                Gizmos.DrawCube(Vector3.zero, new Vector3(dimensions.x, dimensions.y, 0.001f));
                Gizmos.matrix = originalMatrix;

            });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSolidRectangle(Vector3 position, Vector2 dimensions, Color colour)
        {
#if UNITY_EDITOR

            if (UnityEditor.SceneView.lastActiveSceneView != null)
            {
                if (UnityEditor.SceneView.lastActiveSceneView.in2DMode)
                {
                    DrawSolidRectangle(position, dimensions, UnityEditor.SceneView.lastActiveSceneView.rotation.GetForwardVector(), colour);
                }
                else
                {
                    DrawSolidRectangle(position, dimensions, UnityEditor.SceneView.lastActiveSceneView.camera.transform.position - position, colour);
                }
            }

#endif
        }

        /// <summary>
        /// Throws an error if a type is not an Enum type.
        /// </summary>
        /// <param name="T">The type to check</param>
        /// <param name="errorMessage">The optional error message to print to the Unity console if the assertion fails</param>
        [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
        public static void AssertIsEnumType<T>(string errorMessage = null)
        {
            if (!typeof(T).IsEnum) { throw new UnityException(errorMessage == null ? "Assert failed, Type is not an enum!" : "Assert failed: " + errorMessage); }
        }

        /// <summary>
        /// Throws an error if a type is not an Enum type.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="errorMessage">The optional error message to print to the Unity console if the assertion fails</param>
        [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
        public static void AssertIsEnumType(Type type, string errorMessage = null)
        {
            if (!type.IsEnum) { throw new UnityException(errorMessage == null ? "Assert failed, Type is not an enum!" : "Assert failed: " + errorMessage); }
        }

        /// <summary>
        /// Throws an error if a float value is not in the 0-1 range. Method (and calls) are only compiled in if this is an editor build.
        /// </summary>
        /// <param name="value">The float to check</param>
        /// <param name="errorMessage">The optional error message to print to the Unity console if the assertion fails</param>
        [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
        public static void AssertNormalized(float value, string errorMessage = null)
        {
            if (!(value >= 0 && value <= 1)) { throw new UnityException(errorMessage == null ? "Assert failed, value not normalized: " + value : "Assert failed: " + errorMessage); }
        }

        /// <summary>S
        /// Throws an error if an index is out of the valid range for an IList list. Method (and calls) are only compiled in if this is an editor build.
        /// </summary>
        /// <param name="index">The index to check</param>
        /// <param name="list">The list to check</param>
        /// <param name="errorMessage">The optional error message to print to the Unity console if the assertion fails</param>
        [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
        public static void AssertValidIndex(int index, IList list, string errorMessage = null)
        {
            if (!(index >= 0 && index < list.Count)) { throw new UnityException(errorMessage == null ? "Assert failed, index is invalid: " + index : "Assert failed: " + errorMessage); }
        }

        /// <summary>
        /// Throws an error if a list is empty. Method (and calls) are only compiled in if this is an editor build.
        /// </summary>
        /// <param name="collection">The list to check</param>
        /// <param name="errorMessage">The optional error message to print to the Unity console if the assertion fails</param>
        [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
        public static void AssertNotEmpty(ICollection collection, string errorMessage = null)
        {
            if (collection.Count <= 0) { throw new UnityException(errorMessage == null ? "Assert failed, list is empty." : "Assert failed: " + errorMessage); }
        }

        /// <summary>
        /// Throws an error if a float value is not in a specific range. Method (and calls) are only compiled in if this is an editor build.
        /// </summary>
        /// <param name="value">The float to check</param>
        /// <param name="lowerBound">The lower bound</param>
        /// <param name="upperBound">The upper bound</param>
        /// <param name="errorMessage">The optional error message to print to the Unity console if the assertion fails</param>
        [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
        public static void AssertInRange(float value, float lowerBound, float upperBound, string errorMessage = null)
        {
            if (!(value >= lowerBound && value <= upperBound)) { throw new UnityException(errorMessage == null ? "Assert failed, value not in range: " + value : "Assert failed: " + errorMessage); }
        }

        /// <summary>
        /// Throws an error if a int value is not in a specific range. Method (and calls) are only compiled in if this is an editor build.
        /// </summary>
        /// <param name="value">The int to check</param>
        /// <param name="lowerBound">The lower bound</param>
        /// <param name="upperBound">The upper bound</param>
        /// <param name="errorMessage">The optional error message to print to the Unity console if the assertion fails</param>
        [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
        public static void AssertInRange(int value, int lowerBound, int upperBound, string errorMessage = null)
        {
            if (!(value >= lowerBound && value <= upperBound)) { throw new UnityException(errorMessage == null ? "Assert failed, value not in range: " + value : "Assert failed: " + errorMessage); }
        }


        /// <summary>
        /// Logs all the elements of a list out to the Unity console.
        /// </summary>
        /// <param name="collection">The list to log</param>
        /// <param name="oneLine">Whether or not to log the entire colleciton on one line</param>
        [HideInCallstack]
        public static void LogCollection(IEnumerable collection, bool oneLine = true)
        {
            bool isEmpty = true;

            if (oneLine)
            {
                StringBuilder builder = new StringBuilder();

                foreach (object obj in collection)
                {
                    isEmpty = false;
                    builder.Append(obj);
                    builder.Append(", ");
                }

                if (!isEmpty)
                {
                    builder.Remove(builder.Length - 2, 2);
                    UnityEngine.Debug.Log(builder);
                }
            }
            else
            {
                foreach (object obj in collection)
                {
                    isEmpty = false;
                    UnityEngine.Debug.Log(obj);
                }
            }

            if (isEmpty)
            {
                UnityEngine.Debug.Log("Empty collection.");
            }
        }




    }
}
