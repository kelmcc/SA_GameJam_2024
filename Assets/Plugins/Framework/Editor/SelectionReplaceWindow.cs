using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class SelectionReplaceWindow : EditorWindow
    {

        [SerializeField]
        private GameObject _replacementPrefab;

        [SerializeField]
        private bool _keepName = false;

        [SerializeField]
        private bool _keepRotation = true;

        [SerializeField]
        private bool _keepScale = true;


        [MenuItem("GameObject/Selection/Replace", false, 0)]
        static void ShowWindow()
        {
            SelectionReplaceWindow window = GetWindow<SelectionReplaceWindow>(true, "Replace Selection", true);

            window.ShowUtility();
            window.CenterInScreen(360, 110);
        }

        void OnGUI()
        {
            _replacementPrefab = (GameObject)EditorGUILayout.ObjectField("Replacement Prefab", _replacementPrefab, typeof(GameObject), false);
            _keepName = EditorGUILayout.Toggle("Keep Name", _keepName);
            _keepRotation = EditorGUILayout.Toggle("Keep Rotation", _keepRotation);
            _keepRotation = EditorGUILayout.Toggle("Keep Scale", _keepScale);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(Selection.transforms.Length < 1);

            if (GUILayout.Button("Replace"))
            {
                GameObject[] objects = Selection.gameObjects;
                GameObject[] newObjects = new GameObject[objects.Length];

                for (int i = 0; i < objects.Length; i++)
                {
                    GameObject obj = EditorUtils.InstantiatePrefab(_replacementPrefab, objects[i].transform.position, _keepRotation ? objects[i].transform.rotation : Quaternion.identity);
                    obj.transform.parent = objects[i].transform.parent;

                    if (_keepName)
                    {
                        obj.name = objects[i].name;
                    }

                    if (_keepScale)
                    {
                        obj.transform.localScale = objects[i].transform.localScale;
                    }

                    newObjects[i] = obj;

                    DestroyImmediate(objects[i].gameObject);
                }

                Selection.objects = newObjects;
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

        }




    }
}

/*
 *
 *     public void Replace()
    {

        if (_namePrefix.IsNullOrEmpty() || _prefab == null) return;

        List<Transform> children = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.name.StartsWith(_namePrefix))
            {
                children.Add(transform.GetChild(i));
            }
        }

#if UNITY_EDITOR
        for (int i = 0; i < children.Count; i++)
        {
            GameObject newChild = EditorUtils.InstantiatePrefab(_prefab, children[i].transform.position, children[i].transform.rotation);
            newChild.transform.parent = transform;
            newChild.transform.localScale = children[i].transform.localScale;

            DestroyImmediate(children[i].gameObject);
        }
#endif
    }
 */
