using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Extension methods for cardinal enums.
    /// </summary>
    public static class CardinalExtensions
    {
        public static Cardinal4 Reverse(this Cardinal4 cardinal)
        {
            int index = (int)cardinal + 2;
            return (Cardinal4)(index > 3 ? index - 4 : index);
        }

        /// <summary>
        /// Returns this cardinal rotated clockwise by a certain number of 90 degree rotations.
        /// </summary>
        /// <param name="rotations">The number of 90 degree rotations to perform</param>
        /// <returns>The rotated cardinal</returns>
        public static Cardinal4 RotatedClockwise(this Cardinal4 cardinal, int rotations = 1)
        {
            return (Cardinal4)MathUtils.Wrap((int)cardinal + rotations, 0, 3);
        }

        /// <summary>
        /// Returns this cardinal rotated clockwise by a certain number of 45 degree rotations.
        /// </summary>
        /// <param name="rotations">The number of 90 degree rotations to perform</param>
        /// <returns>The rotated cardinal</returns>
        public static Cardinal8 RotatedClockwise(this Cardinal8 cardinal, int rotations = 1)
        {
            return (Cardinal8)MathUtils.Wrap((int)cardinal + rotations, 0, 7);
        }

        /// <summary>
        /// Returns this cardinal rotated anti-clockwise by a certain number of 90 degree rotations.
        /// </summary>
        /// <param name="rotations">The number of 90 degree rotations to perform</param>
        /// <returns>The rotated cardinal</returns>
        public static Cardinal4 RotatedAntiClockwise(this Cardinal4 cardinal, int rotations = 1)
        {
            return (Cardinal4)MathUtils.Wrap((int)cardinal - rotations, 0, 3);
        }

        /// <summary>
        /// Returns this cardinal rotated anti-clockwise by a certain number of 45 degree rotations.
        /// </summary>
        /// <param name="rotations">The number of 90 degree rotations to perform</param>
        /// <returns>The rotated cardinal</returns>
        public static Cardinal8 RotatedAntiClockwise(this Cardinal8 cardinal, int rotations = 1)
        {
            return (Cardinal8)MathUtils.Wrap((int)cardinal - rotations, 0, 7);
        }

        /// <summary>
        /// Returns the vector representation of this cardinal.
        /// </summary>
        /// <returns>This cardinal as a Vector2</returns>
        public static Vector2 ToVector(this Cardinal4 cardinal)
        {
            switch (cardinal)
            {
                case Cardinal4.Up: return Vector2.up;
                case Cardinal4.Down: return -Vector2.up;
                case Cardinal4.Left: return -Vector2.right;
                case Cardinal4.Right: return Vector2.right;
            }

            throw new ArgumentException();
        }

        public static Vector3 ToVector3XZ(this Cardinal8 cardinal)
        {

            switch (cardinal)
            {
                case Cardinal8.North: return Vector3.forward;
                case Cardinal8.South: return -Vector3.forward;
                case Cardinal8.East: return Vector3.right;
                case Cardinal8.West: return -Vector3.right;

                case Cardinal8.NorthEast: return new Vector3(1, 0, 1).normalized;
                case Cardinal8.NorthWest: return new Vector3(-1, 0, 1).normalized;
                case Cardinal8.SouthEast: return new Vector3(1, 0, -1).normalized;
                case Cardinal8.SouthWest: return new Vector3(-1, 0, -1).normalized;
            }


            throw new ArgumentException();
        }


        public static Vector3 ToVector3XZ(this Cardinal4 cardinal)
        {
            switch (cardinal)
            {
                case Cardinal4.Up: return Vector3.forward;
                case Cardinal4.Down: return -Vector3.forward;
                case Cardinal4.Left: return -Vector3.right;
                case Cardinal4.Right: return Vector3.right;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Returns the vector representation of this cardinal.
        /// </summary>
        /// <returns>This cardinal as a Vector2</returns>
        public static Vector2 ToVector(this Cardinal8 cardinal)
        {
            switch (cardinal)
            {
                case Cardinal8.North: return Vector2.up;
                case Cardinal8.South: return -Vector2.up;
                case Cardinal8.East: return Vector2.right;
                case Cardinal8.West: return -Vector2.right;

                case Cardinal8.NorthEast: return new Vector2(1, 1).normalized;
                case Cardinal8.NorthWest: return new Vector2(-1, 1).normalized;
                case Cardinal8.SouthEast: return new Vector2(1, -1).normalized;
                case Cardinal8.SouthWest: return new Vector2(-1, -1).normalized;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Returns this Cardinal8 as a Cardinal4, 45 degree cardinals are snapped to the cardinal that is 45 degrees anti-clockwise.
        /// </summary>
        /// <returns>This Cardinal8 as a Cardinal4</returns>
        public static Cardinal4 ToCardinal4(this Cardinal8 cardinal)
        {
            switch (cardinal)
            {
                case Cardinal8.North: return Cardinal4.Up;
                case Cardinal8.South: return Cardinal4.Down;
                case Cardinal8.East: return Cardinal4.Right;
                case Cardinal8.West: return Cardinal4.Left;

                case Cardinal8.NorthEast: return Cardinal4.Up;
                case Cardinal8.NorthWest: return Cardinal4.Left;
                case Cardinal8.SouthEast: return Cardinal4.Right;
                case Cardinal8.SouthWest: return Cardinal4.Down;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Returns this Cardinal4 as a Cardinal8.
        /// </summary>
        /// <returns>This Cardinal4 as a Cardinal8</returns>
        public static Cardinal8 ToCardinal8(this Cardinal4 cardinal)
        {
            switch (cardinal)
            {
                case Cardinal4.Up: return Cardinal8.North;
                case Cardinal4.Down: return Cardinal8.South;
                case Cardinal4.Left: return Cardinal8.West;
                case Cardinal4.Right: return Cardinal8.East;
            }

            throw new ArgumentException();
        }


        public static float Angle(this Cardinal4 cardinal)
        {
            switch (cardinal)
            {
                case Cardinal4.Up: return 0f;
                case Cardinal4.Down: return 180f;
                case Cardinal4.Left: return 270f;
                case Cardinal4.Right: return 90f;
            }

            throw new ArgumentException();
        }

        public static float Angle(this Cardinal8 cardinal)
        {
            switch (cardinal)
            {
                case Cardinal8.North: return 0f;
                case Cardinal8.South: return 180f;
                case Cardinal8.East: return 90f;
                case Cardinal8.West: return 270f;

                case Cardinal8.NorthEast: return 45f;
                case Cardinal8.NorthWest: return 315f;
                case Cardinal8.SouthEast: return 135f;
                case Cardinal8.SouthWest: return 225f;
            }

            throw new ArgumentException();
        }

        public static bool IsDiagonal(this Cardinal8 cardinal)
        {
            switch (cardinal)
            {
                case Cardinal8.NorthEast: return true;
                case Cardinal8.NorthWest: return true;
                case Cardinal8.SouthEast: return true;
                case Cardinal8.SouthWest: return true;
            }

            return false;
        }

    }
}
