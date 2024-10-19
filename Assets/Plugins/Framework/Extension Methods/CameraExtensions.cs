using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
#if UNITY_EDITOR
    public class PNGRenderSettings
    {
        public int ImportSize = 512;
        public int DepthBuffer = 0;
        public bool GenerateMipMaps = true;
        public bool sRGB = false;
        public bool AlphaIsTransparency = true;
        public RenderTextureFormat RenderTextureFormat = RenderTextureFormat.Default;
        public TextureFormat TextureFormat = TextureFormat.RGBA32;
        public UnityEditor.TextureImporterAlphaSource AlphaSource = UnityEditor.TextureImporterAlphaSource.FromInput;
        public UnityEditor.TextureImporterType TextureType = UnityEditor.TextureImporterType.Default;
        public TextureWrapMode WrapMode = TextureWrapMode.Repeat;
    }
#endif

    public delegate void PNGRenderPassSetupCallback(int passIndex);

    public static class CameraExtensions
    {
        public static Frustum GetFrustum(this Camera camera)
        {
            return new Frustum(camera);
        }

        public static Ray NormalizedScreenPointToRay(this Camera camera, Vector2 normalizedScreenPoint)
        {
            return camera.ScreenPointToRay(new Vector3(normalizedScreenPoint.x * camera.pixelWidth, normalizedScreenPoint.y * camera.pixelHeight, 0));
        }

        public static Ray GetForwardRay(this Camera camera)
        {
            return camera.ScreenPointToRay(new Vector3(camera.pixelWidth * 0.5f, camera.pixelHeight * 0.5f, 0));
        }
        public static Ray GetMouseRay(this Camera camera)
        {
            return camera.ScreenPointToRay(Mouse.Position);
        }

        public static Rect GetScreenSpaceRect(this Bounds bounds, Camera camera)
        {
            float xMin = Mathf.Infinity;
            float xMax = -Mathf.Infinity;
            float yMin = Mathf.Infinity;
            float yMax = -Mathf.Infinity;

            void Compare(Vector3 point)
            {
                point = camera.WorldToScreenPoint(point);
                xMin = Mathf.Min(point.x, xMin);
                yMin = Mathf.Min(point.y, yMin);
                xMax = Mathf.Max(point.x, xMax);
                yMax = Mathf.Max(point.y, yMax);
            }

            Compare(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z));
            Compare(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
            Compare(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
            Compare(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
            Compare(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z));
            Compare(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
            Compare(new Vector3(bounds.max.x, bounds.max.y, bounds.max.z));
            Compare(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));

            Vector2 center = new Vector2(xMin + xMax, yMin + yMax) * 0.5f;
            Vector2 dimensions = new Vector2(xMax - xMin, yMax - yMin);
            return new Rect(center, dimensions);
        }

        /// <summary>
        /// Returns the dimensions of the camera in world units.
        /// </summary>
        /// <returns>Worldspace camera dimensions</returns>
        public static Vector2 WorldspaceDimensions(this Camera camera)
        {
            float height = 2 * camera.orthographicSize;
            return new Vector2(height * camera.aspect, height);
        }

        public static Rect GetScreenspaceRect(this Camera camera, Vector3 cornerA, Vector3 cornerB)
        {
            cornerA = camera.WorldToScreenPoint(cornerA);
            cornerB = camera.WorldToScreenPoint(cornerB);

            return new Rect(Mathf.Min(cornerA.x, cornerB.x), Screen.height - Mathf.Max(cornerA.y, cornerB.y), Mathf.Abs(cornerA.x - cornerB.x), Mathf.Abs(cornerA.y - cornerB.y));
        }

#if UNITY_EDITOR
        public static void RenderMultipassToPNG(this Camera camera, string path, int width, int height, PNGRenderSettings settings, int numPasses, PNGRenderPassSetupCallback passSetup)
        {

            string localPath = FileUtils.GetLocalPath(path);

            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, settings.DepthBuffer, settings.RenderTextureFormat);
            camera.targetTexture = renderTexture;

            if (passSetup == null || numPasses == 0)
            {
                camera.RenderDontRestore();

            }
            else
            {
                for (int i = 0; i < numPasses; i++)
                {
                    passSetup(i);
                    camera.RenderDontRestore();
                }
            }


            RenderTexture.active = renderTexture;

            Texture2D texture = new Texture2D(width, height, settings.TextureFormat, settings.GenerateMipMaps);

            texture.alphaIsTransparency = true;

            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            if (settings.sRGB)
            {
                ConvertToGamma(texture);
            }

            RenderTexture.active = null;
            camera.targetTexture = null;

            byte[] bytes = texture.EncodeToPNG();

            if (File.Exists(path))
            {
                Color32[] textureColours = GetColours(bytes, width, height, settings.TextureFormat, settings.GenerateMipMaps);
                Color32[] fileColours = GetColours(File.ReadAllBytes(path), width, height, settings.TextureFormat, settings.GenerateMipMaps);

                if (!CompareColours(textureColours, fileColours))
                {
                    File.WriteAllBytes(path, bytes);
                }
            }
            else
            {
                File.WriteAllBytes(path, bytes);
            }

            UnityEditor.AssetDatabase.ImportAsset(localPath);

            UnityEditor.TextureImporter importer = UnityEditor.AssetImporter.GetAtPath(localPath) as UnityEditor.TextureImporter;
            importer.alphaSource = settings.AlphaSource;
            importer.mipmapEnabled = settings.GenerateMipMaps;
            importer.alphaIsTransparency = settings.AlphaIsTransparency;
            importer.sRGBTexture = settings.sRGB;
            importer.textureType = settings.TextureType;
            importer.maxTextureSize = settings.ImportSize;
            UnityEditor.EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();


        }

        public static void RenderToPNG(this Camera camera, string path, int width, int height, PNGRenderSettings settings)
        {
            camera.RenderMultipassToPNG(path, width, height, settings, 0, null);
        }

        static void ConvertToGamma(Texture2D texture)
        {
            Color[] colours = texture.GetPixels();


            for (int i = 0; i < colours.Length; ++i)
            {

                Color colour = new Color();

                colour.r = Mathf.LinearToGammaSpace(colours[i].r);
                colour.g = Mathf.LinearToGammaSpace(colours[i].g);
                colour.b = Mathf.LinearToGammaSpace(colours[i].b);
                colour.a = colours[i].a;
                colours[i] = colour;
            }

            texture.SetPixels(colours);
            texture.Apply();
        }


        static Color32[] GetColours(byte[] bytes, int width, int height, TextureFormat textureFormat, bool mipChain)
        {
            Texture2D texture = new Texture2D(width, height, textureFormat, mipChain);

            texture.alphaIsTransparency = true;
            texture.LoadImage(bytes);

            Color32[] colours = texture.GetPixels32();
            Object.DestroyImmediate(texture);

            return colours;
        }

        static bool CompareColours(Color32[] a, Color32[] b)
        {
            const int THRESHOLD = 3;

            if (a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (Mathf.Abs(a[i].r - b[i].r) > THRESHOLD) return false;
                if (Mathf.Abs(a[i].g - b[i].g) > THRESHOLD) return false;
                if (Mathf.Abs(a[i].b - b[i].b) > THRESHOLD) return false;
                if (Mathf.Abs(a[i].a - b[i].a) > THRESHOLD) return false;
            }

            return true;
        }
#endif
    }


}
