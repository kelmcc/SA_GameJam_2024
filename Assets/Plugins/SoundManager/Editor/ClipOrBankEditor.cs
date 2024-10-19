using Framework;
using UnityEditor;
using UnityEngine;

namespace SoundManager
{
    [CustomPropertyDrawer(typeof(ClipOrBank))]
    public class ClipOrBankEditor : PropertyDrawer
    {
        public override void OnGUI(Rect totalRect, SerializedProperty property, GUIContent label)
        {
            const float iconWidth = 20f;
            const float iconMargin = 2f;
            float indent = EditorGUI.indentLevel * EditorUtils.INDENT_WIDTH;

            EditorGUI.BeginProperty(totalRect, label, property);

            SerializedProperty isBankProperty = property.FindPropertyRelative("_isBank");
            SerializedProperty bankProperty = property.FindPropertyRelative("_soundBank");
            SerializedProperty clipProperty = property.FindPropertyRelative("_audioClip");

            Rect rect = new Rect(totalRect.x + EditorGUIUtility.labelWidth - indent, totalRect.y, totalRect.width - EditorGUIUtility.labelWidth + indent, totalRect.height);
            Rect fieldRect = new Rect(rect.x, rect.y, rect.width - iconWidth, rect.height);
            Rect iconRect = new Rect(rect.x + rect.width - iconWidth - indent + iconMargin, rect.y + 4f, iconWidth + indent, rect.height);

            EditorGUI.PrefixLabel(totalRect.WithWidth(EditorGUIUtility.labelWidth), new GUIContent(label.text, property.GetTooltip()));


            EditorGUI.PropertyField(fieldRect, isBankProperty.boolValue ? bankProperty : clipProperty, GUIContent.none);

            EditorGUI.BeginChangeCheck();
            bool isBank = EditorGUI.Popup(iconRect, new GUIContent(""), isBankProperty.boolValue ? 0 : 1, new[] { new GUIContent(" Sound Bank"), new GUIContent(" Audio Clip") }, GUI.skin.GetStyle("PaneOptions")) == 0;

            if (EditorGUI.EndChangeCheck())
            {
                isBankProperty.boolValue = isBank;
            }
            EditorGUI.EndProperty();
        }


    }
}
