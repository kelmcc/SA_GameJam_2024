using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    /// <summary>
    /// Utility class for drawing meshes and such like graphical operations.
    /// </summary>
    public static class DrawingUtils
    {

        private static Texture2D _antialiasingLineTexture = null;
        private static Texture2D _lineTexture = null;
        private static Material _blitMaterial = null;
        private static Material _blendMaterial = null;
        private static Rect _lineRect = new Rect(0, 0, 1, 1);

        /// <summary>
        /// Draws a 2D line on the screen. 
        /// </summary>
        /// <param name="start">The starting point of the line</param>
        /// <param name="end">The end point of the line</param>
        public static void DrawLine(Vector2 start, Vector2 end)
        {
            DrawLine(start, end, GUI.contentColor, 1f, true);
        }

        /// <summary>
        /// Draws a 2D line on the screen. 
        /// </summary>
        /// <param name="start">The starting point of the line</param>
        /// <param name="end">The end point of the line</param>
        /// <param name="colour">The colour to draw the line</param>
        /// <param name="antiAlias">Whether or not to use antialiasing when drawing</param>
        public static void DrawLine(Vector2 start, Vector2 end, Color colour, bool antiAlias = true)
        {
            DrawLine(start, end, colour, 1f, antiAlias);
        }

        /// <summary>
        /// Draws a 2D line on the screen.
        /// </summary>
        /// <param name="start">The starting point of the line</param>
        /// <param name="end">The end point of the line</param>
        /// <param name="colour">The colour to draw the line</param>
        /// <param name="width">The width to draw the line</param>
        /// <param name="antiAlias">Whether or not to use antialiasing when drawing</param>
        public static void DrawLine(Vector2 start, Vector2 end, Color colour, float width, bool antiAlias)
        {
            //This code is all courtesy of Yoyo, a member of the Unity forums (http://forum.unity3d.com/threads/drawing-lines-in-the-editor.71979/)

            // Normally the static initializer does this, but to handle texture reinitialization
            // after editor play mode stops we need this check in the Editor.
#if UNITY_EDITOR
            if (!_lineTexture)
            {
                if (_lineTexture == null)
                {
                    _lineTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    _lineTexture.SetPixel(0, 1, Color.white);
                    _lineTexture.Apply();
                }
                if (_antialiasingLineTexture == null)
                {
                    // TODO: better anti-aliasing of wide lines with a larger texture? or use Graphics.DrawTexture with border settings
                    _antialiasingLineTexture = new Texture2D(1, 3, TextureFormat.ARGB32, false);
                    _antialiasingLineTexture.SetPixel(0, 0, new Color(1, 1, 1, 0));
                    _antialiasingLineTexture.SetPixel(0, 1, Color.white);
                    _antialiasingLineTexture.SetPixel(0, 2, new Color(1, 1, 1, 0));
                    _antialiasingLineTexture.Apply();
                }

                // GUI.blitMaterial and GUI.blendMaterial are used internally by GUI.DrawTexture,
                // depending on the alphaBlend parameter. Use reflection to "borrow" these references.
                _blitMaterial = (Material)typeof(GUI).GetMethod("get_blitMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
                _blendMaterial = (Material)typeof(GUI).GetMethod("get_blendMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
            }
#endif

            // Note that theta = atan2(dy, dx) is the angle we want to rotate by, but instead
            // of calculating the angle we just use the sine (dy/len) and cosine (dx/len).
            float dx = end.x - start.x;
            float dy = end.y - start.y;
            float len = Mathf.Sqrt(dx * dx + dy * dy);

            // Early out on tiny lines to avoid divide by zero.
            // Plus what's the point of drawing a line 1/1000th of a pixel long??
            if (len < 0.001f)
            {
                return;
            }

            // Pick texture and material (and tweak width) based on anti-alias setting.
            Texture2D tex;
            Material mat;
            if (antiAlias)
            {
                // Multiplying by three is fine for anti-aliasing width-1 lines, but make a wide "fringe"
                // for thicker lines, which may or may not be desirable.
                width = width * 3.0f;
                tex = _antialiasingLineTexture;
                mat = _blendMaterial;
            }
            else
            {
                tex = _lineTexture;
                mat = _blitMaterial;
            }

            float wdx = width * dy / len;
            float wdy = width * dx / len;

            Matrix4x4 matrix = Matrix4x4.identity;
            matrix.m00 = dx;
            matrix.m01 = -wdx;
            matrix.m03 = start.x + 0.5f * wdx;
            matrix.m10 = dy;
            matrix.m11 = wdy;
            matrix.m13 = start.y - 0.5f * wdy;

            // Use GL matrix and Graphics.DrawTexture rather than GUI.matrix and GUI.DrawTexture,
            // for better performance. (Setting GUI.matrix is slow, and GUI.DrawTexture is just a
            // wrapper on Graphics.DrawTexture.)
            GL.PushMatrix();
            GL.MultMatrix(matrix);
            Graphics.DrawTexture(_lineRect, tex, _lineRect, 0, 0, 0, 0, colour, mat);
            GL.PopMatrix();
        }

        public static void DrawGameObjectBuffer(GameObject gameObject, Vector3 position, Quaternion rotation, Vector3 scale, Camera camera, Material materialOverride, int layer = 0)
        {

            CommandBuffer buffer = new CommandBuffer();
            camera.AddCommandBuffer(CameraEvent.AfterLighting, buffer);

            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale) * gameObject.transform.GetLocalTRSMatrix().inverse;

            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(true);
            for (int i = 0; i < meshFilters.Length; i++)
            {
                Mesh mesh = meshFilters[i].sharedMesh;
                if (mesh != null)
                {
                    Renderer renderer = meshFilters[i].GetComponent<Renderer>();
                    if (renderer != null && renderer.enabled)
                    {
                        Material[] mats = renderer.sharedMaterials;
                        Matrix4x4 filterMatrix = matrix * renderer.transform.GetTRSMatrix();

                        for (int j = 0; j < mesh.subMeshCount; j++)
                        {
                            if (materialOverride != null && materialOverride.HasProperty("_MainTex"))
                            {
                                materialOverride.mainTexture = mats[j].mainTexture;
                            }

                            buffer.DrawRenderer(renderer, materialOverride == null ? mats[j] : materialOverride, j);

                            Graphics.DrawMesh(mesh, filterMatrix, materialOverride == null ? mats[j] : materialOverride, layer, camera, j);
                        }
                    }
                }
            }

            Graphics.ExecuteCommandBuffer(buffer);

        }


        /// <summary>
        /// Renders all the meshfilter meshes of a GameObject with a specific transformation and a specific material.
        /// </summary>
        /// <param name="gameObject">The GameObject to render</param>
        /// <param name="position">The position of the meshes</param>
        /// <param name="rotation">The rotation of the meshes</param>
        /// <param name="scale">The scale of the meshes</param>
        /// <param name="material">The material to render the meshes </param>
        /// <param name="camera">The camera to render the meshes</param>
        /// <param name="layer">The layer to render the meshes on</param>
        public static void DrawGameObject(GameObject gameObject, Vector3 position, Quaternion rotation, Vector3 scale, Camera camera, Material material, int layer = 0)
        {


            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(false);
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale) * gameObject.transform.GetLocalTRSMatrix().inverse;

            for (int i = 0; i < meshFilters.Length; i++)
            {
                Renderer renderer = meshFilters[i].GetComponent<Renderer>();
                if (renderer != null && renderer.enabled)
                {
                    DrawMeshFilter(meshFilters[i], matrix * meshFilters[i].transform.GetTRSMatrix(), material, camera, layer, renderer);
                }
            }


            SkinnedMeshRenderer[] skinnedMeshes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(false);
            for (int i = 0; i < skinnedMeshes.Length; i++)
            {
                if (skinnedMeshes[i].enabled)
                {
                    DrawSkinnedMesh(skinnedMeshes[i], matrix * skinnedMeshes[i].transform.GetTRSMatrix(), material, camera, layer);
                }
            }


        }


        /// <summary>
        /// Renders all the meshfilter meshes of a GameObject with a specific transformation. Each meshfilter will use its meshrenderer's material.
        /// </summary>
        /// <param name="gameObject">The GameObject to render</param>
        /// <param name="position">The position of the meshes</param>
        /// <param name="rotation">The rotation of the meshes</param>
        /// <param name="scale">The scale of the meshes</param>
        /// <param name="camera">The camera to render the meshes</param>
        /// <param name="layer">The layer to render the meshes on</param>
        public static void DrawGameObject(GameObject gameObject, Vector3 position, Quaternion rotation, Vector3 scale, Camera camera, int layer = 0)
        {
            DrawGameObject(gameObject, position, rotation, scale, camera, null, layer);
        }

        /// <summary>
        /// Renders a mesh with a specific transformation and a specific material, ignoring the transform of the meshfilter itself.
        /// </summary>
        /// <param name="meshFilter">The meshfilter of the mesh to be rendered</param>
        /// <param name="position">The position of the mesh</param>
        /// <param name="rotation">The rotation of the mesh</param>
        /// <param name="scale">The scale of the mesh</param>
        /// <param name="material">The material to render the mesh with</param>
        /// <param name="camera">The camera to render the mesh with</param>
        /// <param name="layer">The layer to render the mesh on</param>
        public static void DrawMesh(MeshFilter meshFilter, Vector3 position, Quaternion rotation, Vector3 scale, Camera camera, Material material, int layer = 0)
        {
            DrawMeshFilter(meshFilter, Matrix4x4.TRS(position, rotation, scale), material, camera, layer, null);
        }


        /// <summary>
        /// Renders a mesh by combining a specific transformation and the transformation of the meshfilter iteself.
        /// </summary>
        /// <param name="meshFilter">The meshfilter of the mesh to be rendered</param>
        /// <param name="position">The position of the mesh</param>
        /// <param name="rotation">The rotation of the mesh</param>
        /// <param name="scale">The scale of the mesh</param>
        /// <param name="material">The material to render the mesh with</param>
        /// <param name="camera">The camera to render the mesh with</param>
        /// <param name="layer">The layer to render the mesh on</param>
        public static void DrawMeshTransformed(MeshFilter meshFilter, Vector3 position, Quaternion rotation, Vector3 scale, Camera camera, Material material, int layer = 0)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale) * Matrix4x4.TRS(meshFilter.transform.position, meshFilter.transform.rotation, meshFilter.transform.lossyScale);
            DrawMeshFilter(meshFilter, matrix, material, camera, layer, null);

        }

        /// <summary>
        /// Renders a mesh with a specific transformation and a specific material.
        /// </summary>
        /// <param name="mesh">The mesh to render</param>
        /// <param name="position">The position of the mesh</param>
        /// <param name="rotation">The rotation of the mesh</param>
        /// <param name="scale">The scale of the mesh</param>
        /// <param name="material">The material to render the mesh with</param>
        /// <param name="camera">The camera to render the mesh with</param>
        /// <param name="layer">The layer to render the mesh on</param>
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Camera camera, Material material, int layer = 0)
        {
            for (int j = 0; j < mesh.subMeshCount; j++)
            {
                Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, scale), material, layer, camera, j);
            }
        }

        static void DrawSkinnedMesh(SkinnedMeshRenderer renderer, Matrix4x4 matrix, Material material, Camera camera, int layer)
        {
            Mesh mesh = renderer.sharedMesh;
            if (mesh != null)
            {


                Material[] mats = renderer == null ? null : renderer.sharedMaterials;
                Material submeshMaterial = null;

                for (int j = 0; j < mesh.subMeshCount; j++)
                {
                    if (material != null)
                    {
                        submeshMaterial = material;
                    }
                    else if (mats != null && mats.Length > j)
                    {
                        submeshMaterial = mats[j];
                    }

                    if (submeshMaterial != null)
                    {
                        Texture oldTexture = null;
                        bool hasMainTexture = material != null && material.HasProperty("_MainTex");
                        if (hasMainTexture)
                        {
                            oldTexture = material.mainTexture;
                            material.mainTexture = submeshMaterial.mainTexture;
                        }

                        Graphics.DrawMesh(mesh, matrix, material == null ? submeshMaterial : material, layer, camera, j);

                        if (hasMainTexture)
                        {
                            material.mainTexture = oldTexture;
                        }
                    }

                }
            }
        }

        static void DrawMeshFilter(MeshFilter meshFilter, Matrix4x4 matrix, Material material, Camera camera, int layer, Renderer renderer)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh != null)
            {
                if (renderer == null)
                {
                    renderer = meshFilter.GetComponent<Renderer>();
                }

                Material[] mats = renderer == null ? null : renderer.sharedMaterials;
                Material submeshMaterial = null;

                for (int j = 0; j < mesh.subMeshCount; j++)
                {
                    if (material != null)
                    {
                        submeshMaterial = material;
                    }
                    else if (mats != null && mats.Length > j)
                    {
                        submeshMaterial = mats[j];
                    }

                    if (submeshMaterial != null)
                    {
                        Texture oldTexture = null;
                        bool hasMainTexture = material != null && material.HasProperty("_MainTex");
                        if (hasMainTexture)
                        {
                            oldTexture = material.mainTexture;
                            material.mainTexture = submeshMaterial.mainTexture;
                        }

                        Graphics.DrawMesh(mesh, matrix, material == null ? submeshMaterial : material, layer, camera, j);

                        if (hasMainTexture)
                        {
                            material.mainTexture = oldTexture;
                        }
                    }

                }
            }
        }

    }
}
