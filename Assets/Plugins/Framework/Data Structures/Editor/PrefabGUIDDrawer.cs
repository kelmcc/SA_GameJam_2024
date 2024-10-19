using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Framework;

namespace Framework
{


    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(PrefabGUID), true)]
    public class PrefabGUIDDrawer : PropertyDrawer
    {



        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            SerializedProperty guidProperty = property.FindPropertyRelative("GUID");

            string guid = guidProperty.stringValue;
            Object prefab = null;

            if (!string.IsNullOrEmpty(guid))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(path))
                {
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
            }


            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            prefab = EditorGUI.ObjectField(position, label, prefab, typeof(GameObject), false);

            if (EditorGUI.EndChangeCheck())
            {
                if (prefab == null)
                {
                    guidProperty.stringValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefab, out guid, out long localID))
                    {
                        guidProperty.stringValue = guid;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            EditorGUI.EndProperty();

        }

    }

}