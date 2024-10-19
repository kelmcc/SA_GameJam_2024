using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.Serialization;


namespace Framework
{

    [ScriptedImporter(1, "ramp")]
    public class RampTextureImporter : ScriptedImporter
    {
        public enum GradientBlendMode
        {
            Interpolate,
            Multiply,
            Divide,
            Add,
            Subtract,
            Average
        }

        [Clamp(1, 1024)]
        public int Width = 256;

        [Clamp(1, 1024)]
        public int Height = 1;

        [FormerlySerializedAs("Gradient")]
        public Gradient HorizontalGradient;

        public Gradient VerticalGradient;

        public GradientBlendMode BlendMode = GradientBlendMode.Interpolate;
        public TextureWrapMode Wrap = TextureWrapMode.Clamp;
        public FilterMode Filter = FilterMode.Bilinear;

        public override void OnImportAsset(AssetImportContext context)
        {

            Texture2D texture = new Texture2D(Width, Height);
            texture.wrapMode = Wrap;
            texture.filterMode = Filter;

            Color Blend(float x, float y)
            {
                Color a = HorizontalGradient.Evaluate(x);
                Color b = VerticalGradient.Evaluate(y);

                switch (BlendMode)
                {
                    case GradientBlendMode.Interpolate: return a * x + b * y;
                    case GradientBlendMode.Multiply: return a * b;
                    case GradientBlendMode.Divide: return new Color(a.r / b.r, a.g / b.g, a.b / b.b, a.a / b.a);
                    case GradientBlendMode.Add: return a + b;
                    case GradientBlendMode.Subtract: return a - b;
                    case GradientBlendMode.Average: return (a + b) * 0.5f;
                }

                throw new ArgumentOutOfRangeException();
            }

            if (Width == 1)
            {
                if (VerticalGradient != null)
                {

                    for (int y = 0; y < Height; y++)
                    {
                        texture.SetPixel(0, y, VerticalGradient.Evaluate((float)y / Height));
                    }
                }
                else
                {

                    for (int y = 0; y < Height; y++)
                    {
                        texture.SetPixel(0, y, Color.white);
                    }
                }

            }
            else if (Height == 1)
            {
                if (HorizontalGradient != null)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        texture.SetPixel(x, 0, HorizontalGradient.Evaluate((float)x / Width));
                    }
                }
                else
                {
                    for (int x = 0; x < Width; x++)
                    {
                        texture.SetPixel(x, 0, Color.white);
                    }
                }
            }
            else
            {
                if (HorizontalGradient != null && VerticalGradient != null)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            texture.SetPixel(x, y, Blend((float)x / Width, (float)y / Height));
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            texture.SetPixel(x, y, Color.white);
                        }
                    }
                }
            }

            texture.Apply();
            context.AddObjectToAsset("texture", texture);
        }

        [MenuItem("Assets/Create/Ramp Texture")]
        public static void CreateRampTexture()
        {
            string path = EditorUtility.SaveFilePanelInProject("New Ramp Texture", "NewRamp", "ramp", "");
            if (path.Length >= 0)
            {
                File.WriteAllText(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/" + path, "");
                AssetDatabase.Refresh();
            }
        }

    }

    [CustomEditor(typeof(RampTextureImporter))]
    public class RampTextureImporterImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty width = serializedObject.FindProperty("Width");
            SerializedProperty height = serializedObject.FindProperty("Height");

            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(height);

            if (width.intValue > 1)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("HorizontalGradient"));
            }

            if (height.intValue > 1)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("VerticalGradient"));
            }

            if (height.intValue > 1 && width.intValue > 1)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BlendMode"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Wrap"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Filter"));

            serializedObject.ApplyModifiedProperties();

            base.ApplyRevertGUI();
        }


    }
}

