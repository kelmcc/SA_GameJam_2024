using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Framework;
using UnityEditor;
using UnityEngine;

namespace Framework
{


    // https://gist.github.com/MattRix/c1f7840ae2419d8eb2ec0695448d4321
    // https://unitylist.com/p/5c3/Unity-editor-icons
    // https://github.com/jasursadikov/unity-editor-icons

    public class EditorUIBroswer : EditorWindow
    {
        [SerializeField]
        private int _tab;

        [SerializeField]
        private IconBrowser _iconBrowser;

        [SerializeField]
        private StyleBrowser _styleBrowser;

        [MenuItem("Window/Editor UI Browser")]
        static void Init()
        {

            EditorUIBroswer window = EditorWindow.GetWindow<EditorUIBroswer>();
            window.titleContent = new GUIContent("Editor UI Browser");
            window.Show();
            window.CenterInScreen(500, 500);
        }



        void OnGUI()
        {

            _tab = GUILayout.Toolbar(_tab, new string[] { "Styles", "Icons" });
            if (_iconBrowser == null)
            {
                _iconBrowser = new IconBrowser();
            }

            if (_styleBrowser == null)
            {
                _styleBrowser = new StyleBrowser();
            }

            EditorGUILayout.Space();

            switch (_tab)
            {
                case 0:
                    _styleBrowser.OnGUI();
                    break;
                case 1:
                    _iconBrowser.OnGUI();
                    break;

            }
        }

        [Serializable]
        class StyleBrowser
        {
            [SerializeField]
            Vector2 _scrollPosition = new Vector2(0, 0);

            [SerializeField]
            string _search = "";

            public void OnGUI()
            {
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.Label("GUILayout.Label(\"Hello\",\"ErrorLabel\");");
                GUILayout.FlexibleSpace();
                GUILayout.Label("Search:");
                _search = EditorGUILayout.TextField(_search);

                GUILayout.EndHorizontal();
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);


                foreach (GUIStyle style in GUI.skin.customStyles)
                {

                    if (style.name.ToLower().Contains(_search.ToLower()))
                    {
                        GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                        GUILayout.Space(7);
                        if (GUILayout.Button(style.name, style))
                        {

                            EditorGUIUtility.systemCopyBuffer = style.name;
                        }

                        GUILayout.FlexibleSpace();
                        EditorGUILayout.SelectableLabel("\"" + style.name + "\"");
                        GUILayout.EndHorizontal();
                        GUILayout.Space(11);
                    }
                }

                GUILayout.EndScrollView();
            }
        }

        [Serializable]
        class IconBrowser
        {
            [SerializeField]
            bool viewBigIcons = true;
            [SerializeField]
            bool darkPreview = true;
            [SerializeField]
            Vector2 scroll;
            [SerializeField]
            int buttonSize = 70;
            [SerializeField]
            string search = "";
            [SerializeField]
            string[] iconNames;
            [SerializeField]
            GUIContent iconSelected;
            [SerializeField]
            List<GUIContent> iconContentListAll;
            [SerializeField]
            List<GUIContent> iconContentListSmall;
            [SerializeField]
            List<GUIContent> iconContentListBig;
            [SerializeField]
            List<string> iconMissingNames;
            [SerializeField]
            GUIStyle iconButtonStyle = null;
            [SerializeField]
            GUIStyle iconPreviewBlack = null;
            [SerializeField]
            GUIStyle iconPreviewWhite = null;


            void SearchGUI()
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (isWide) GUILayout.Space(10);

