using System;
using UnityEngine;

namespace Framework
{
    public static class CanvasExtensions
    {

        public static float GetWidth(this Canvas canvas)
        {
            RectTransform transform = (canvas.transform as RectTransform);
            return transform.rect.width * transform.lossyScale.x;
        }

        public static float GetHeight(this Canvas canvas)
        {
            RectTransform transform = (canvas.transform as RectTransform);
            return transform.rect.height * transform.lossyScale.y;
        }

        public static Vector2 GetDimensions(this Canvas canvas)
        {
            RectTransform transform = (canvas.transform as RectTransform);
            return new Vector2(transform.rect.width * transform.lossyScale.x, transform.rect.height * transform.lossyScale.y);
        }

        public static Rect GetCanvasRect(this Canvas canvas)
        {
            RectTransform transform = (canvas.transform as RectTransform);
            Vector2 size = new Vector2(transform.rect.width * transform.lossyScale.x, transform.rect.height * transform.lossyScale.y);
            Vector2 position = new Vector2(transform.localPosition.x - 0.5f * size.x, transform.localPosition.y - 0.5f * size.y);

            return new Rect(position, size);
        }

        public static float WorldUnitsToCanvasUnits(this Canvas canvas, float worldUnits)
        {
            return worldUnits / canvas.transform.localScale.x;
        }

        public static float CanvasUnitsToWorldUnits(this Canvas canvas, float canvasUnits)
        {
            return canvasUnits * canvas.transform.localScale.x;
        }

        public static float PixelsToWorldUnits(this Canvas canvas, float pixels)
        {
            RectTransform transform = (canvas.transform as RectTransform);
            return pixels * (transform.rect.width / Screen.width * transform.lossyScale.x);
        }

        public static float WorldUnitsToPixels(this Canvas canvas, float worldUnits)
        {
            RectTransform transform = (canvas.transform as RectTransform);
            return worldUnits / (transform.rect.width / Screen.width * transform.lossyScale.x);
        }

        public static float CanvasUnitsToPixels(this Canvas canvas, float canvasUnits)
        {
            RectTransform transform = (canvas.transform as RectTransform);
            return canvasUnits / (transform.rect.width / Screen.width);
        }

        public static float PixelsToCanvasUnits(this Canvas canvas, float pixels)
        {
            RectTransform transform = (canvas.transform as RectTransform);
            return pixels * (transform.rect.width / Screen.width);
        }

        public static Vector3 ScreenSpaceToWorldSpace(this Canvas canvas, Vector2 screenPosition)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return SceneUtils.MainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y));
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera) return canvas.worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, canvas.planeDistance));
            if (canvas.renderMode == RenderMode.WorldSpace) return SceneUtils.MainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y));

            throw new NotImplementedException();
        }

        public static Vector2 WorldSpaceToScreenSpace(this Canvas canvas, Vector3 worldPosition)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return SceneUtils.MainCamera.WorldToScreenPoint(worldPosition).ToVector2();
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera) return canvas.worldCamera.WorldToScreenPoint(worldPosition).ToVector2();
            if (canvas.renderMode == RenderMode.WorldSpace) return SceneUtils.MainCamera.WorldToScreenPoint(worldPosition).ToVector2();

            throw new NotImplementedException();
        }


    }
}
