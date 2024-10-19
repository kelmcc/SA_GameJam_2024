using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class SelectionDetailsWindow : EditorWindow
    {

        private bool _countDisabledObjects;
        private bool _hasCalculated;

        private int _rootObjects;
        private int _gameObjects;
        private int _monoBehaviours;

        private int _renderers;
        private int _rigidBodies;
        private int _colliders;

        private int _lights;
        private int _particleSystems;
        private int _audioSources;

        private int _vertices;
        private int _triangles;
        private HashSet<Mesh> _meshes;
        private HashSet<Material> _materials;

        private Bounds _bounds;


        [MenuItem("GameObject/Selection/Details", false, 0)]
        static void ShowWindow()
        {
            SelectionDetailsWindow window = GetWindow<SelectionDetailsWindow>(true, "Selection Details", true);

            window.ShowUtility();
            window.CenterInScreen(250, 280);
        }

        private void OnSelectionChange()
        {
            Calculate();
            Repaint();
        }

        private void OnFocus()
        {
            Calculate();
            Repaint();
        }

        void OnGUI()
        {

            EditorGUILayout.BeginVertical();

            EditorGUI.BeginChangeCheck();
            _countDisabledObjects = EditorGUILayout.Toggle("Count Disabled Objects", _countDisabledObjects);
            if (EditorGUI.EndChangeCheck() || !_hasCalculated)
            {
                Calculate();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Root Objects", _rootObjects.ToString());
            EditorGUILayout.LabelField("Game Objects", _gameObjects.ToString());
            EditorGUILayout.LabelField("MonoBehaviours", _monoBehaviours.ToString());

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Renderers", _renderers.ToString());
            EditorGUILayout.LabelField("Rigidbodies", _rigidBodies.ToString());
            EditorGUILayout.LabelField("Colliders", _colliders.ToString());

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Lights", _lights.ToString());
            EditorGUILayout.LabelField("Particle Systems", _particleSystems.ToString());
            EditorGUILayout.LabelField("Audio Sources", _audioSources.ToString());

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Meshes", _meshes.Count.ToString());
            EditorGUILayout.LabelField("Materials", _materials.Count.ToString());
            EditorGUILayout.LabelField("Vertices", _vertices.ToString());
            EditorGUILayout.LabelField("Triangles", _triangles.ToString());

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Bounds", _bounds.size.x.ToString("F2") + " x " + _bounds.size.y.ToString("F2") + " x " + _bounds.size.z.ToString("F2"));

            EditorGUILayout.EndVertical();

        }

        void Calculate()
        {
            _hasCalculated = true;

            _rootObjects = Selection.GetFiltered<Transform>(SelectionMode.TopLevel).Length;
            _gameObjects = 0;
            _monoBehaviours = 0;

            _renderers = 0;
            _rigidBodies = 0;
            _colliders = 0;

            _lights = 0;
            _particleSystems = 0;
            _audioSources = 0;

            _triangles = 0;
            _vertices = 0;
            _meshes = new HashSet<Mesh>();
            _materials = new HashSet<Material>();

            _bounds = new Bounds();

            Transform[] transforms = Selection.GetFiltered<Transform>(SelectionMode.Deep);

            for (int i = 0; i < transforms.Length; i++)
            {
                GameObject go = transforms[i].gameObject;

                T[] GetEnabled<T>() where T : Component
                {
                    T[] components = go.GetComponents<T>();


                    if (_countDisabledObjects)
                    {
                        return components;
                    }

                    List<T> result = new List<T>();
                    for (int j = 0; j < components.Length; j++)
                    {
                        if (components[j] is Behaviour behaviour)
                        {
                            if (behaviour.enabled)
                            {
                                result.Add(components[j]);
                            }
                        }
                        else
                        {
                            result.Add(components[j]);
                        }
                    }

                    return result.ToArray();
                }

                int CountEnabled<T>() where T : Component
                {
                    return GetEnabled<T>().Length;
                }


                if (go.activeInHierarchy || _countDisabledObjects)
                {
                    _gameObjects++;
                    _monoBehaviours += CountEnabled<MonoBehaviour>();
                    _rigidBodies += CountEnabled<Rigidbody>() + CountEnabled<Rigidbody2D>();
                    _colliders += CountEnabled<Collider>() + CountEnabled<Collider2D>();

                    MeshFilter[] filters = GetEnabled<MeshFilter>();
                    for (int j = 0; j < filters.Length; j++)
                    {
                        if (filters[j].sharedMesh != null)
                        {
                            _meshes.Add(filters[j].sharedMesh);
                            _vertices += filters[j].sharedMesh.vertexCount;
                            _triangles += filters[j].sharedMesh.triangles.Length / 3;
                        }
                    }


                    Renderer[] renderers = GetEnabled<Renderer>();
                    for (int j = 0; j < renderers.Length; j++)
                    {
                        if (renderers[j].enabled || _countDisabledObjects)
                        {
                            _bounds.Encapsulate(renderers[j].bounds);

                            _renderers++;

                            for (int k = 0; k < renderers[j].sharedMaterials.Length; k++)
                            {
                                if (renderers[j].sharedMaterials[k] != null)
                                {
                                    _materials.Add(renderers[j].sharedMaterials[k]);
                                }
                            }
                        }
                    }

                    _lights += CountEnabled<Light>();
                    _particleSystems += CountEnabled<ParticleSystem>();
                    _audioSources += CountEnabled<AudioSource>();

                }
            }
        }


    }
}