                    search = EditorGUILayout.TextField(search, EditorStyles.toolbarSearchField);
                    if (GUILayout.Button(EditorGUIUtility.IconContent("icons/packagemanager/dark/cancel.png"), //SVN_DeletedLocal
                        EditorStyles.toolbarButton,
                        GUILayout.Width(22))
                    ) search = "";
                }
            }


            bool isWide => Screen.width > 550;
            bool doSearch => !string.IsNullOrWhiteSpace(search) && search != "";

            GUIContent GetIcon(string icon_name)
            {
                GUIContent valid = null;
                Debug.unityLogger.logEnabled = false;
                if (!string.IsNullOrEmpty(icon_name)) valid = EditorGUIUtility.IconContent(icon_name);
                Debug.unityLogger.logEnabled = true;
                return valid?.image == null ? null : valid;
            }

            void SaveIcon(string icon_name)
            {
                Texture2D tex = EditorGUIUtility.IconContent(icon_name).image as Texture2D;

                if (tex != null)
                {
                    string path = EditorUtility.SaveFilePanel(
                        "Save icon", "", icon_name, "png");

                    if (path != null)
                    {
                        try
                        {
                            Texture2D outTex = new Texture2D(
                                tex.width, tex.height,
                                tex.format, tex.mipmapCount, true);

                            Graphics.CopyTexture(tex, outTex);

                            File.WriteAllBytes(path, outTex.EncodeToPNG());
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError("Cannot save the icon : " + e.Message);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Cannot save the icon : null texture error!");
                }
            }


            public void OnGUI()
            {
                float ppp = EditorGUIUtility.pixelsPerPoint;

                if (iconNames == null)
                {
                    InitIcons();
                }

                if (!isWide) SearchGUI();

                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    GUILayout.Label("Select what icons to show", GUILayout.Width(160));
                    viewBigIcons = GUILayout.SelectionGrid(
                        viewBigIcons ? 1 : 0, new string[] { "Small", "Big" },
                        2, EditorStyles.toolbarButton) == 1;

                    if (isWide) SearchGUI();
                }

                if (isWide) GUILayout.Space(3);

                using (GUILayout.ScrollViewScope scope = new GUILayout.ScrollViewScope(scroll))
                {
                    GUILayout.Space(10);

                    scroll = scope.scrollPosition;

                    buttonSize = viewBigIcons ? 70 : 40;

                    // scrollbar_width = ~ 12.5
                    float render_width = (Screen.width / ppp - 13f);
                    int gridW = Mathf.FloorToInt(render_width / buttonSize);
                    float margin_left = (render_width - buttonSize * gridW) / 2;

                    int row = 0, index = 0;

                    List<GUIContent> iconList;

                    if (doSearch)
                        iconList = iconContentListAll.Where(x => x.tooltip.ToLower()
                            .Contains(search.ToLower())).ToList();
                    else iconList = viewBigIcons ? iconContentListBig : iconContentListSmall;

                    while (index < iconList.Count)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Space(margin_left);

                            for (int i = 0; i < gridW; ++i)
                            {
                                int k = i + row * gridW;

                                GUIContent icon = iconList[k];

                                if (GUILayout.Button(icon,
                                    iconButtonStyle,
                                    GUILayout.Width(buttonSize),
                                    GUILayout.Height(buttonSize)))
                                {
                                    EditorGUI.FocusTextInControl("");
                                    iconSelected = icon;
                                }

                                index++;

                                if (index == iconList.Count) break;
                            }
                        }

                        row++;
                    }

                    GUILayout.Space(10);
                }


                if (iconSelected == null) return;
                if (iconSelected.image == null)
                {
                    iconSelected = null;
                    return;
                }


                GUILayout.FlexibleSpace();

                using (new GUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.MaxHeight(viewBigIcons ? 140 : 120)))
                {
                    using (new GUILayout.VerticalScope(GUILayout.Width(130)))
                    {
                        GUILayout.Space(2);

                        GUILayout.Button(iconSelected,
                            darkPreview ? iconPreviewBlack : iconPreviewWhite,
                            GUILayout.Width(128), GUILayout.Height(viewBigIcons ? 128 : 40));

                        GUILayout.Space(5);

                        darkPreview = GUILayout.SelectionGrid(
                            darkPreview ? 1 : 0, new string[] { "Light", "Dark" },
                            2, EditorStyles.miniButton) == 1;

                        GUILayout.FlexibleSpace();
                    }

                    GUILayout.Space(10);

                    using (new GUILayout.VerticalScope())
                    {
                        string s = $"Size: {iconSelected.image.width}x{iconSelected.image.height}";
                        s += "\nIs Pro Skin Icon: " + (iconSelected.tooltip.IndexOf("d_") == 0 ? "Yes" : "No");
                        s += $"\nTotal {iconContentListAll.Count} icons";
                        GUILayout.Space(5);
                        EditorGUILayout.HelpBox(s, MessageType.None);
                        GUILayout.Space(5);
                        EditorGUILayout.TextField("EditorGUIUtility.IconContent(\"" + iconSelected.tooltip + "\")");
                        GUILayout.Space(5);
                        if (GUILayout.Button("Copy to clipboard", EditorStyles.miniButton))
                            EditorGUIUtility.systemCopyBuffer = iconSelected.tooltip;
                        if (GUILayout.Button("Save icon to file ...", EditorStyles.miniButton))
                            SaveIcon(iconSelected.tooltip);
                    }

                    GUILayout.Space(10);

                    if (GUILayout.Button("X", GUILayout.ExpandHeight(true)))
                    {
                        iconSelected = null;
                    }

                }
            }



            void AllTheTEXTURES(ref GUIStyle s, Texture2D t)
            {
                s.hover.background = s.onHover.background =
                    s.focused.background = s.onFocused.background = s.active.background = s.onActive.background = s.normal.background = s.onNormal.background = t;
                s.hover.scaledBackgrounds = s.onHover.scaledBackgrounds = s.focused.scaledBackgrounds = s.onFocused.scaledBackgrounds =
                    s.active.scaledBackgrounds = s.onActive.scaledBackgrounds = s.normal.scaledBackgrounds = s.onNormal.scaledBackgrounds = new Texture2D[] { t };
            }

            Texture2D Texture2DPixel(Color c)
            {
                Texture2D t = new Texture2D(1, 1);
                t.SetPixel(0, 0, c);
                t.Apply();
                return t;
            }


            private string GetIconsPath()
            {
#if UNITY_2018_3_OR_NEWER
                return UnityEditor.Experimental.EditorResources.iconsPath;
#else
        var assembly = typeof(EditorGUIUtility).Assembly;
        var editorResourcesUtility = assembly.GetType("UnityEditorInternal.EditorResourcesUtility");
        var iconsPathProperty = editorResourcesUtility.GetProperty(
            "iconsPath",
            BindingFlags.Static | BindingFlags.Public);
        return (string)iconsPathProperty.GetValue(null, new object[] { });
#endif
            }


            string[] FindIconNames()
            {
                AssetBundle editorAssetBundle = (AssetBundle)typeof(EditorGUIUtility).GetMethod("GetEditorAssetBundle", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { });
                string iconsPath = GetIconsPath();

                List<string> names = new List<string>();

                foreach (string assetName in editorAssetBundle.GetAllAssetNames())
                    if (assetName.StartsWith(iconsPath, StringComparison.OrdinalIgnoreCase) &&
                        (assetName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                         assetName.EndsWith(".asset", StringComparison.OrdinalIgnoreCase)))
                        names.Add(assetName);

                return names.ToArray();
            }


            void InitIcons()
            {
                if (iconContentListSmall != null) return;

                iconNames = FindIconNames();

                IEnumerable<string> all_icons = iconNames.Where(x => GetIcon(x) != null);

                List<string> unique = new List<string>();

                foreach (Texture2D x in Resources.FindObjectsOfTypeAll<Texture2D>())
                {

                    GUIContent icoContent = GetIcon(x.name);
                    if (icoContent == null) continue; // skipped 14 icons 

                    if (!all_icons.Contains(x.name))
                    {
                        unique.Add(x.name);
                    }

                }

                iconNames = iconNames.ToList().Concat(unique).ToArray();

                iconButtonStyle = new GUIStyle(EditorStyles.miniButton);
                iconButtonStyle.margin = new RectOffset(0, 0, 0, 0);
                iconButtonStyle.fixedHeight = 0;

                iconPreviewBlack = new GUIStyle(iconButtonStyle);
                AllTheTEXTURES(ref iconPreviewBlack, Texture2DPixel(new Color(0.15f, 0.15f, 0.15f)));

                iconPreviewWhite = new GUIStyle(iconButtonStyle);
                AllTheTEXTURES(ref iconPreviewWhite, Texture2DPixel(new Color(0.85f, 0.85f, 0.85f)));

                iconMissingNames = new List<string>();
                iconContentListSmall = new List<GUIContent>();
                iconContentListBig = new List<GUIContent>();
                iconContentListAll = new List<GUIContent>();

                for (int i = 0; i < iconNames.Length; ++i)
                {
                    GUIContent ico = GetIcon(iconNames[i]);

                    if (ico == null)
                    {
                        iconMissingNames.Add(iconNames[i]);
                        continue;
                    }

                    ico.tooltip = iconNames[i];

                    iconContentListAll.Add(ico);

                    if (!(ico.image.width <= 36 || ico.image.height <= 36))
                        iconContentListBig.Add(ico);
                    else iconContentListSmall.Add(ico);
                }
            }


        }
    }
}

