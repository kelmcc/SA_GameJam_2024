using System;
using System.Reflection;
using Framework;
using UnityEditor;
using UnityEngine;

namespace SoundManager
{
    /// <summary>
    /// Custom inspector for sound banks.
    /// </summary>
    [CustomEditor(typeof(SoundBank), true)]
    [CanEditMultipleObjects]
    public class SoundBankEditor : UnityEditor.Editor
    {

        [SerializeField]
        private bool _expandFilters;

        private bool _pickedObjectReady;
        private SoundInstance _lastInstance;
        private SoundBank _bank;
        private float _blendParameter;

        private static EditorSoundPool _soundPool;
        private readonly string[] FILTER_FIELD_NAMES = { "_lowPassFilter", "_highPassFilter", "_distortionFilter", "_chorusFilter", "_echoFilter", "_reverbFilter", "_startFadeFilter", "_endFadeFilter", "_volumeCurveFilter", "_pitchCurveFilter" };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _soundPool = null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _bank = target as SoundBank;

            serializedObject.ApplyModifiedProperties();


            _expandFilters = EditorGUILayout.Foldout(_expandFilters, new GUIContent("Filters", "Optional filters to apply to this sound, and all of its child sounds."), true);

            if (_expandFilters)
            {
                DrawFilterSection();
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Separator();

            DrawButtonSection();
            DrawStatusSection();
        }


