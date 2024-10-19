using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Static wrapper for mouse input.
    /// </summary>
    public static class Mouse
    {
        public enum Button
        {
            Left = 0,
            Right = 1,
            Middle = 2,
            Fourth = 3,
            Fifth = 4,
            Sixth = 5
        }

        public enum ScrollWheelDirection
        {
            Up,
            Down
        }

        /// <summary>
        /// Whether or not there is a mouse connected and available for use.
        /// </summary>
        public static bool IsAvailable => Input.mousePresent;

        /// <summary>
        /// The position of the mouse in screen coordinates.
        /// </summary>
        public static Vector2 Position => Input.mousePosition;

        /// <summary>
        /// The position of the mouse in world space (Projected using Camera.main).
        /// </summary>
        public static Vector3 WorldPosition => SceneUtils.MainCamera.ScreenToWorldPoint(Input.mousePosition);

        /// <summary>
        /// The mouse movement in this frame.
        /// </summary>
        public static Vector2 PositionDelta => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        /// <summary>
        /// The mouse wheel movement this frame.
        /// </summary>
        public static float ScrollwheelDelta => Input.GetAxis("Mouse ScrollWheel");

        /// <summary>
        /// Whether or not the mouse wheel has been scrolled up this frame.
        /// </summary>
        public static bool HasScrolledUp => Input.GetAxis("Mouse ScrollWheel") > 0;

        /// <summary>
        /// Whether or not the mouse wheel has been scrolled down this frame.
        /// </summary>
        public static bool HasScrolledDown => Input.GetAxis("Mouse ScrollWheel") < 0;

        /// <summary>
        /// Checks whether or not a mouse button is currently pressed.
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>True if the button is currently pressed</returns>
        public static bool GetButton(Button button)
        {
            return Input.GetMouseButton((int)button);
        }

        /// <summary>
        /// Checks whether or not a mouse button was pressed since the last frame.
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>True if the button was pressed since the last frame</returns>
        public static bool GetButtonDown(Button button)
        {
            return Input.GetMouseButtonDown((int)button);
        }

        /// <summary>
        /// Checks whether or not a mouse button was released since the last frame.
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>True if the button was released since the last frame</returns>
        public static bool GetButtonUp(Button button)
        {
            return Input.GetMouseButtonUp((int)button);
        }

        public static bool HasScrolled(ScrollWheelDirection direction)
        {
            return direction == ScrollWheelDirection.Up ? Input.GetAxis("Mouse ScrollWheel") > 0 : Input.GetAxis("Mouse ScrollWheel") < 0;
        }

        public static bool PlaneRaycast(Plane plane, out Vector3 point)
        {
            if (plane.Raycast(SceneUtils.MainCamera.GetMouseRay(), out point))
            {
                return true;
            }

            return false;
        }

        public static bool Raycast(LayerMask layerMask)
        {
            return Physics.Raycast(SceneUtils.MainCamera.GetMouseRay(), Mathf.Infinity, layerMask);
        }

        public static bool Raycast(out RaycastHit hit, LayerMask layerMask)
        {
            if (Physics.Raycast(SceneUtils.MainCamera.GetMouseRay(), out hit, Mathf.Infinity, layerMask))
            {
                return true;
            }

            return false;
        }

        public static bool Raycast(out RaycastHit hit, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            if (Physics.Raycast(SceneUtils.MainCamera.GetMouseRay(), out hit, maxDistance, layerMask, queryTriggerInteraction))
            {
                return true;
            }

            return false;
        }

    }
}
