using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EditorGUI = UnityEditor.EditorGUI;
using Object = UnityEngine.Object;

namespace Framework
{
    public class SelectionLayoutWindow : EditorWindow
    {
        enum Tab
        {
            Line,
            Grid,
            Radial
        }

        enum Axis
        {
            X,
            Y,
            Z
        }

        enum Plane
        {
            XY,
            XZ,
            ZY
        }

        enum Space
        {
            Local,
            World
        }

        enum SpacingMode
        {
            FixedInterval,
            BoundsAndSpacing
        }

        enum ElementRotation
        {
            DontRotate,
            AlignByX,
            AlignByY,
            AlignByZ
        }


        [SerializeField] private Transform _pivot;
        [SerializeField] private Space _space;
        [SerializeField] private Axis _axis;
        [SerializeField] private Plane _plane;
        [SerializeField] private Tab _tab;
        [SerializeField] private SpacingMode _spacingMode;
        [SerializeField] private float _lineInterval = 1f;
        [SerializeField] private float _lineSpacing = 0f;
        [SerializeField] private int _rows = 1;
        [SerializeField] private Vector2 _gridSpacing = new Vector2(1f, 1f);
        [SerializeField] private bool _inclusiveAngle = false;
        [SerializeField] private float _totalAngle = 360f;
        [SerializeField] private float _radius = 1f;
        [SerializeField] private ElementRotation _elementRotation = ElementRotation.AlignByY;
        [SerializeField] private Vector3 _rotationOffset = Vector3.zero;
        [SerializeField] private bool _applyContinuously;

        [MenuItem("GameObject/Selection/Layout", false, 0)]
        static void ShowWindow()
        {
            SelectionLayoutWindow window = GetWindow<SelectionLayoutWindow>(true, "Layout Selection", true);

            window.ShowUtility();
            window.CenterInScreen(330, 220);

        }

        void OnEnable()
        {
            if (_pivot == null && Selection.transforms.Length > 0)
            {
                Transform parent = Selection.transforms[0].parent;

                for (int i = 1; i < Selection.transforms.Length; i++)
                {
                    if (Selection.transforms[i].parent != parent)
                    {
                        parent = null;
                    }
                }

                _pivot = parent;
            }
        }


        private void OnSelectionChange()
        {
            _applyContinuously = false;
        }

        void OnGUI()
        {

            EditorGUI.BeginChangeCheck();
            _tab = (Tab)GUILayout.Toolbar((int)_tab, Enum.GetNames(typeof(Tab)));
            if (EditorGUI.EndChangeCheck())
            {
                _applyContinuously = false;
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            switch (_tab)
            {
                case Tab.Line: LineTab(); break;
                case Tab.Grid: GridTab(); break;
                case Tab.Radial: RadialTab(); break;
            }
        }

        void RadialTab()
        {
            _pivot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Pivot"), _pivot, typeof(Transform), true);
            _plane = (Plane)EditorGUILayout.EnumPopup("Plane", _plane);
            _space = (Space)EditorGUILayout.EnumPopup("Space", _space);

            _totalAngle = EditorGUILayout.FloatField("Total Angle", _totalAngle);
            _inclusiveAngle = EditorGUILayout.Toggle("Inclusive Angle", _inclusiveAngle);

            _radius = EditorGUILayout.FloatField("Radius", _radius);

            _elementRotation = (ElementRotation)EditorGUILayout.EnumPopup("Rotate Elements", _elementRotation);

            EditorGUI.BeginDisabledGroup(_elementRotation == ElementRotation.DontRotate);
            EditorGUIUtility.wideMode = true;
            _rotationOffset = EditorGUILayout.Vector3Field("Rotation Offset", _rotationOffset);
            EditorGUIUtility.wideMode = false;
            EditorGUI.EndDisabledGroup();

            LayoutButton(RadialLayout);
        }


        void LineTab()
        {
            _pivot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Pivot"), _pivot, typeof(Transform), true);
            _axis = (Axis)EditorGUILayout.EnumPopup("Axis", _axis);
            _space = (Space)EditorGUILayout.EnumPopup("Rotation Space", _space);
            _spacingMode = (SpacingMode)EditorGUILayout.EnumPopup("Spacing Mode", _spacingMode);

            if (_spacingMode == SpacingMode.FixedInterval)
            {
                _lineInterval = EditorGUILayout.FloatField("Interval", _lineInterval);
            }
            if (_spacingMode == SpacingMode.BoundsAndSpacing)
            {
                _lineSpacing = EditorGUILayout.FloatField("Spacing", _lineSpacing);
            }

            LayoutButton(LineLayout);
        }

        void GridTab()
        {
            _pivot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Pivot"), _pivot, typeof(Transform), true);
            _plane = (Plane)EditorGUILayout.EnumPopup("Plane", _plane);
            _space = (Space)EditorGUILayout.EnumPopup("Rotation Space", _space);
            _rows = Mathf.Max(EditorGUILayout.IntField("Rows", _rows), 1);
            _gridSpacing.x = EditorGUILayout.FloatField("Horizontal Spacing", _gridSpacing.x);
            _gridSpacing.y = EditorGUILayout.FloatField("Vertical Spacing", _gridSpacing.y);

            LayoutButton(GridLayout);
        }

        void LayoutButton(Action<Transform[]> layoutFucntion)
        {
            if (EditorGUI.EndChangeCheck() && _applyContinuously)
            {
                layoutFucntion(GetOrderedSelection());
            }

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();


            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(Selection.transforms.Length < 1 || _applyContinuously);
            if (GUILayout.Button("Layout"))
            {
                layoutFucntion(GetOrderedSelection());
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();
            _applyContinuously = EditorGUILayout.ToggleLeft("AUTO", _applyContinuously, GUILayout.Width(55));
            if (EditorGUI.EndChangeCheck() && _applyContinuously)
            {
                layoutFucntion(GetOrderedSelection());
            }

            EditorGUILayout.EndHorizontal();



            EditorGUILayout.EndVertical();
        }

        void GridLayout(Transform[] transforms)
        {
            Undo.RecordObjects(transforms, "Layout Selection");

            Vector3 pivotPosition = _pivot == null ? Vector3.zero : _pivot.position;
            Quaternion pivotRotation = _pivot == null ? Quaternion.identity : _pivot.rotation;

            Vector3 horizontalDirection = Vector3.zero;
            Vector3 verticalDirection = Vector3.zero;

            switch (_plane)
            {
                case Plane.XY:
                    horizontalDirection = Vector3.right;
                    verticalDirection = Vector3.up;
                    break;

                case Plane.XZ:
                    horizontalDirection = Vector3.right;
                    verticalDirection = Vector3.forward;
                    break;

                case Plane.ZY:
                    horizontalDirection = Vector3.forward;
                    verticalDirection = Vector3.up;
                    break;
            }

            int columns = Mathf.CeilToInt((float)transforms.Length / _rows);
            int index = 0;

            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (index < transforms.Length)
                    {
                        Vector3 offset = (horizontalDirection * x * _gridSpacing.x) + (verticalDirection * y * _gridSpacing.y);
                        if (_space == Space.Local)
                        {
                            transforms[index].position = pivotPosition + (pivotRotation * offset);
                            transforms[index].rotation = _pivot.rotation;
                        }
                        else
                        {
                            transforms[index].position = pivotPosition + offset;
                        }

                        transforms[index].rotation = pivotRotation;

                        index++;
                    }
                }

            }
        }

