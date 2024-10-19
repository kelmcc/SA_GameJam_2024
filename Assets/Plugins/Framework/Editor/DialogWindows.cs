using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public abstract class UtilityDialogData : ScriptableObject
    {

    }

    public class UtilityDialogField
    {
        public string Label;
        public Type Type;
        public object DefaultValue;

        public UtilityDialogField(string label, Type type, object defaultValue)
        {
            Label = label;
            Type = type;
            DefaultValue = defaultValue;
        }

        public virtual object Draw(object currentValue, float width)
        {
            return EditorUtils.PropertyField(Type, new GUIContent(Label), currentValue);
        }

        public class ObjectField : UtilityDialogField
        {
            public bool AllowSceneObjects;

            public ObjectField(string label, Type type, object defaultValue, bool allowSceneObjects) : base(label, type, defaultValue)
            {
                AllowSceneObjects = allowSceneObjects;
            }

            public override object Draw(object currentValue, float width)
            {
                return EditorGUILayout.ObjectField(new GUIContent(Label), (Object)currentValue, Type, AllowSceneObjects, GUILayout.Width(width));
            }
        }

        public class LayerField : UtilityDialogField
        {

            public LayerField(string label) : base(label, typeof(int), 0)
            {

            }

            public override object Draw(object currentValue, float width)
            {
                return EditorGUILayout.LayerField(new GUIContent(Label), (int)currentValue);
            }
        }

    }

    public class UtilityDialogField<T> : UtilityDialogField
    {
        public UtilityDialogField(string label, T defaultValue) : base(label, typeof(T), defaultValue)
        {
        }
    }

    public class UtilityDialog : EditorWindow
    {
        private Action<UtilityDialog> _onGUI;

        const float MARGIN = 10f;

        public static UtilityDialog ShowDialog<T>(string titleText, float width, float height) where T : UtilityDialog
        {
            T dialog = CreateInstance<T>();

            dialog.titleContent = new GUIContent(titleText);
            dialog.ShowUtility();
            dialog.CenterInScreen(width, height);

            return dialog;
        }

        public static UtilityDialog ShowDialog(string titleText, float width, float height, Action onGUI)
        {
            UtilityDialog dialog = CreateInstance<UtilityDialog>();

            dialog._onGUI = (d) => onGUI();
            dialog.titleContent = new GUIContent(titleText);
            dialog.ShowUtility();
            dialog.CenterInScreen(width, height);

            return dialog;
        }

        public static UtilityDialog ShowDialog(string titleText, float width, float height, Action<UtilityDialog> onGUI)
        {
            UtilityDialog dialog = CreateInstance<UtilityDialog>();

            dialog._onGUI = (d) => onGUI(d);
            dialog.titleContent = new GUIContent(titleText);
            dialog.ShowUtility();
            dialog.CenterInScreen(width, height);

            return dialog;
        }



        public static UtilityDialog ShowDataDialog<T>(string titleText, float width, float height, Action<T> onSubmit) where T : UtilityDialogData
        {
            UtilityDataDialog dialog = CreateInstance<UtilityDataDialog>();

            dialog.Data = CreateInstance<T>();
            dialog.OnSubmit = (data) => onSubmit(data as T);
            dialog.titleContent = new GUIContent(titleText);
            dialog.ShowUtility();
            dialog.CenterInScreen(width, height);

            return dialog;
        }

        public static UtilityDialog ShowDataDialog<T>(string titleText, float width, float height, T data, Action<T> onSubmit) where T : UtilityDialogData
        {
            UtilityDataDialog dialog = CreateInstance<UtilityDataDialog>();

            dialog.Data = data;
            dialog.OnSubmit = (d) => onSubmit(d as T);
            dialog.titleContent = new GUIContent(titleText);
            dialog.ShowUtility();
            dialog.CenterInScreen(width, height);

            return dialog;
        }

        public static UtilityDialog ShowFieldDialog(string titleText, string submitText, string cancelText, UtilityDialogField[] fields, Action<Dictionary<string, object>> onSubmit, Action onCancel = null)
        {
            UtilityFieldDialog dialog = CreateInstance<UtilityFieldDialog>();

            dialog.SubmitText = submitText;
            dialog.CancelText = cancelText;
            dialog.OnSubmit = onSubmit;
            dialog.OnCancel = onCancel;

            dialog.Fields = fields;
            dialog.Values = new object[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                dialog.Values[i] = fields[i].DefaultValue;
            }

            dialog.titleContent = new GUIContent(titleText);
            dialog.ShowUtility();

            float height = (MARGIN * 2) + (fields.Length + 2) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            dialog.CenterInScreen(325, Mathf.Min(height, 1024));

            return dialog;
        }

        public static UtilityDialog ShowFieldDialog<T>(string titleText, string submitText, string cancelText, string labelText, T defaultValue, Action<T> onSubmit, Action onCancel = null)
        {
            return ShowFieldDialog(titleText, submitText, cancelText, new[] { new UtilityDialogField(labelText, typeof(T), defaultValue) }, (values) =>
            {
                onSubmit.Invoke((T)values[labelText]);
            }, onCancel);
        }

        public static void ShowConfirmationDialog(string titleText, string contentText, float width, float height, Action onSubmit, Action onCancel = null, string confirmText = "Continue", float margin = 10)
        {
            ShowDialog(titleText, width, height, (d) =>
            {
                GUILayout.Space(margin);


                GUILayout.Label(contentText, FontStyles.Centered);

                GUILayout.Space(EditorGUIUtility.singleLineHeight);

                GUILayout.BeginHorizontal();
                GUILayout.Space(margin);

                if (GUILayout.Button(confirmText))
                {
                    onSubmit?.Invoke();
                    d.Close();
                }
                if (GUILayout.Button("Cancel"))
                {
                    onCancel?.Invoke();
                    d.Close();
                }
                GUILayout.Space(margin);
                GUILayout.EndHorizontal();


                GUILayout.Space(margin);
            });
        }

        public static void ShowConfirmationDialog(string titleText, string contentText, Action onSubmit, Action onCancel = null, string confirmText = "Continue")
        {
            Vector2 size = EditorUtils.CalculateRectSize(contentText, 10f, 44f);

            ShowDialog(titleText, Mathf.Max(size.x, 300f), size.y, (d) =>
             {
                 GUILayout.Space(10f);

                 GUILayout.Label(contentText, FontStyles.Centered);

                 GUILayout.Space(EditorGUIUtility.singleLineHeight);

                 GUILayout.BeginHorizontal();
                 GUILayout.Space(10f);

                 if (GUILayout.Button(confirmText))
                 {
                     onSubmit?.Invoke();
                     d.Close();
                 }
                 if (GUILayout.Button("Cancel"))
                 {
                     onCancel?.Invoke();
                     d.Close();
                 }
                 GUILayout.Space(10f);
                 GUILayout.EndHorizontal();


                 GUILayout.Space(10f);
             });
        }

        public static void ShowMessage(string titleText, string contentText, float width, float height, float margin = 10)
        {
            ShowDialog(titleText, width, height, () =>
            {
                GUILayout.Space(margin);
                GUILayout.BeginHorizontal();
                GUILayout.Space(margin);
                GUILayout.Label(contentText);
                GUILayout.Space(margin);
                GUILayout.EndHorizontal();
                GUILayout.Space(margin);
            });
        }

        public static void ShowMessage(string titleText, string contentText, float width, float height, string submitText, float margin = 10)
        {
            ShowDialog(titleText, width, height, (d) =>
            {
                GUILayout.Space(margin);
                GUILayout.BeginHorizontal();
                GUILayout.Space(margin);
                GUILayout.Label(contentText);
                GUILayout.Space(10f);
                if (GUILayout.Button(submitText))
                {
                    d.Close();
                }
                GUILayout.Space(margin);
                GUILayout.EndHorizontal();
                GUILayout.Space(margin);
            });
        }



        public static UtilityDialog ShowObjectFieldDialog(string titleText, string assetFieldLabel, string submitText, string cancelText, Type type, Object defaultValue, bool allowSceneObjects, Action<Object> onSubmit, Action onCancel = null)
        {
            return ShowFieldDialog(titleText, submitText, cancelText, new[] { new UtilityDialogField.ObjectField(assetFieldLabel, type, defaultValue, allowSceneObjects), }, (values) =>
            {
                onSubmit.Invoke((Object)values[assetFieldLabel]);
            }, onCancel);
        }

        public static UtilityDialog ShowObjectFieldDialog(string titleText, string assetFieldLabel, string submitText, string cancelText, Type type, Object defaultValue, bool allowSceneObjects, UtilityDialogField[] fields, Action<Object, Dictionary<string, object>> onSubmit, Action onCancel = null)
        {
            List<UtilityDialogField> fieldList = new List<UtilityDialogField>();
            fieldList.Add(new UtilityDialogField.ObjectField(assetFieldLabel, type, defaultValue, allowSceneObjects));
            fieldList.AddRange(fields);

            return ShowFieldDialog(titleText, submitText, cancelText, fieldList.ToArray(), (values) =>
            {
                onSubmit.Invoke((Object)values[assetFieldLabel], values);
            }, onCancel);
        }

        protected virtual void OnGUI()
        {
            _onGUI(this);
        }

        protected class UtilityFieldDialog : UtilityDialog
        {
            public UtilityDialogField[] Fields;
            public object[] Values;
            public string SubmitText;
            public string CancelText;
            public Action<Dictionary<string, object>> OnSubmit;
            public Action OnCancel;

            protected Vector2 _scrollPosition;

            void Submit()
            {
                Close();
                Dictionary<string, object> valueDictionary = new Dictionary<string, object>();
                for (int i = 0; i < Fields.Length; i++)
                {
                    valueDictionary.Add(Fields[i].Label, Values[i]);
                }

                OnSubmit?.Invoke(valueDictionary);
            }

            protected override void OnGUI()
            {


                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
                {
                    Submit();
                }

                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

                GUILayout.Space(MARGIN);


                for (int i = 0; i < Fields.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(MARGIN);

                    Values[i] = Fields[i].Draw(Values[i], position.width - MARGIN * 2);

                    GUILayout.Space(MARGIN);
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(MARGIN);

                if (GUILayout.Button(SubmitText))
                {
                    Submit();
                }

                if (GUILayout.Button(CancelText))
                {
                    Close();
                    OnCancel?.Invoke();
                }

                GUILayout.Space(MARGIN);
                EditorGUILayout.EndHorizontal();

                GUILayout.EndScrollView();

            }
        }

        protected class UtilityDataDialog : UtilityDialog
        {
            public UtilityDialogData Data;
            public Action<UtilityDialogData> OnSubmit;

            void Submit()
            {
                Close();
                OnSubmit?.Invoke(Data);
            }

            protected override void OnGUI()
            {
                GUILayout.Space(MARGIN);

                SerializedObject so = new SerializedObject(Data);
                so.Update();

                EditorGUI.BeginChangeCheck();

                so.ForEachProperty(p =>
                {
                    if (p.name != "m_Script")
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(MARGIN);

                        EditorGUILayout.PropertyField(p);
                        GUILayout.Space(MARGIN);
                        EditorGUILayout.EndHorizontal();
                    }
                });

                if (EditorGUI.EndChangeCheck())
                {
                    so.ApplyModifiedProperties();
                }



                GUILayout.Space(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(MARGIN);

                if (GUILayout.Button("Apply"))
                {
                    Submit();
                }

                if (GUILayout.Button("Cancel"))
                {
                    Close();
                }

                GUILayout.Space(MARGIN);
                EditorGUILayout.EndHorizontal();

            }
        }



    }



}