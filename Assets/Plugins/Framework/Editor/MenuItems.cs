using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Framework
{
    public static class MenuItems
    {
        [MenuItem("GameObject/Selection/Copy Transforms", true)]
        [MenuItem("GameObject/Selection/Paste Transforms", true)]
        [MenuItem("GameObject/Selection/Group", true)]
        [MenuItem("GameObject/Selection/Deparent", true)]
        [MenuItem("GameObject/Selection/Set Parent", true)]
        static bool HasSelectedGameObjects(MenuCommand command)
        {
            return Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable).Length > 0;
        }

        [MenuItem("Assets/Move", true)]
        static bool HasSelectedAssets(MenuCommand command)
        {
            return Selection.assetGUIDs.Length > 0;
        }

        [MenuItem("GameObject/Selection/Copy Transforms")]
        static void CopyTransforms(MenuCommand command)
        {
            if (command.context == null || command.context == Selection.activeObject)
            {

                List<Transform> transforms = new List<Transform>(Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable));
                if (transforms.Count > 0)
                {

                    transforms.Sort((a, b) => a.GetInstanceID().CompareTo(b.GetInstanceID()));

                    Matrix4x4[] matrices = new Matrix4x4[transforms.Count];
                    for (int i = 0; i < transforms.Count; i++)
                    {
                        matrices[i] = transforms[i].GetTRSMatrix();
                    }

                    EditorUtils.TrySerializeToClipboard(matrices);
                }
            }
        }

        [MenuItem("GameObject/Selection/Paste Transforms")]
        static void PasteTransforms(MenuCommand command)
        {
            if (command.context == null || command.context == Selection.activeObject)
            {
                List<Transform> transforms = new List<Transform>(Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable));
                if (transforms.Count > 0)
                {
                    transforms.Sort((a, b) => a.GetInstanceID().CompareTo(b.GetInstanceID()));

                    if (EditorUtils.TryDeserializeFromClipboard(out Matrix4x4[] matrices))
                    {
                        if (matrices.Length == transforms.Count)
                        {
                            Undo.RecordObjects(transforms.ToArray(), "Paste Transforms");

                            for (int i = 0; i < transforms.Count; i++)
                            {
                                transforms[i].SetTRS(matrices[i]);
                            }
                        }
                    }
                }
            }
        }

        [MenuItem("GameObject/Selection/Group %G")]
        static void Group(MenuCommand command)
        {
            if (command.context == null || command.context == Selection.activeObject)
            {

                Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable);
                if (transforms.Length > 0)
                {
                    GameObject group = new GameObject(transforms.Length > 1 ? "Group" : transforms[0].name);

                    Undo.RegisterCreatedObjectUndo(group, "Group");
                    Transform commonParent = transforms[0].parent;
                    Vector3 totalPosition = Vector3.zero;

                    for (int i = 1; i < transforms.Length; i++)
                    {
                        if (commonParent != transforms[i].parent)
                        {
                            commonParent = null;
                            break;
                        }
                    }

                    if (commonParent != null)
                    {
                        group.transform.parent = commonParent;
                    }

                    for (int i = 0; i < transforms.Length; i++)
                    {
                        totalPosition += transforms[i].position;
                    }

                    group.transform.position = totalPosition / transforms.Length;

                    for (int i = 0; i < transforms.Length; i++)
                    {
                        Undo.SetTransformParent(transforms[i], group.transform, "Group");
                    }


                    Selection.activeGameObject = group;
                }
            }
        }

        [MenuItem("GameObject/Selection/Deparent")]
        static void Deparent(MenuCommand command)
        {
            if (command.context == null || command.context == Selection.activeObject)
            {
                List<Transform> transforms = new List<Transform>(Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable));
                for (int i = 0; i < transforms.Count; i++)
                {
                    Undo.SetTransformParent(transforms[i], null, "Deparent");
                }
            }
        }

        [MenuItem("GameObject/Selection/Set Parent")]
        static void SetParent(MenuCommand command)
        {
            if (command.context == null || command.context == Selection.activeObject)
            {
                UtilityDialog.ShowObjectFieldDialog("Set Parent", "Parent", "Accept", "Cancel", typeof(Transform), null, true, value =>
                {
                    if (value == null)
                    {
                        List<Transform> transforms = new List<Transform>(Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable));
                        for (int i = 0; i < transforms.Count; i++)
                        {
                            Undo.SetTransformParent(transforms[i], null, "Deparent");
                        }
                    }
                    else if (value as Transform != null)
                    {
                        List<Transform> transforms = new List<Transform>(Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable));
                        for (int i = 0; i < transforms.Count; i++)
                        {
                            Undo.SetTransformParent(transforms[i], value as Transform, "Set Parent");
                        }
                    }
                });
            }

        }

        [MenuItem("CONTEXT/TextureImporter/Invert Green Channel")]
        public static void InvertGreenChannel(MenuCommand command)
        {
            TextureImporter importer = command.context as TextureImporter;
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);

            if (texture != null)
            {

                Texture2D tempTexture = new Texture2D(texture.width, texture.height);
                tempTexture.LoadImage(File.ReadAllBytes(importer.assetPath));

                Color[] colours = tempTexture.GetPixels();

                for (int i = 0; i < colours.Length; ++i)
                {
                    colours[i] = new Color(colours[i].r, 1f - colours[i].g, colours[i].b, colours[i].a);
                }

                tempTexture.SetPixels(colours);
                tempTexture.Apply();

                string extension = importer.assetPath.Substring(importer.assetPath.LastIndexOf(".") + 1).ToLower().Trim();

                if (extension == "png")
                {
                    File.WriteAllBytes(importer.assetPath, tempTexture.EncodeToPNG());
                }
                else if (extension == "jpg" || extension == "jpeg")
                {
                    File.WriteAllBytes(importer.assetPath, tempTexture.EncodeToJPG());
                }
                else if (extension == "exr")
                {
                    File.WriteAllBytes(importer.assetPath, tempTexture.EncodeToEXR());
                }
                else if (extension == "tga")
                {
                    File.WriteAllBytes(importer.assetPath, tempTexture.EncodeToTGA());
                }

                AssetDatabase.ImportAsset(importer.assetPath);

                tempTexture.SmartDestroy();
            }

        }

        [MenuItem("Assets/Move", false, 19)]
        public static void MoveAssets()
        {
            string folderPath = EditorUtility.OpenFolderPanel("Move to folder", "Assets", "");
            if (!string.IsNullOrEmpty(folderPath))
            {
                folderPath = FileUtils.GetLocalPath(folderPath);
                bool failed = false;

                for (int i = 0; i < Selection.assetGUIDs.Length; i++)
                {
                    string currentPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]);
                    string path = folderPath + "/" + FileUtils.GetFileName(currentPath, true);

                    string error = AssetDatabase.ValidateMoveAsset(currentPath, path);
                    if (!string.IsNullOrEmpty(error))
                    {
                        failed = true;
                        Debug.LogError("Cannot move asset: " + error + "\n" + currentPath + "\n" + path);
                        break;
                    }
                }

                if (!failed)
                {
                    for (int i = 0; i < Selection.assetGUIDs.Length; i++)
                    {
                        string currentPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]);
                        string path = folderPath + "/" + FileUtils.GetFileName(currentPath, true);

                        AssetDatabase.MoveAsset(currentPath, path);

                        if (i == 0)
                        {
                            EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(path));
                        }
                    }
                }
            }

        }


        [MenuItem("Assets/Find by GUID")]
        public static void FindByGUID()
        {
            UtilityDialog.ShowFieldDialog("Find by GUID", "Search", "Cancel", "GUID", string.Empty, guid =>
               {
                   string path = AssetDatabase.GUIDToAssetPath(guid.Trim());

                   if (!string.IsNullOrEmpty(path))
                   {
                       Object asset = AssetDatabase.LoadMainAssetAtPath(path);
                       if (asset != null)
                       {
                           Selection.activeObject = asset;
                       }
                   }
               });


        }

        [MenuItem("File/Open Player Log Folder")]
        public static void OpenPlayerLogFolder()
        {
            EditorUtility.RevealInFinder(Path.Combine(Environment.GetEnvironmentVariable("AppData"), "..", "LocalLow", Application.companyName, Application.productName, "Player.log"));
        }

        [MenuItem("File/Open Editor Log Folder")]
        public static void OpenEditorLogFolder()
        {
            EditorUtility.RevealInFinder(Path.Combine(Environment.GetEnvironmentVariable("AppData"), "..", "Local/Unity/Editor/Editor.log"));
        }


        [MenuItem("File/Open Project Folder")]
        public static void OpenProjectFolder()
        {
            EditorUtility.RevealInFinder(Application.dataPath);
        }

        [MenuItem("CONTEXT/MonoScript/Create Custom Inspector")]
        private static void CreateCustomInspector(MenuCommand command)
        {
            string scriptName = command.context.name;
            string path = EditorUtility.SaveFilePanelInProject("New Editor Script", scriptName + "Editor", "cs", "");
            if (path.Length < 0) return;
            string editorScriptName = path.Substring(path.LastIndexOf('/') + 1, path.LastIndexOf('.') - path.LastIndexOf('/') - 1);

            StringBuilder code = new StringBuilder();
            code.Append("using System.Collections;\n");
            code.Append("using System.Collections.Generic;\n");
            code.Append("using UnityEditor;\n");
            code.Append("using UnityEngine;\n");
            code.Append("using Framework;\n\n");

            code.Append("[CanEditMultipleObjects]\n");
            code.Append("[CustomEditor(typeof(" + scriptName + "), true)]\n");
            code.Append("public class " + editorScriptName + " : Editor\n");
            code.Append("{\n");
            code.Append("\tpublic override void OnInspectorGUI()\n");
            code.Append("\t{\n");
            code.Append("\t\tbase.OnInspectorGUI();\n\n");

            code.Append("\t\tif (GUILayout.Button(\"Do Thing\"))\n");
            code.Append("\t\t{\n");
            code.Append("\t\t\t//(target as " + scriptName + ").DoThing();\n");
            code.Append("\t\t}\n");
            code.Append("\t}\n");
            code.Append("}");

            FileUtils.CreateTextFile(path, code.ToString());
        }

        [MenuItem("CONTEXT/MonoScript/Create Custom Property Drawer")]
        private static void CreateCustomPropertyDrawer(MenuCommand command)
        {
            string scriptName = command.context.name;
            string path = EditorUtility.SaveFilePanelInProject("New Property Drawer Script", scriptName + "Drawer", "cs", "");
            if (path.Length < 0) return;
            string editorScriptName = path.Substring(path.LastIndexOf('/') + 1, path.LastIndexOf('.') - path.LastIndexOf('/') - 1);

            StringBuilder code = new StringBuilder();
            code.Append("using System.Collections;\n");
            code.Append("using System.Collections.Generic;\n");
            code.Append("using UnityEditor;\n");
            code.Append("using UnityEngine;\n");
            code.Append("using Framework;\n\n");

            code.Append("[CanEditMultipleObjects]\n");
            code.Append("[CustomPropertyDrawer(typeof(" + scriptName + "), true)]\n");
            code.Append("public class " + editorScriptName + " : PropertyDrawer\n");
            code.Append("{\n");
            code.Append("\tpublic override void OnGUI(Rect position, SerializedProperty property, GUIContent label)\n");
            code.Append("\t{\n");
            code.Append("\t\tbase.OnGUI(position, property, label);\n");
            code.Append("\t}\n\n");
            code.Append("\tpublic override float GetPropertyHeight(SerializedProperty property, GUIContent label)\n");
            code.Append("\t{\n");
            code.Append("\t\treturn base.GetPropertyHeight(property, label);\n");
            code.Append("\t}\n");
            code.Append("}");

            FileUtils.CreateTextFile(path, code.ToString());
        }



        [MenuItem("Assets/Create/Post-processing Effect")]
        private static void CreatePostProcessingEffect()
        {

            string scriptPath = EditorUtility.SaveFilePanelInProject("New Post-processing Effect", "NewEffect", "cs", "");
            if (scriptPath.Length < 0) return;

            string scriptName = scriptPath.Substring(scriptPath.LastIndexOf('/') + 1, scriptPath.LastIndexOf('.') - scriptPath.LastIndexOf('/') - 1);

            string shaderPath = EditorUtility.SaveFilePanelInProject("New Post-processing Shader", scriptName, "shader", "");
            if (shaderPath.Length < 0) return;


            StringBuilder code = new StringBuilder();

            code.Append("using System;\n");
            code.Append("using System.Collections;\n");
            code.Append("using System.Collections.Generic;\n");
            code.Append("using UnityEngine;\n");
            code.Append("using UnityEngine.Rendering.PostProcessing;\n");
            code.Append("using Framework;\n\n");

            code.Append("[Serializable]\n");
            code.Append("[UnityEngine.Rendering.PostProcessing.PostProcess(typeof(" + scriptName + "Renderer), PostProcessEvent.AfterStack, \"Custom/" + scriptName + "\")]\n");
            code.Append("public sealed class " + scriptName + " : PostProcessEffectSettings\n");
            code.Append("{\n");
            code.Append("\tpublic ColorParameter Colour = new ColorParameter();\n");
            code.Append("}\n\n");

            code.Append("public sealed class " + scriptName + "Renderer : PostProcessEffectRenderer<" + scriptName + ">\n");
            code.Append("{\n");
            code.Append("\tpublic override void Render(PostProcessRenderContext context)\n");
            code.Append("\t{\n");
            code.Append("\t\tPropertySheet sheet = context.propertySheets.Get(Shader.Find(\"Hidden/Custom/" + scriptName + "\"));\n");
            code.Append("\t\tsheet.properties.SetColor(\"_Colour\", settings.Colour);\n");
            code.Append("\t\tcontext.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);\n");
            code.Append("\t}\n");
            code.Append("}");

            FileUtils.CreateTextFile(scriptPath, code.ToString());

            code.Clear();

            code.Append("Shader \"Hidden/Custom/" + scriptName + "\"\n");
            code.Append("{\n");
            code.Append("\tHLSLINCLUDE\n\n");

            code.Append("\t\t#include \"Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl\"\n\n");

            code.Append("\t\tTEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);\n");
            code.Append("\t\tfloat4 _Colour;\n\n");

            code.Append("\t\t float4 Frag(VaryingsDefault i) : SV_Target\n");
            code.Append("\t\t{\n");
            code.Append("\t\t\tfloat4 colour = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);\n");
            code.Append("\t\t\tcolour.rgb = lerp(colour.rgb, _Colour.rgb, _Colour.a);\n");
            code.Append("\t\t\treturn colour;\n");
            code.Append("\t\t}\n\n");

            code.Append("\tENDHLSL\n\n");

            code.Append("\tSubShader\n");
            code.Append("\t{\n");
            code.Append("\t\tCull Off ZWrite Off ZTest Always\n\n");

            code.Append("\t\tPass\n");
            code.Append("\t\t{\n");
            code.Append("\t\t\tHLSLPROGRAM\n\n");

            code.Append("\t\t\t\t#pragma vertex VertDefault\n");
            code.Append("\t\t\t\t#pragma fragment Frag\n\n");

            code.Append("\t\t\tENDHLSL\n");
            code.Append("\t\t}\n");
            code.Append("\t}\n");
            code.Append("}");

            FileUtils.CreateTextFile(shaderPath, code.ToString());
        }


        static bool IsSelectionScriptableObject()
        {
            MonoScript script = Selection.activeObject as MonoScript;

            if (script != null)
            {
                return typeof(ScriptableObject).IsAssignableFrom(script.GetClass());
            }

            return false;
        }

        [MenuItem("Assets/Create/Scriptable Object Instance")]
        static void CreateScriptableObjectInstance()
        {
            MonoScript script = Selection.activeObject as MonoScript;

            if (script != null)
            {
                if (typeof(ScriptableObject).IsAssignableFrom(script.GetClass()))
                {
                    ScriptableObject obj = ScriptableObject.CreateInstance(script.name);
                    if (obj == null)
                    {
                        EditorUtility.DisplayDialog("Scriptable Object Creator", "Unable to instantiate class: " + script.name, "Ok");
                        return;
                    }

                    Debug.Log("Created scriptable object instance: " + script.name);

                    AssetDatabase.CreateAsset(obj, "Assets/" + script.name + ".asset");
                    AssetDatabase.SaveAssets();

                    Selection.activeObject = obj;
                }
                else
                {
                    EditorUtility.DisplayDialog("Scriptable Object Creator", "Selected object is not a ScriptableObject class.", "Ok");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Scriptable Object Creator", "Selected object is not a MonoScript", "Ok");

            }
        }

        [MenuItem("Assets/Duplicate Script")]
        private static void DuplicateScript(MenuCommand command)
        {
            void Duplicate(string name)
            {
                MonoScript script = Selection.activeObject as MonoScript;

                if (script.name != name)
                {

                    string text = script.text.Replace("\r\n", "\n");
                    text = text.Replace(script.name, name);

                    string path = AssetDatabase.GetAssetPath(script);
                    path = path.Substring(0, path.LastIndexOf(script.name)) + name + ".cs";

                    FileUtils.CreateTextFile(path, text);
                }
            }

            UtilityDialog.ShowFieldDialog("Duplicate Script", "Ok", "Cancel", "Script name:", Selection.activeObject.name, Duplicate, null);
        }

        [MenuItem("Assets/Duplicate Script", true)]
        private static bool IsMonoScript()
        {
            return Selection.activeObject is MonoScript;
        }



        [MenuItem("CONTEXT/MonoBehaviour/Validate")]
        private static void ValidateMonoBehaviour(MenuCommand command)
        {
            MethodInfo methodInfo = command.context.GetType().GetMethod("OnValidate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo != null)
            {
                methodInfo.Invoke(command.context, new object[0]);
            }

            UnityEditor.EditorUtility.SetDirty(command.context);
        }

        [MenuItem("CONTEXT/Transform/Randomize Y Rotation")]
        private static void RandomizeYRotation(MenuCommand command)
        {
            Transform transform = (command.context as Transform);
            transform.localRotation = Quaternion.Euler(transform.eulerAngles.WithY(Random.Range(0, 360)));
            UnityEditor.EditorUtility.SetDirty(transform);
        }

        [MenuItem("CONTEXT/Camera/Move To Scene View Camera Position")]
        private static void MoveToSceneViewCameraPosition(MenuCommand command)
        {
            Transform transform = (command.context as Camera).transform;
            SceneView sceneView = SceneView.lastActiveSceneView;

            if (sceneView != null && sceneView.camera != null)
            {
                transform.position = sceneView.camera.transform.position;
                transform.rotation = sceneView.camera.transform.rotation;
                UnityEditor.EditorUtility.SetDirty(transform);
            }
        }


        [MenuItem("CONTEXT/SkinnedMeshRenderer/Update Bones To New Root")]
        private static void UpdateBonesToNewRoot(MenuCommand command)
        {
            SkinnedMeshRenderer renderer = command.context as SkinnedMeshRenderer;
            GameObject meshAssetObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(renderer.sharedMesh)) as GameObject;

            if (meshAssetObject != null)
            {
                if (renderer.rootBone == null)
                {
                    int depth = SceneUtils.GetChildDepth(meshAssetObject.transform, meshAssetObject.transform.FindInChildren(renderer.name));

                    Transform parent = renderer.transform;
                    for (int i = 0; i < depth; i++)
                    {
                        parent = parent.parent;
                    }

                    SkinnedMeshRenderer assetRenderer = meshAssetObject.transform.FindInChildren(renderer.name).GetComponent<SkinnedMeshRenderer>();
                    renderer.rootBone = parent.FindInChildren(assetRenderer.rootBone.name);
                }

                SkinnedMeshRenderer[] childRenderers = meshAssetObject.GetComponentsInChildren<SkinnedMeshRenderer>();

                for (int i = 0; i < childRenderers.Length; i++)
                {
                    if (childRenderers[i].sharedMesh == renderer.sharedMesh)
                    {
                        Transform[] bones = new Transform[childRenderers[i].bones.Length];
                        for (int j = 0; j < bones.Length; j++)
                        {
                            bones[j] = renderer.rootBone.FindInChildren(childRenderers[i].bones[j].name);
                        }

                        renderer.bones = bones;
                        EditorUtility.SetDirty(renderer);
                        break;
                    }
                }
            }
        }


        [MenuItem("CONTEXT/ModelImporter/Import Animation Settings From Another Model")]
        private static void ImportAnimationSettings(MenuCommand command)
        {
            ModelImporter model = command.context as ModelImporter;
            UtilityDialog.ShowObjectFieldDialog("Import Animation Settings", "Model", "Import", "Cancel", typeof(GameObject), null, false, new UtilityDialogField[]
            {
                new UtilityDialogField<bool>("Keep Clip Names", true)
            }, (asset, values) =>
            {

                ModelImporter importModel = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(asset)) as ModelImporter;

                ModelImporterClipAnimation[] clips = importModel.clipAnimations;

                if ((bool)values["Keep Clip Names"])
                {
                    for (int i = 0; i < clips.Length; i++)
                    {
                        if (model.clipAnimations.Length > i)
                        {
                            clips[i].name = model.clipAnimations[i].name;
                        }
                    }
                }

                model.clipAnimations = clips;

                model.importConstraints = importModel.importConstraints;
                model.importAnimation = importModel.importAnimation;

                if (importModel.isBakeIKSupported)
                {
                    model.bakeIK = importModel.bakeIK;
                }

                model.resampleCurves = importModel.resampleCurves;
                model.animationCompression = importModel.animationCompression;
                model.importAnimatedCustomProperties = importModel.importAnimatedCustomProperties;




                model.SaveAndReimport();
            });
        }

        [MenuItem("Assets/Delete Empty Folders")]
        public static void DeleteEmptyFolders()
        {
            bool deletedSomething = true;
            while (deletedSomething)
            {
                deletedSomething = false;
                string[] paths = Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories);
                for (int i = 0; i < paths.Length; i++)
                {
                    string[] directories = Directory.GetDirectories(paths[i]);

                    if (directories.Length == 0)
                    {
                        string[] files = Directory.GetFiles(paths[i]);
                        bool containsFiles = false;

                        for (int j = 0; j < files.Length; j++)
                        {
                            if (!files[j].ToLower().Trim().EndsWith(".meta"))
                            {
                                containsFiles = true;
                                break;
                            }
                        }

                        if (!containsFiles)
                        {
                            string localPath = FileUtils.GetLocalPath(paths[i]);
                            if (AssetDatabase.DeleteAsset(localPath))
                            {
                                Debug.Log("Deleted empty folder: " + localPath);
                                deletedSomething = true;
                            }
                            // Directory.Delete(paths[i], true);
                        }
                    }
                }

                if (deletedSomething)
                {
                    AssetDatabase.Refresh();
                }
            }
        }


        [MenuItem("Window/Collapse Hierarchy %#C")]
        public static void CollapseHierarchy()
        {
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            EditorWindow hierarchyWindow = EditorWindow.focusedWindow;
            MethodInfo expandMethodInfo = hierarchyWindow.GetType().GetMethod("SetExpandedRecursive");
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                expandMethodInfo.Invoke(hierarchyWindow, new object[] { root.GetInstanceID(), false });
            }
        }

        [MenuItem("Tools/Update Generated Constants")]
        private static void UpdateGeneratedConstants()
        {

            string[] sortingLayerNames = (string[])(typeof(UnityEditorInternal.InternalEditorUtility)).GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, new object[0]); ;
            string[] layerNames = UnityEditorInternal.InternalEditorUtility.layers;
            List<string> sceneNames = new List<string>();
            List<string> santitizedSceneNames = new List<string>();
            int[] sortingLayerValues = (int[])(typeof(UnityEditorInternal.InternalEditorUtility)).GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, new object[0]);
            int[] layerValues = new int[layerNames.Length];
            LayerMask[] layerMaskValues = new LayerMask[layerNames.Length];
            string[] collisionMatrixNames = new string[layerNames.Length];
            LayerMask[] collisionMatrixValues = new LayerMask[layerNames.Length];
            string[] tagNames = UnityEditorInternal.InternalEditorUtility.tags;
            string[] tagValues = new string[tagNames.Length];

            for (int i = 0; i < layerNames.Length; i++)
            {
                layerNames[i] = StringUtils.Santise(layerNames[i], false, false);
                layerValues[i] = LayerMask.NameToLayer(layerNames[i]);
                layerMaskValues[i] = 1 << layerValues[i];
                collisionMatrixNames[i] = layerNames[i] + "CollisionMask";
                collisionMatrixValues[i] = PhysicsUtils.GetCollisionMatrixMask(i);
            }

            for (int i = 0; i < sortingLayerNames.Length; i++)
            {
                sortingLayerNames[i] = StringUtils.Santise(sortingLayerNames[i], false, false);
            }

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                string path = EditorBuildSettings.scenes[i].path;

                if (!string.IsNullOrEmpty(path))
                {
                    int slashIndex = path.LastIndexOf('/') + 1;
                    string sceneName = path.Substring(slashIndex, path.Length - slashIndex - 6);
                    string sanitizedName = StringUtils.Santise(StringUtils.Titelize(sceneName), false, false);

                    if (!sceneNames.Contains(sceneName) && File.Exists(path))
                    {
                        sceneNames.Add(sceneName);
                        santitizedSceneNames.Add(sanitizedName);
                    }
                }
            }


            for (int i = 0; i < tagNames.Length; i++)
            {
                tagValues[i] = tagNames[i];
                tagNames[i] = StringUtils.Santise(tagNames[i], false, false);
            }


            List<CodeGenerator.CodeDefintion> definitions = new List<CodeGenerator.CodeDefintion>();

            definitions.Add(CodeGenerator.CreateEnumDefinition("LayerName", layerNames, layerValues));
            definitions.Add(CodeGenerator.CreateEnumDefinition("SortingLayerName", sortingLayerNames, sortingLayerValues));

            definitions.Add(CodeGenerator.CreateClass("Layer", CodeGenerator.CreateConstantInts(layerNames, layerValues), true, false));
            definitions.Add(CodeGenerator.CreateClass("SortingLayer", CodeGenerator.CreateConstantInts(sortingLayerNames, sortingLayerValues), true, false));
            definitions.Add(CodeGenerator.CreateClass("Tag", CodeGenerator.CreateConstantStrings(tagNames, tagValues), true, false));
            definitions.Add(CodeGenerator.CreateClass("LayerMasks", CodeGenerator.CreateConstantLayerMasks(layerNames, layerMaskValues, true), true, true));
            definitions.Add(CodeGenerator.CreateClass("CollisionMatrix", CodeGenerator.CreateConstantLayerMasks(collisionMatrixNames, collisionMatrixValues, true), true, false));
            definitions.Add(CodeGenerator.CreateClass("SceneNames", CodeGenerator.CreateConstantStrings(santitizedSceneNames, sceneNames), true, false));

            Type[] scriptableEnumTypes = typeof(ScriptableEnum).GetAllSubtypesInUnityAssemblies();


            for (int i = 0; i < scriptableEnumTypes.Length; i++)
            {
                CodeGenerator.CodeDefintion enumClass = CodeGenerator.CreateClass(scriptableEnumTypes[i].Name, CodeGenerator.CreateScriptableEnumConstants(scriptableEnumTypes[i]), false, true);
                string titleName = StringUtils.Titelize(scriptableEnumTypes[i].Name);
                enumClass.AppendAttribute("CreateAssetMenu", "fileName = \"" + titleName + "\"", "menuName = \"Scriptable Enum/" + titleName + "\"");

                definitions.Add(enumClass);
            }

            CodeGenerator.CreateSourceFile("AutoGeneratedConstants", definitions);
        }

    }


}