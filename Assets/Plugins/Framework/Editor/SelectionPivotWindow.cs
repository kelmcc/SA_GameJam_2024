using System;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class SelectionPivotWindow : EditorWindow
    {
        enum Mode
        {
            RelativeToBounds,
            OffsetFromCenter,
            Absolute
        }

        [SerializeField]
        private Vector3 _pivotPosition = new Vector3(0f, 0f, 0f);

        [SerializeField]
        private Mode _mode = Mode.RelativeToBounds;


        [MenuItem("GameObject/Selection/Pivot", false, 0)]
        static void ShowWindow()
        {
            SelectionPivotWindow window = GetWindow<SelectionPivotWindow>(true, "Selection Pivot", true);

            window.ShowUtility();
            window.CenterInScreen(360, 100);
        }

        void OnGUI()
        {

            _mode = (Mode)EditorGUILayout.EnumPopup("Mode", _mode);
            _pivotPosition = EditorGUILayout.Vector3Field("Position", _pivotPosition);


            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(Selection.transforms.Length < 1);

            if (GUILayout.Button("Set Pivot"))
            {

                Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel);

                for (int i = 0; i < transforms.Length; i++)
                {


                    Vector3 oldPosition = transforms[i].position;

                    Undo.RegisterFullObjectHierarchyUndo(transforms[i], "Set Pivot");

                    switch (_mode)
                    {
                        case Mode.RelativeToBounds:

                            Bounds bounds = transforms[i].gameObject.GetBounds();
                            Vector3 scale = bounds.extents;
                            scale.Scale(_pivotPosition);
                            transforms[i].position = bounds.center + scale;

                            break;
                        case Mode.OffsetFromCenter:

                            transforms[i].position = transforms[i].gameObject.GetBounds().center + _pivotPosition;

                            break;
                        case Mode.Absolute:

                            transforms[i].position = _pivotPosition;

                            break;

                        default: throw new ArgumentOutOfRangeException();
                    }

                    Vector3 offset = transforms[i].position - oldPosition;

                    for (int j = 0; j < transforms[i].childCount; j++)
                    {
                        transforms[i].GetChild(j).position -= offset;
                    }

                }

            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

        }




    }
}

