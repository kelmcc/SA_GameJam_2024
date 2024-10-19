using UnityEngine;

namespace Framework
{
    public static class FontStyles
    {

        public static GUIStyle Bold => Get(ref _bold, TextAnchor.MiddleCenter, FontStyle.Bold);
        public static GUIStyle Centered => Get(ref _centered, TextAnchor.MiddleCenter, FontStyle.Normal);
        public static GUIStyle MiddleLeftAligned => Get(ref _middleLeftAligned, TextAnchor.MiddleLeft, FontStyle.Normal);
        public static GUIStyle MiddleRightAligned => Get(ref _middleRightAligned, TextAnchor.MiddleRight, FontStyle.Normal);
        public static GUIStyle TopLeftAligned => Get(ref _topLeftAligned, TextAnchor.UpperLeft, FontStyle.Normal);
        public static GUIStyle TopRightAligned => Get(ref _topRightAligned, TextAnchor.UpperRight, FontStyle.Normal);


        private static GUIStyle _bold;
        private static GUIStyle _centered;
        private static GUIStyle _middleLeftAligned;
        private static GUIStyle _middleRightAligned;
        private static GUIStyle _topLeftAligned;
        private static GUIStyle _topRightAligned;

        static GUIStyle Get(ref GUIStyle style, TextAnchor alignment, FontStyle fontStyle)
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.GetStyle("Label"));
                style.alignment = alignment;
                style.fontStyle = fontStyle;
            }

            return style;
        }



    }
}
