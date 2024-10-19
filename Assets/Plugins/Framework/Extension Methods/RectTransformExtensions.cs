using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{
    /// <summary>
    /// The most common ways to use a RectTransform horizontal anchor.
    /// </summary>
    public enum RectTransformHorizontalAnchor
    {
        Left,
        Middle,
        Right,
        Stretch
    }

    /// <summary>
    /// The most common ways to use a RectTransform vertical anchor.
    /// </summary>
    public enum RectTransformVerticalAnchor
    {
        Top,
        Middle,
        Bottom,
        Stretch
    }

    /// <summary>
    /// Extension methods for RectTransforms.
    /// </summary>
    public static class RectTransformExtensions
    {
        private static Vector3[] _corners = new Vector3[4];

        /// <summary>
        /// Sets the vertical and horizontal anchor modes of this RectTransform.
        /// </summary>
        /// <param name="horizontalAnchor">The horizontal anchor mode</param>
        /// <param name="verticalAnchor">The vertical anchor mode</param>
        public static void SetAnchors(this RectTransform rectTransform, RectTransformHorizontalAnchor horizontalAnchor, RectTransformVerticalAnchor verticalAnchor)
        {
            rectTransform.SetHorizontalAnchor(horizontalAnchor);
            rectTransform.SetVerticalAnchor(verticalAnchor);
        }

        /// <summary>
        /// Sets the horizontal anchor mode of this RectTransform.
        /// </summary>
        /// <param name="anchorMode">The horizontal anchor mode</param>
        public static void SetHorizontalAnchor(this RectTransform rectTransform, RectTransformHorizontalAnchor anchorMode)
        {
            switch (anchorMode)
            {
                case RectTransformHorizontalAnchor.Left:
                    rectTransform.anchorMin = new Vector2(0, rectTransform.anchorMin.y);
                    rectTransform.anchorMax = new Vector2(0, rectTransform.anchorMax.y);
                    break;
                case RectTransformHorizontalAnchor.Middle:
                    rectTransform.anchorMin = new Vector2(0.5f, rectTransform.anchorMin.y);
                    rectTransform.anchorMax = new Vector2(0.5f, rectTransform.anchorMax.y);
                    break;
                case RectTransformHorizontalAnchor.Right:
                    rectTransform.anchorMin = new Vector2(1, rectTransform.anchorMin.y);
                    rectTransform.anchorMax = new Vector2(1, rectTransform.anchorMax.y);
                    break;
                case RectTransformHorizontalAnchor.Stretch:
                    rectTransform.anchorMin = new Vector2(0, rectTransform.anchorMin.y);
                    rectTransform.anchorMax = new Vector2(1, rectTransform.anchorMax.y);
                    break;
            }
        }

        /// <summary>
        /// Sets the vertical anchor mode of this RectTransform.
        /// </summary>
        /// <param name="anchorMode">The vertical anchor mode</param>
        public static void SetVerticalAnchor(this RectTransform rectTransform, RectTransformVerticalAnchor anchorMode)
        {
            switch (anchorMode)
            {
                case RectTransformVerticalAnchor.Top:
                    rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 1);
                    rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1);
                    break;
                case RectTransformVerticalAnchor.Middle:
                    rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0.5f);
                    rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0.5f);
                    break;
                case RectTransformVerticalAnchor.Bottom:
                    rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0);
                    rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0);
                    break;
                case RectTransformVerticalAnchor.Stretch:
                    rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0);
                    rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1);
                    break;
            }
        }

        /// <summary>
        /// Sets the min and max vertical anchor values of this RectTransform.
        /// </summary>
        /// <param name="min">The minimum anchor value</param>
        /// <param name="max">The maximum anchor value</param>
        public static void SetVerticalAnchor(this RectTransform rectTransform, float min, float max)
        {
            Assert.IsTrue(min >= 0 && min <= 1);
            Assert.IsTrue(max >= 0 && max <= 1);

            rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, min);
            rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, max);
        }

        /// <summary>
        /// Sets the min and max horizontal anchor values of this RectTransform.
        /// </summary>
        /// <param name="min">The minimum anchor value</param>
        /// <param name="max">The maximum anchor value</param>
        public static void SetHorizontalAnchor(this RectTransform rectTransform, float min, float max)
        {
            Assert.IsTrue(min >= 0 && min <= 1);
            Assert.IsTrue(max >= 0 && max <= 1);

            rectTransform.anchorMin = new Vector2(min, rectTransform.anchorMin.y);
            rectTransform.anchorMax = new Vector2(max, rectTransform.anchorMax.y);
        }

        /// <summary>
        /// Gets the horizontal anchor values of this RectTransform.
        /// </summary>
        /// <returns>A FloatRange spanning the horizontal anchor range</returns>
        public static FloatRange GetHorizontalAnchor(this RectTransform rectTransform)
        {
            return new FloatRange(rectTransform.anchorMin.x, rectTransform.anchorMax.x);
        }
        /// <summary>
        /// Gets the vertical anchor values of this RectTransform.
        /// </summary>
        /// <returns>A FloatRange spanning the vertical anchor range</returns>
        public static FloatRange GetVerticalAnchor(this RectTransform rectTransform)
        {
            return new FloatRange(rectTransform.anchorMin.y, rectTransform.anchorMax.y);
        }

        /// <summary>
        /// Returns the left margin value of this RectTransform. Only applicable if the RectTransform is being stretched horizontally.
        /// </summary>
        /// <returns>The left margin value as it appears in the inspector, in local-space pixels</returns>
        public static float GetLeftMargin(this RectTransform rectTransform)
        {
            Assert.IsTrue(rectTransform.anchorMin.x != rectTransform.anchorMax.x, "RectTransform is not being streched on the horizontal axis. Horizontal margins makes no sense with this anchor setup!");

            return rectTransform.offsetMin.x;
        }

        /// <summary>
        /// Returns the right margin value of this RectTransform. Only applicable if the RectTransform is being stretched horizontally.
        /// </summary>
        /// <returns>The right margin value as it appears in the inspector, in local-space pixels</returns>
        public static float GetRightMargin(this RectTransform rectTransform)
        {
            Assert.IsTrue(rectTransform.anchorMin.x != rectTransform.anchorMax.x, "RectTransform is not being streched on the horizontal axis. Horizontal margins makes no sense with this anchor setup!");

            return -rectTransform.offsetMax.x;
        }

        /// <summary>
        /// Returns the top margin value of this RectTransform. Only applicable if the RectTransform is being stretched vertically.
        /// </summary>
        /// <returns>The top margin value as it appears in the inspector, in local-space pixels</returns>
        public static float GetTopMargin(this RectTransform rectTransform)
        {
            Assert.IsTrue(rectTransform.anchorMin.y != rectTransform.anchorMax.y, "RectTransform is not being streched on the vertical axis. Vertical margins makes no sense with this anchor setup!");

            return -rectTransform.offsetMax.y;
        }

        /// <summary>
        /// Returns the bottom margin value of this RectTransform. Only applicable if the RectTransform is being stretched vertically.
        /// </summary>
        /// <returns>The bottom margin value as it appears in the inspector, in local-space pixels</returns>
        public static float GetBottomMargin(this RectTransform rectTransform)
        {
            Assert.IsTrue(rectTransform.anchorMin.y != rectTransform.anchorMax.y, "RectTransform is not being streched on the vertical axis. Vertical margins makes no sense with this anchor setup!");

            return rectTransform.offsetMin.y;
        }


        /// <summary>
        /// Sets the left margin value of this RectTransform. Only applicable if the RectTransform is being stretched horizontally.
        /// </summary>
        /// <param name="margin">The margin value as it would appear in the inspector, in local-space pixels</param>
        public static void SetLeftMargin(this RectTransform rectTransform, float margin)
        {
            Assert.IsTrue(rectTransform.anchorMin.x != rectTransform.anchorMax.x, "RectTransform is not being streched on the horizontal axis. Setting a horizontal margin makes no sense with this anchor setup!");

            rectTransform.offsetMin = new Vector2(margin, rectTransform.offsetMin.y);
        }

        /// <summary>
        /// Sets the right margin value of this RectTransform. Only applicable if the RectTransform is being stretched horizontally.
        /// </summary>
        /// <param name="margin">The margin value as it would appear in the inspector, in local-space pixels</param>
        public static void SetRightMargin(this RectTransform rectTransform, float margin)
        {
            Assert.IsTrue(rectTransform.anchorMin.x != rectTransform.anchorMax.x, "RectTransform is not being streched on the horizontal axis. Setting a horizontal margin makes no sense with this anchor setup!");

            rectTransform.offsetMax = new Vector2(-margin, rectTransform.offsetMax.y);
        }

        /// <summary>
        /// Sets the top margin value of this RectTransform. Only applicable if the RectTransform is being stretched vertically.
        /// </summary>
        /// <param name="margin">The margin value as it would appear in the inspector, in local-space pixels</param>
        public static void SetTopMargin(this RectTransform rectTransform, float margin)
        {
            Assert.IsTrue(rectTransform.anchorMin.y != rectTransform.anchorMax.y, "RectTransform is not being streched on the vertical axis. Setting a vertical margin makes no sense with this anchor setup!");

            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -margin);
        }

        /// <summary>
        /// Sets the bottom margin value of this RectTransform. Only applicable if the RectTransform is being stretched vertically.
        /// </summary>
        /// <param name="margin">The margin value as it would appear in the inspector, in local-space pixels</param>
        public static void SetBottomMargin(this RectTransform rectTransform, float margin)
        {
            Assert.IsTrue(rectTransform.anchorMin.y != rectTransform.anchorMax.y, "RectTransform is not being streched on the vertical axis. Setting a vertical margin makes no sense with this anchor setup!");

            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, margin);
        }

        /// <summary>
        /// Sets the margin values of this RectTransform. Only applicable if the RectTransform is being stretched both horizontally and vertically.
        /// </summary>
        /// <param name="left">The left margin value as it would appear in the inspector, in local-space pixels</param>
        /// <param name="right">The right margin value as it would appear in the inspector, in local-space pixels</param>
        /// <param name="top">The top margin value as it would appear in the inspector, in local-space pixels</param>
        /// <param name="bottom">The bottom margin value as it would appear in the inspector, in local-space pixels</param>
        public static void SetMargins(this RectTransform rectTransform, float left, float right, float top, float bottom)
        {
            Assert.IsTrue(rectTransform.anchorMin.x != rectTransform.anchorMax.x, "RectTransform is not being streched on the horizontal axis. Setting a horizontal margin makes no sense with this anchor setup!");
            Assert.IsTrue(rectTransform.anchorMin.y != rectTransform.anchorMax.y, "RectTransform is not being streched on the vertical axis. Setting a vertical margin makes no sense with this anchor setup!");

            rectTransform.offsetMin = new Vector2(left, bottom);
            rectTransform.offsetMax = new Vector2(-right, -top);
        }

        /// <summary>
        /// Sets the margin values of this RectTransform. Only applicable if the RectTransform is being stretched both horizontally and vertically.
        /// </summary>
        /// <param name="margins">A vector representing the four margin values (left, right, top, bottom), in local-space pixels</param>
        public static void SetMargins(this RectTransform rectTransform, Vector4 margins)
        {
            Assert.IsTrue(rectTransform.anchorMin.x != rectTransform.anchorMax.x, "RectTransform is not being streched on the horizontal axis. Setting a horizontal margin makes no sense with this anchor setup!");
            Assert.IsTrue(rectTransform.anchorMin.y != rectTransform.anchorMax.y, "RectTransform is not being streched on the vertical axis. Setting a vertical margin makes no sense with this anchor setup!");

            rectTransform.offsetMin = new Vector2(margins.x, margins.w);
            rectTransform.offsetMax = new Vector2(-margins.y, -margins.z);
        }

        /// <summary>
        /// Gets margin values of this RectTransform as they appear in the inspector. Only applicable if the RectTransform is being stretched both horizontally and vertically.
        /// </summary>
        /// <returns>The margin values (left, right, top, bottom), in local-space pixels</returns>
        public static Vector4 GetMargins(this RectTransform rectTransform)
        {
            Assert.IsTrue(rectTransform.anchorMin.x != rectTransform.anchorMax.x, "RectTransform is not being streched on the horizontal axis. Horizontal margins make no sense with this anchor setup!");
            Assert.IsTrue(rectTransform.anchorMin.y != rectTransform.anchorMax.y, "RectTransform is not being streched on the vertical axis. Vertical margins make no sense with this anchor setup!");

            return new Vector4(rectTransform.offsetMin.x, -rectTransform.offsetMax.x, -rectTransform.offsetMax.y, rectTransform.offsetMin.y);
        }

        /// <summary>
        /// Tests whether this RectTransform's anchor is set up to stretch the rect horizontally.
        /// </summary>
        /// <returns>True if the RectTransform will stretch horizontally based on the width of its parent rect</returns>
        public static bool IsStretchedHorizontally(this RectTransform rectTransform)
        {
            return rectTransform.anchorMin.x != rectTransform.anchorMax.x;
        }

        /// <summary>
        /// Tests whether this RectTransform's anchor is set up to stretch the rect vertically.
        /// </summary>
        /// <returns>True if the RectTransform will stretch vertically based on the height of its parent rect</returns>
        public static bool IsStretchedVertically(this RectTransform rectTransform)
        {
            return rectTransform.anchorMin.y != rectTransform.anchorMax.y;
        }

        /// <summary>
        /// Sets the horizontal local offset of this RectTransform (ie. The local x-position relative to the anchor). Only applicable if the RectTransform is not being stretched on the horizontal axis.
        /// </summary>
        /// <param name="offset">The horizontal offset value as it would appear in the inspector, in local-space pixels</param>
        public static void SetLocalOffsetX(this RectTransform rectTransform, float offset)
        {
            Assert.IsTrue(rectTransform.anchorMin.x == rectTransform.anchorMax.x, "RectTransform is being streched on the horizontal axis. Setting a horizontal offset makes no sense with this anchor setup!");

            rectTransform.anchoredPosition = new Vector2(offset, rectTransform.anchoredPosition.x);
        }

        /// <summary>
        /// Sets the vertical local offset of this RectTransform (ie. The local y-position relative to the anchor). Only applicable if the RectTransform is not being stretched on the vertical axis.
        /// </summary>
        /// <param name="offset">The vertical offset value as it would appear in the inspector, in local-space pixels</param>
        public static void SetLocalOffsetY(this RectTransform rectTransform, float offset)
        {
            Assert.IsTrue(rectTransform.anchorMin.y == rectTransform.anchorMax.y, "RectTransform is being streched on the vertical axis. Setting a vertical offset makes no sense with this anchor setup!");

            rectTransform.anchoredPosition = new Vector2(offset, rectTransform.anchoredPosition.y);
        }

        /// <summary>
        /// Sets the vertical local offset values of this RectTransform (ie. The local position relative to the anchor). Only applicable if the RectTransform is not being stretched horizontally or vertically.
        /// </summary>
        /// <param name="offset">The offset, in local-space pixels</param>
        public static void SetLocalOffset(this RectTransform rectTransform, Vector2 offset)
        {
            Assert.IsTrue(rectTransform.anchorMin.x == rectTransform.anchorMax.x, "RectTransform is being streched on the horizontal axis. Setting a horizontal offset makes no sense with this anchor setup! Use margins instead?");
            Assert.IsTrue(rectTransform.anchorMin.y == rectTransform.anchorMax.y, "RectTransform is being streched on the vertical axis. Setting a vertical offset makes no sense with this anchor setup! Use margins instead?");

            rectTransform.anchoredPosition = offset;
        }

        /// <summary>
        /// Gets the horizontal local offset of this RectTransform (ie. The local x-position relative to the anchor). Only applicable if the RectTransform is not being stretched on the horizontal axis.
        /// </summary>
        /// <returns>The horizontal offset value as it appears in the inspector, in local-space pixels</returns>
        public static float GetLocalOffsetX(this RectTransform rectTransform)
        {
            Assert.IsTrue(rectTransform.anchorMin.x == rectTransform.anchorMax.x, "RectTransform is being streched on the horizontal axis. Horizontal offset makes no sense with this anchor setup! Use margins instead?");

            return rectTransform.anchoredPosition.x;
        }

        /// <summary>
        /// Gets the vertical local offset of this RectTransform (ie. The local y-position relative to the anchor). Only applicable if the RectTransform is not being stretched on the vertical axis.
        /// </summary>
        /// <returns>The vertical offset value as it appears in the inspector, in local-space pixels</returns>
        public static float GetLocalOffsetY(this RectTransform rectTransform)
        {
            Assert.IsTrue(rectTransform.anchorMin.y == rectTransform.anchorMax.y, "RectTransform is being streched on the vertical axis. Vertical offset makes no sense with this anchor setup! Use margins instead?");

            return rectTransform.anchoredPosition.y;
        }

        /// <summary>
        /// Gets the local offset of this RectTransform (ie. The local position relative to the anchor). Only applicable if the RectTransform is not being stretched on the vertical or horizontal axis.
        /// </summary>
        /// <returns>The local offset values as they appear in the inspector, in local-space pixels</returns>
        public static Vector2 GetLocalOffset(this RectTransform rectTransform)
        {
            Assert.IsTrue(rectTransform.anchorMin.x == rectTransform.anchorMax.x, "RectTransform is being streched on the horizontal axis. Horizontal offset makes no sense with this anchor setup! Use margins instead?");
            Assert.IsTrue(rectTransform.anchorMin.y == rectTransform.anchorMax.y, "RectTransform is being streched on the vertical axis. Vertical offset makes no sense with this anchor setup! Use margins instead?");

            return rectTransform.anchoredPosition;
        }

        /// <summary>
        /// Sets the local width of this RectTransform. This is the "Width" value as it would appear in the inspector. Only applicable if the RectTransform is not being stretched on the horizontal axis.
        /// </summary>
        /// <param name="width">The width value, in local-space pixels</param>
        public static void SetLocalWidth(this RectTransform rectTransform, float width)
        {
            Assert.IsTrue(rectTransform.anchorMin.x == rectTransform.anchorMax.x, "RectTransform is being streched on the horizontal axis. Setting a local width makes no sense with this anchor setup!");

            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        }

        /// <summary>
        /// Sets the local height of this RectTransform. This is the "Height" value as it would appear in the inspector. Only applicable if the RectTransform is not being stretched on the vertical axis.
        /// </summary>
        /// <param name="height">The height value, in local pixels</param>
        public static void SetLocalHeight(this RectTransform rectTransform, float height)
        {
            Assert.IsTrue(rectTransform.anchorMin.y == rectTransform.anchorMax.y, "RectTransform is being streched on the vertical axis. Setting a local height makes no sense with this anchor setup!");

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }

        /// <summary>
        /// Sets the local width and height of this RectTransform. These are the "Width" and "Height" value as they would appear in the inspector. Only applicable if the RectTransform is not being stretched horizontally or vertically.
        /// </summary>
        /// <param name="dimensions">The values, in local-space pixels</param>
        public static void SetLocalDimensions(this RectTransform rectTransform, Vector2 dimensions)
        {
            Assert.IsTrue(rectTransform.anchorMin.x == rectTransform.anchorMax.x, "RectTransform is being streched on the horizontal axis. Setting a local width makes no sense with this anchor setup!");
            Assert.IsTrue(rectTransform.anchorMin.y == rectTransform.anchorMax.y, "RectTransform is being streched on the vertical axis. Setting a local height makes no sense with this anchor setup!");

            rectTransform.sizeDelta = dimensions;
        }

        /// <summary>
        /// Gets the local width of this RectTransform. This is the "Width" value as it appears in the inspector.
        /// </summary>
        /// <returns>The width value, in local-space pixels</returns>
        public static float GetLocalWidth(this RectTransform rectTransform)
        {
            return rectTransform.rect.width;
        }

        /// <summary>
        /// Gets the local height of this RectTransform. This is the "Height" value as it appears in the inspector.
        /// </summary>
        /// <returns>The height value, in local-space pixels</returns>
        public static float GetLocalHeight(this RectTransform rectTransform)
        {
            return rectTransform.rect.height;
        }

        /// <summary>
        /// Gets the position of this RectTransform on the screen. Only valid for un-rotated RectTransforms.
        /// </summary>
        /// <returns>The absoloute position of the rectangle, in screen-space pixels</returns>
        public static Vector2 GetScreenSpacePosition(this RectTransform rectTransform, Canvas canvas)
        {
            rectTransform.GetWorldCorners(_corners);

            _corners[0] = canvas.WorldSpaceToScreenSpace(_corners[0]);

            return new Vector2(_corners[0].x, _corners[0].y);
        }


        /// <summary>
        /// Gets the width and height of this RectTransform on the screen. Only valid for un-rotated RectTransforms.
        /// </summary>
        /// <param name="canvas">The Canvas this RectTransform is parented to</param>
        /// <returns>The absoloute dimensions of the rectangle, in screen-space pixels/returns>
        public static Vector2 GetScreenSpaceDimensions(this RectTransform rectTransform, Canvas canvas)
        {
            rectTransform.GetWorldCorners(_corners);

            _corners[0] = canvas.WorldSpaceToScreenSpace(_corners[0]);
            _corners[2] = canvas.WorldSpaceToScreenSpace(_corners[2]);

            return new Vector2(Mathf.Abs(_corners[2].x - _corners[0].x), Mathf.Abs(_corners[2].y - _corners[0].y));
        }

        /// <summary>
        /// Gets the width of this RectTransform on the screen. Only valid for un-rotated RectTransforms.
        /// </summary>
        /// <param name="canvas">The Canvas this RectTransform is parented to</param>
        /// <returns>The absoloute width of the rectangle, in screen-space pixels/returns>
        public static float GetScreenSpaceWidth(this RectTransform rectTransform, Canvas canvas)
        {
            rectTransform.GetWorldCorners(_corners);

            _corners[0] = canvas.WorldSpaceToScreenSpace(_corners[0]);
            _corners[2] = canvas.WorldSpaceToScreenSpace(_corners[2]);

            return Mathf.Abs(_corners[2].x - _corners[0].x);
        }

        /// <summary>
        /// Gets the height of this RectTransform on the screen. Only valid for un-rotated RectTransforms.
        /// </summary>
        /// <param name="canvas">The Canvas this RectTransform is parented to</param>
        /// <returns>The absoloute height of the rectangle, in screen-space pixels/returns>
        public static float GetScreenSpaceHeight(this RectTransform rectTransform, Canvas canvas)
        {
            rectTransform.GetWorldCorners(_corners);

            _corners[0] = canvas.WorldSpaceToScreenSpace(_corners[0]);
            _corners[2] = canvas.WorldSpaceToScreenSpace(_corners[2]);

            return Mathf.Abs(_corners[2].y - _corners[0].y);
        }

        /// <summary>
        /// Tests whether a screen-space point falls within this RectTransform's bounds. Only valid for un-rotated RectTransforms.
        /// </summary>
        /// <param name="canvas">The Canvas this RectTransform is parented to</param>
        /// <param name="point">The screen-space point to test</param>
        /// <returns>True if the rectangle contains the point</returns>
        public static bool ContainsScreenSpacePoint(this RectTransform rectTransform, Canvas canvas, Vector2 point)
        {
            return GetScreenSpaceRect(rectTransform, canvas).Contains(point);
        }

        /// <summary>
        /// Returns the normalized position of a screen-space point within this RectTransform's rectangle. Eg. if the point is exactly in the middle of the top edge, this will return (0.5, 1). Only valid for un-rotated RectTransforms.
        /// </summary>
        /// <param name="canvas">The Canvas this RectTransform is parented to</param>
        /// <param name="point">The screen-space point to test</param>
        /// <returns>The point normalized relative to the RectTransform's rectangle.</returns>
        public static Vector2 GetNormalizedPositionOfScreenSpacePoint(this RectTransform rectTransform, Canvas canvas, Vector2 point)
        {
            Rect rect = GetScreenSpaceRect(rectTransform, canvas);
            return new Vector2((point.x - rect.xMin) / rect.width, (point.y - rect.yMin) / rect.height);
        }


        /// <summary>
        /// Returns a Rect representing this RectTransform as it appears on the screen. Only valid for un-rotated RectTransforms.
        /// </summary>
        /// <param name="canvas">The Canvas this RectTransform is parented to</param>
        /// <returns>The Rect, in screen-space pixels</returns>
        public static Rect GetScreenSpaceRect(this RectTransform rectTransform, Canvas canvas)
        {
            rectTransform.GetWorldCorners(_corners);

            _corners[0] = canvas.WorldSpaceToScreenSpace(_corners[0]);
            _corners[2] = canvas.WorldSpaceToScreenSpace(_corners[2]);

            return new Rect(_corners[0].x, _corners[0].y, Mathf.Abs(_corners[2].x - _corners[0].x), Mathf.Abs(_corners[2].y - _corners[0].y));
        }

        public static Rect GetWorldSpaceRect(this RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners(_corners);

            return new Rect(_corners[0].x, _corners[0].y, Mathf.Abs(_corners[2].x - _corners[0].x), Mathf.Abs(_corners[2].y - _corners[0].y));
        }

        /// <summary>
        /// Sets the screen-space width and height of this RectTransform.  Only applicable if the RectTransform is not being stretched on the horizontal axis. Only applicable if the RectTransform is not being stretched horizontally or vertically.
        /// </summary>
        /// <param name="canvas">The Canvas this RectTransform is parented to</param>
        /// <param name="dimensions">The values, in screen-space pixels</param>
        public static void SetScreenSpaceDimensions(this RectTransform rectTransform, Canvas canvas, Vector2 dimensions)
        {
            Assert.IsTrue(rectTransform.anchorMin.x == rectTransform.anchorMax.x, "RectTransform is being streched on the horizontal axis. Setting a horizontal width makes no sense with this anchor setup!");
            Assert.IsTrue(rectTransform.anchorMin.y == rectTransform.anchorMax.y, "RectTransform is being streched on the vertical axis. Setting a vertical height makes no sense with this anchor setup!");

            float ratio = (canvas.transform as RectTransform).rect.width / Screen.width;
            rectTransform.sizeDelta = dimensions * ratio;
        }



        /// <summary>
        /// Sets the scree-space height of this RectTransform. Only applicable if the RectTransform is not being stretched on the vertical axis.
        /// </summary>
        /// <param name="canvas">The Canvas this RectTransform is parented to</param>
        /// <param name="height">The height value, in screen-space pixels</param>
        public static void SetScreenSpaceHeight(this RectTransform rectTransform, Canvas canvas, float height)
        {
            Assert.IsTrue(rectTransform.anchorMin.y == rectTransform.anchorMax.y, "RectTransform is being streched on the vertical axis. Setting a vertical height makes no sense with this anchor setup!");

            float ratio = (canvas.transform as RectTransform).rect.width / Screen.width;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height * ratio);
        }

        /// <summary>
        /// Sets the scree-space width of this RectTransform. Only applicable if the RectTransform is not being stretched on the horizontal axis.
        /// </summary>
        /// <param name="canvas">The Canvas this RectTransform is parented to</param>
        /// <param name="width">The width value, in screen-space pixels</param>
        public static void SetScreenSpaceWidth(this RectTransform rectTransform, Canvas canvas, float width)
        {
            Assert.IsTrue(rectTransform.anchorMin.x == rectTransform.anchorMax.x, "RectTransform is being streched on the horizontal axis. Setting a horizontal width makes no sense with this anchor setup!");

            float ratio = (canvas.transform as RectTransform).rect.width / Screen.width;
            rectTransform.sizeDelta = new Vector2(width * ratio, rectTransform.sizeDelta.y);
        }



        /// <summary>
        /// Gets the local position of this RectTransform. This is the position relative to the pivot of the parent rectangle. For positions relative to the anchor position, use local offsets.
        /// </summary>
        /// <param name="ofCorner">If true, return the local position of the bottom left corner of the rectangle. Otherwise return the local position of the rectangle's pivot</param>
        /// <returns>The local position, in local-space pixels</returns>
        public static Vector2 GetLocalPosition(this RectTransform rectTransform)
        {
            return new Vector2(rectTransform.localPosition.x - (rectTransform.rect.width * rectTransform.pivot.x), rectTransform.localPosition.y - (rectTransform.rect.height * rectTransform.pivot.y));
        }

        /// <summary>
        /// Gets the width and height of this RectTransform. These are the "Width" and "Height" values as they appear in the inspector. Only applicable if the RectTransform is not being stretched horizontally or vertically.
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetLocalDimensions(this RectTransform rectTransform)
        {
            Assert.IsTrue(rectTransform.anchorMin.x == rectTransform.anchorMax.x, "RectTransform is being streched on the horizontal axis. Setting a horizontal width makes no sense with this anchor setup!");
            Assert.IsTrue(rectTransform.anchorMin.y == rectTransform.anchorMax.y, "RectTransform is being streched on the vertical axis. Setting a vertical height makes no sense with this anchor setup!");

            return new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        }


    }
}