        void LineLayout(Transform[] transforms)
        {
            Undo.RecordObjects(transforms, "Layout Selection");

            Vector3 pivotPosition = _pivot == null ? Vector3.zero : _pivot.position;
            Quaternion pivotRotation = _pivot == null ? Quaternion.identity : _pivot.rotation;

            Vector3 direction = _axis == Axis.X ? Vector3.right : _axis == Axis.Y ? Vector3.up : Vector3.forward;
            float distance = 0;

            for (int i = 1; i < transforms.Length; i++)
            {
                if (_spacingMode == SpacingMode.FixedInterval)
                {
                    distance += _lineInterval;
                }
                else
                {
                    switch (_axis)
                    {
                        case Axis.X:
                            distance += transforms[i].gameObject.GetBounds().size.x + _lineSpacing;
                            break;
                        case Axis.Y:
                            distance += transforms[i].gameObject.GetBounds().size.y + _lineSpacing;
                            break;
                        case Axis.Z:
                            distance += transforms[i].gameObject.GetBounds().size.z + _lineSpacing;
                            break;
                    }
                }

                if (_space == Space.Local)
                {
                    transforms[i].position = pivotPosition + (pivotRotation * direction * distance);
                }
                else
                {
                    transforms[i].position = pivotPosition + (direction * distance);
                }

                transforms[i].rotation = pivotRotation;
            }
        }

        void RadialLayout(Transform[] transforms)
        {
            Undo.RecordObjects(transforms, "Layout Selection");

            Vector3 pivotPosition = _pivot == null ? Vector3.zero : _pivot.position;
            Quaternion pivotRotation = _pivot == null ? Quaternion.identity : _pivot.rotation;

            float interval = _totalAngle / (_inclusiveAngle ? transforms.Length - 1 : transforms.Length);
            float angle = 0;


            Vector3 normal = _plane == Plane.XY ? Vector3.forward : _plane == Plane.XZ ? Vector3.up : Vector3.right;
            Vector3 center = pivotPosition;

            if (_space == Space.Local && _pivot != null)
            {
                normal = transforms[0].parent.InverseTransformDirection(normal);
            }

            for (int i = 0; i < transforms.Length; i++)
            {
                Vector3 position = MathUtils.GetPointOnCircle(center, _radius, normal, angle);
                transforms[i].position = position;


                if (_elementRotation == ElementRotation.AlignByX)
                {
                    transforms[i].rotation = Quaternion.LookRotation(normal) * Quaternion.LookRotation(Vector3.left) * Quaternion.Euler(-angle, 0, 0) * Quaternion.Euler(_rotationOffset);
                }
                else if (_elementRotation == ElementRotation.AlignByY)
                {
                    transforms[i].rotation = Quaternion.LookRotation(normal) * Quaternion.LookRotation(Vector3.down) * Quaternion.Euler(0, -angle, 0) * Quaternion.Euler(_rotationOffset);
                }
                else if (_elementRotation == ElementRotation.AlignByZ)
                {
                    transforms[i].rotation = Quaternion.LookRotation(normal) * Quaternion.LookRotation(Vector3.forward) * Quaternion.Euler(0, 0, -angle) * Quaternion.Euler(_rotationOffset);
                }

                angle += interval;
            }

        }

        Transform[] GetOrderedSelection()
        {
            List<Transform> transforms = new List<Transform>(Selection.transforms);
            transforms.Sort((x, y) => x.GetSiblingIndex().CompareTo(y.GetSiblingIndex()));
            return transforms.ToArray();
        }


    }
}
