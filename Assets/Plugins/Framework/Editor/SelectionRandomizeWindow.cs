using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework
{
    public class SelectionRandomizeWindow : EditorWindow
    {
        enum Tab
        {
            Position,
            Rotation,
            Scale
        }

        enum Space
        {
            Local,
            World
        }

        enum Shape
        {
            Circle,
            Sphere,
            Box,
            Offset
        }

        enum Plane
        {
            XY,
            XZ,
            ZY
        }

        [SerializeField] private Tab _tab;
        [SerializeField] private Space _space;
        [SerializeField] private Shape _shape;
        [SerializeField] private Plane _plane;

        [SerializeField] private Vector3 _boxDimensions = new Vector3(10, 10, 10);
        [SerializeField] private float _radius = 5;

        [SerializeField] private bool _uniformScale = true;
        [SerializeField] private FloatRange _scale = new FloatRange(1, 2);
        [SerializeField] private FloatRange _xScale = new FloatRange(1, 2);
        [SerializeField] private FloatRange _yScale = new FloatRange(1, 2);
        [SerializeField] private FloatRange _zScale = new FloatRange(1, 2);

        [SerializeField] private FloatRange _xRotation = new FloatRange(0, 0);
        [SerializeField] private FloatRange _yRotation = new FloatRange(0, 360);
        [SerializeField] private FloatRange _zRotation = new FloatRange(0, 0);

        private SerializedObject _serializedObject;


        [MenuItem("GameObject/Selection/Randomize", false, 0)]
        static void ShowWindow()
        {
            SelectionRandomizeWindow window = GetWindow<SelectionRandomizeWindow>(true, "Randomize Selection", true);

            window.CenterInScreen(300, 140);
        }

        void OnGUI()
        {
            if (_serializedObject == null)
            {
                _serializedObject = new SerializedObject(this);
            }

            _tab = (Tab)GUILayout.Toolbar((int)_tab, Enum.GetNames(typeof(Tab)));

            EditorGUILayout.Space();

            switch (_tab)
            {
                case Tab.Position: PositionTab(); break;
                case Tab.Rotation: RotationTab(); break;
                case Tab.Scale: ScaleTab(); break;
            }

            if (_serializedObject != null)
            {
                _serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }



        void PositionTab()
        {
            _shape = (Shape)EditorGUILayout.EnumPopup("Shape", _shape);


            if (_shape == Shape.Box)
            {
                _boxDimensions.x = EditorGUILayout.FloatField("X Size", _boxDimensions.x);
                _boxDimensions.y = EditorGUILayout.FloatField("Y Size", _boxDimensions.y);
                _boxDimensions.z = EditorGUILayout.FloatField("Z Size", _boxDimensions.z);

                ApplyButton(transform =>
                {
                    float x = Random.Range(-_boxDimensions.x * 0.5f, _boxDimensions.x * 0.5f);
                    float y = Random.Range(-_boxDimensions.y * 0.5f, _boxDimensions.y * 0.5f);
                    float z = Random.Range(-_boxDimensions.z * 0.5f, _boxDimensions.z * 0.5f);

                    transform.localPosition = new Vector3(x, y, z);
                });
            }

            if (_shape == Shape.Circle)
            {
                _plane = (Plane)EditorGUILayout.EnumPopup("Plane", _plane);
                _radius = EditorGUILayout.FloatField("Radius", _radius);


                ApplyButton(transform =>
                {
                    if (_plane == Plane.XY) transform.localPosition = RandomUtils.InsideCircle(Vector3.zero, _radius, Vector3.forward);
                    if (_plane == Plane.XZ) transform.localPosition = RandomUtils.InsideCircle(Vector3.zero, _radius, Vector3.up);
                    if (_plane == Plane.ZY) transform.localPosition = RandomUtils.InsideCircle(Vector3.zero, _radius, Vector3.right);
                });
            }


            if (_shape == Shape.Sphere)
            {
                _radius = EditorGUILayout.FloatField("Radius", _radius);

                ApplyButton(transform =>
                {
                    transform.localPosition = Random.insideUnitSphere * _radius;
                });
            }

            if (_shape == Shape.Offset)
            {
                _plane = (Plane)EditorGUILayout.EnumPopup("Plane", _plane);
                _radius = EditorGUILayout.FloatField("Radius", _radius);

                ApplyButton(transform =>
                {
                    if (_plane == Plane.XY) transform.localPosition = transform.localPosition + RandomUtils.InsideCircle(Vector3.zero, _radius, Vector3.forward);
                    if (_plane == Plane.XZ) transform.localPosition = transform.localPosition + RandomUtils.InsideCircle(Vector3.zero, _radius, Vector3.up);
                    if (_plane == Plane.ZY) transform.localPosition = transform.localPosition + RandomUtils.InsideCircle(Vector3.zero, _radius, Vector3.right);
                });
            }

        }

        void RotationTab()
        {

            _space = (Space)EditorGUILayout.EnumPopup("Space", _space);

            EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_xRotation)), new GUIContent("X Rotation"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_yRotation)), new GUIContent("Y Rotation"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_zRotation)), new GUIContent("Z Rotation"));


            ApplyButton(transform =>
            {

                if (_space == Space.Local)
                {
                    transform.rotation = Quaternion.Euler(_xRotation.ChooseRandom(), _yRotation.ChooseRandom(), _zRotation.ChooseRandom());
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(_xRotation.ChooseRandom(), _yRotation.ChooseRandom(), _zRotation.ChooseRandom());
                }

            });
        }


        void ScaleTab()
        {

            _uniformScale = EditorGUILayout.Toggle("Uniform Scale", _uniformScale);

            if (_uniformScale)
            {
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_scale)), new GUIContent("Scale"));
            }
            else
            {
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_xScale)), new GUIContent("X Scale"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_yScale)), new GUIContent("Y Scale"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_zScale)), new GUIContent("Z Scale"));
            }


            ApplyButton(transform =>
            {

                if (_uniformScale)
                {
                    float scale = _scale.ChooseRandom();
                    transform.localScale = new Vector3(scale, scale, scale);
                }
                else
                {
                    transform.localScale = new Vector3(_xScale.ChooseRandom(), _yScale.ChooseRandom(), _zScale.ChooseRandom());
                }

            });

        }

        void ApplyButton(Action<Transform> applyFucntion)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(Selection.transforms.Length < 1);

            if (GUILayout.Button("Randomize"))
            {
                for (int i = 0; i < Selection.transforms.Length; i++)
                {
                    applyFucntion(Selection.transforms[i]);
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
        }




    }
}
