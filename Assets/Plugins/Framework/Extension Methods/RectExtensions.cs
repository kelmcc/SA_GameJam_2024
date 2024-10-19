using UnityEngine;

namespace Framework
{
    public static class RectExtensions
    {

        public static Rect[] DivideHorizontallyWithSpacing(this Rect rect, float spacing, params float[] widths)
        {
            float x = rect.x;

            Rect[] rects = new Rect[widths.Length];
            for (int i = 0; i < rects.Length; i++)
            {
                float width = widths[i];
                if (width == 0)
                {
                    width = rect.width;
                    for (int j = 0; j < widths.Length; j++)
                    {
                        width -= widths[j];
                    }
                }

                rects[i] = new Rect(x, rect.y, width, rect.height);
                x += width + spacing;
            }

            return rects;
        }


        public static Rect[] DivideHorizontally(this Rect rect, params float[] widths)
        {
            float x = rect.x;

            Rect[] rects = new Rect[widths.Length];
            for (int i = 0; i < rects.Length; i++)
            {
                float width = widths[i];
                if (width == 0)
                {
                    width = rect.width;
                    for (int j = 0; j < widths.Length; j++)
                    {
                        width -= widths[j];
                    }
                }

                rects[i] = new Rect(x, rect.y, width, rect.height);
                x += width;
            }

            return rects;
        }

        public static Rect[] DivideHorizontallyEqually(this Rect rect, int divisions, float spacing = 0f)
        {
            float x = rect.x;
            float width = (rect.width - (spacing * (divisions - 1))) / divisions;

            Rect[] rects = new Rect[divisions];
            for (int i = 0; i < rects.Length; i++)
            {
                rects[i] = new Rect(x, rect.y, width, rect.height);
                x += width + spacing;
            }

            return rects;
        }

        public static Rect WithWidth(this Rect rect, float width)
        {
            Rect newRect = new Rect(rect);
            newRect.width = width;
            return newRect;
        }

        public static Rect WithHeight(this Rect rect, float height)
        {
            Rect newRect = new Rect(rect);
            newRect.height = height;
            return newRect;
        }

        public static Rect WithPosition(this Rect rect, Vector2 position)
        {
            Rect newRect = new Rect(rect);
            newRect.position = position;
            return newRect;
        }

        public static Rect WithCenter(this Rect rect, Vector2 center)
        {
            Rect newRect = new Rect(rect);
            newRect.center = center;
            return newRect;
        }

        public static Rect WithSize(this Rect rect, Vector2 size)
        {
            Rect newRect = new Rect(rect);
            newRect.size = size;
            return newRect;
        }

        public static Rect WithPosition(this Rect rect, float x, float y)
        {
            Rect newRect = new Rect(rect);
            newRect.position = new Vector2(x, y);
            return newRect;
        }

        public static Rect WithCenter(this Rect rect, float x, float y)
        {
            Rect newRect = new Rect(rect);
            newRect.center = new Vector2(x, y);
            return newRect;
        }

        public static Rect WithSize(this Rect rect, float x, float y)
        {
            Rect newRect = new Rect(rect);
            newRect.size = new Vector2(x, y);
            return newRect;
        }

        public static Rect WithX(this Rect rect, float x)
        {
            Rect newRect = new Rect(rect);
            newRect.x = x;
            return newRect;
        }

        public static Rect WithY(this Rect rect, float y)
        {
            Rect newRect = new Rect(rect);
            newRect.y = y;
            return newRect;
        }

        public static Rect WithAddedLine(this Rect rect, int lineCount = 1)
        {
            Rect newRect = new Rect(rect);
#if UNITY_EDITOR
            newRect.y = rect.y + lineCount * (UnityEditor.EditorGUIUtility.singleLineHeight + UnityEditor.EditorGUIUtility.standardVerticalSpacing);
#else
            newRect.y = rect.y + 20f;
#endif
            return newRect;
        }

        public static bool Contains(this Rect rect, Rect other, bool includeEqual = true)
        {
            if (includeEqual && rect == other) return true;
            if (other.xMin < rect.xMin) return false;
            if (other.yMin < rect.yMin) return false;
            if (other.xMax > rect.xMax) return false;
            if (other.yMax > rect.yMax) return false;
            return true;
        }
    }
}