        void DrawButtonSection()
        {
            if (targets.Length > 1) return;

            SerializedProperty clipsProperty = serializedObject.FindProperty("_audioClips");

            if (Event.current.commandName == "ObjectSelectorClosed" && _pickedObjectReady)
            {
                _pickedObjectReady = false;
                if (EditorGUIUtility.GetObjectPickerObject() != null)
                {
                    if (clipsProperty.arraySize == 0 || clipsProperty.GetArrayElementAtIndex(clipsProperty.arraySize - 1).objectReferenceValue != null)
                    {
                        clipsProperty.arraySize++;
                    }

                    clipsProperty.GetArrayElementAtIndex(clipsProperty.arraySize - 1).objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                    serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.BeginHorizontal();

            if (clipsProperty != null)
            {
                if (GUILayout.Button("Add Clip", GUILayout.MaxWidth(80)))
                {
                    UnityEngine.Object defaultClip = null;

                    if (clipsProperty.arraySize > 0)
                    {
                        defaultClip = clipsProperty.GetArrayElementAtIndex(clipsProperty.arraySize - 1).objectReferenceValue;
                    }

                    _pickedObjectReady = true;
                    EditorGUIUtility.ShowObjectPicker<AudioClip>(defaultClip, false, "", -1);
                }

                if (GUILayout.Button("Clear Clips", GUILayout.MaxWidth(80)))
                {

                    clipsProperty.arraySize = 0;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (_bank is BlendSoundBank)
            {
                EditorGUIUtility.labelWidth = 45f;
                _blendParameter = EditorGUILayout.Slider(new GUIContent("Blend"), _blendParameter, 0, 1);
            }
            else if (_bank is SequenceSoundBank)
            {
                SequenceSoundInstance sequence = null;
                if (_lastInstance != null)
                {
                    sequence = _lastInstance as SequenceSoundInstance;
                }

                if (sequence != null && sequence.HasPreviousSection)
                {
                    if (GUILayout.Button("<<", GUILayout.MaxWidth(50f)))
                    {
                        sequence.PlayPreviousSection();
                    }
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.Button("<<", GUILayout.MaxWidth(50f));
                    EditorGUI.EndDisabledGroup();
                }

                if (sequence != null && sequence.HasNextSection)
                {
                    if (GUILayout.Button(">>", GUILayout.MaxWidth(50f)))
                    {
                        sequence.PlayNextSection();
                    }
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.Button(">>", GUILayout.MaxWidth(50f));
                    EditorGUI.EndDisabledGroup();
                }

                GUILayout.FlexibleSpace();
            }
            else
            {
                GUILayout.FlexibleSpace();
            }

            if (GUILayout.Button("PLAY", GUILayout.MaxWidth(80f)))
            {
                if (_soundPool == null)
                {
                    _soundPool = new EditorSoundPool();
                    Selection.selectionChanged += _soundPool.Clear;
                }
                else
                {
                    _soundPool.Clear();
                }

                _lastInstance = _bank.TestInEditor(_soundPool);
            }

            if (GUILayout.Button("STOP", GUILayout.MaxWidth(80f)))
            {

                if (_lastInstance != null)
                {
                    _lastInstance.StopAndDestroy();
                }
            }

            BlendSoundInstance blend = _lastInstance as BlendSoundInstance;
            if (blend != null)
            {
                blend.SetBlendParameter(_blendParameter);
            }

            GUILayout.EndHorizontal();
        }

        void DrawStatusSection()
        {

            if (_lastInstance != null)
            {
                EditorGUILayout.Space();
                GUILayout.Label(_lastInstance.GetGUIStatusString());
            }
        }

        void DrawFilterSection()
        {

            int filtersEnabled = 0;
            int filtersDisabled = 0;

            SerializedProperty property = serializedObject.GetIterator();
            SerializedProperty[] filterFields = GetFilterFields(out filtersEnabled, out filtersDisabled);

            string filterPropertyPath = "";
            bool isExpanded = false;
            bool insideBlock = false;

            GUILayout.BeginVertical("GroupBox");


            // HEADER
            GUILayout.BeginHorizontal();

            if (filtersEnabled == 0)
            {
                GUILayout.Label("NO FILTERS");
            }

            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(filtersDisabled < 1);

            // ADD BUTTON
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus More")))
            {
                GenericMenu menu = new GenericMenu();

                for (int i = 0; i < filterFields.Length; i++)
                {
                    if (!filterFields[i].FindPropertyRelative("_isEnabled").boolValue)
                    {
                        string name = filterFields[i].displayName;
                        menu.AddItem(new GUIContent(name.Substring(0, name.LastIndexOf(' '))), false, EnableFilter, filterFields[i]);
                    }
                }

                menu.ShowAsContext();
            }

            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            while (property.Next(true))
            {

                // PARENT FILTER PROPERTY
                Type type = property.name.ToLower().EndsWith("filter") ? property.GetValueType() : null;
                if (type != null && typeof(SoundBankFilter).IsAssignableFrom(type) && property.FindPropertyRelative("_isEnabled").boolValue)
                {

                    if (insideBlock)
                    {
                        GUILayout.EndVertical();
                    }

                    filterPropertyPath = property.propertyPath;
                    GUILayout.BeginVertical("GroupBox");

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);

                    property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName, true);
                    isExpanded = property.isExpanded;

                    // CONTEXT MENU
                    if (Event.current.type == EventType.ContextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {

                        GenericMenu menu = new GenericMenu();
                        string name = property.name;
                        menu.AddItem(new GUIContent("Reset"), false, ResetFilter, name);
                        menu.ShowAsContext();
                    }

                    GUILayout.FlexibleSpace();

                    // REMOVE BUTTON
                    GUIStyle minusButtonStyle = GUI.skin.GetStyle("OL Minus");
                    minusButtonStyle.margin = new RectOffset(0, 0, 3, 0);
                    if (GUILayout.Button("", minusButtonStyle))
                    {
                        property.FindPropertyRelative("_isEnabled").boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                    }

                    GUILayout.EndHorizontal();

                    if (isExpanded)
                    {
                        GUILayout.Space(10);
                    }

                    insideBlock = true;
                }

                // FILTER FIELD
                if (isExpanded && IsFilterField(filterPropertyPath, property))
                {
                    if (property.name != "_isEnabled")
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(property, true);
                        GUILayout.EndHorizontal();
                    }

                    if (filterPropertyPath == "_reverbFilter" && property.name == "reverbPreset")
                    {
                        EditorGUI.BeginDisabledGroup(property.enumValueIndex != (int)AudioReverbPreset.User);
                    }
                    if (filterPropertyPath == "_reverbFilter" && property.name == "density")
                    {
                        EditorGUI.EndDisabledGroup();
                    }
                }
            }

            if (insideBlock)
            {
                GUILayout.EndVertical();
            }


            GUILayout.EndVertical();
        }

        void ResetFilter(object data)
        {
            FieldInfo field = serializedObject.targetObject.GetType().GetField((string)data, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            SoundBankFilter filter = field.GetValue(serializedObject.targetObject) as SoundBankFilter;

            filter.Reset();
        }

        void EnableFilter(object data)
        {
            ((SerializedProperty)data).FindPropertyRelative("_isEnabled").boolValue = true;
            ((SerializedProperty)data).isExpanded = true;
            serializedObject.ApplyModifiedProperties();
        }

        bool IsFilterField(string filterPropertyPath, SerializedProperty property)
        {
            int dotIndex = property.propertyPath.LastIndexOf('.');

            if (dotIndex > -1)
            {
                if (property.propertyPath.Substring(0, dotIndex) == filterPropertyPath)
                {
                    return true;
                }
            }

            return false;
        }

        SerializedProperty[] GetFilterFields(out int enabled, out int disabled)
        {
            enabled = 0;
            disabled = 0;

            SerializedProperty[] filterFields = new SerializedProperty[FILTER_FIELD_NAMES.Length];

            for (int i = 0; i < filterFields.Length; i++)
            {
                filterFields[i] = serializedObject.FindProperty(FILTER_FIELD_NAMES[i]);
                if (filterFields[i].FindPropertyRelative("_isEnabled").boolValue)
                {
                    enabled++;
                }
                else
                {
                    disabled++;
                }
            }

            return filterFields;
        }



    }
}



