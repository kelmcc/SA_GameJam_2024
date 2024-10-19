using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EditorGUI = UnityEditor.EditorGUI;
using Object = UnityEngine.Object;

namespace Framework
{
    public class SelectionTransformWindow : EditorWindow
    {
        enum Tab
        {
            Translate,
            Rotate,
            Scale
        }

        enum Space
        {
            Local,
            World
        }



        [SerializeField] private Space _space;
        [SerializeField] private Tab _tab;
        [SerializeField] private Vector3 _positionOffset;
        [SerializeField] private Vector3 _rotationOffset;
        [SerializeField] private Vector3 _scaleOffset;
        [SerializeField] private Vector3 _positionMulitplier = Vector3.one;
        [SerializeField] private Vector3 _rotationMulitplier = Vector3.one;
        [SerializeField] private Vector3 _scaleMultiplier = Vector3.one;


        [MenuItem("GameObject/Selection/Transform", false, 0)]
        static void ShowWindow()
        {
            SelectionTransformWindow window = GetWindow<SelectionTransformWindow>(true, "Transform Selection", true);

            window.ShowUtility();
            window.CenterInScreen(330, 220);

        }



        void OnGUI()
        {


            _tab = (Tab)GUILayout.Toolbar((int)_tab, Enum.GetNames(typeof(Tab)));
            EditorGUILayout.Space();
            EditorGUIUtility.wideMode = true;

            switch (_tab)
            {
                case Tab.Translate: TranslateTab(); break;
                case Tab.Rotate: RotateTab(); break;
                case Tab.Scale: ScaleTab(); break;
            }
        }

        void TranslateTab()
        {
            _space = (Space)EditorGUILayout.EnumPopup("Space", _space);
            _positionOffset = EditorGUILayout.Vector3Field("Offset", _positionOffset);
            _positionMulitplier = EditorGUILayout.Vector3Field("Multiplier", _positionMulitplier);

            TransformButton(transform =>
            {
                if (_space == Space.Local)
                {
                    transform.position = transform.position + (transform.right * _positionOffset.x) + (transform.up * _positionOffset.y) + (transform.forward * _positionOffset.z);
                    transform.localPosition = transform.localPosition.MultiplyComponentWise(_positionMulitplier);
                }
                else
                {
                    transform.position += _positionOffset;
                    transform.position = transform.position.MultiplyComponentWise(_positionMulitplier);
                }
            });
        }

        void RotateTab()
        {
            _space = (Space)EditorGUILayout.EnumPopup("Space", _space);
            _rotationOffset = EditorGUILayout.Vector3Field("Offset", _rotationOffset);
            _rotationMulitplier = EditorGUILayout.Vector3Field("Multiplier", _rotationMulitplier);

            TransformButton(transform =>
            {
                if (_space == Space.Local)
                {
                    transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + _rotationOffset);
                    transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.MultiplyComponentWise(_rotationMulitplier));
                }
                else
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + _rotationOffset);
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.MultiplyComponentWise(_rotationMulitplier));
                }
            });
        }

        void ScaleTab()
        {
            _scaleOffset = EditorGUILayout.Vector3Field("Offset", _scaleOffset);
            _scaleMultiplier = EditorGUILayout.Vector3Field("Multiplier", _scaleMultiplier);

            TransformButton(transform =>
            {
                transform.localScale += _scaleOffset;
                transform.localScale = transform.localScale.MultiplyComponentWise(_scaleMultiplier);
            });
        }

        void TransformButton(Action<Transform> transformFunction)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(Selection.transforms.Length < 1);

            if (GUILayout.Button("Transform"))
            {
                Transform[] transforms = Selection.transforms;
                Undo.RecordObjects(transforms, "Transform Selection");

                for (int i = 0; i < transforms.Length; i++)
                {
                    transformFunction(transforms[i]);
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

    }
}
