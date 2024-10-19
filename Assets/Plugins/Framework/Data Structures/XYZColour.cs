using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Represents a colour in XYZ space. (CIE 1931)
    /// </summary>
    [Serializable]
    public struct XYZColour
    {
        [SerializeField]
        private float _x;
        [SerializeField]
        private float _y;
        [SerializeField]
        private float _z;
        [SerializeField]
        private float _a;

        /// <summary>
        /// The colour's X component.
        /// </summary>
        public float X
        {
            get => _x;
            set => _x = Mathf.Clamp(value, 0, 0.9505f);
        }

        /// <summary>
        /// The colour's Y component.
        /// </summary>
        public float Y
        {
            get => _y;
            set => _y = Mathf.Clamp01(value);
        }

        /// <summary>
        /// The colour's Z component.
        /// </summary>
        public float Z
        {
            get => _z;
            set => _z = Mathf.Clamp(value, 0, 1.089f);
        }

        /// <summary>
        /// The colour's Alpha.
        /// </summary>
        public float A
        {
            get => _a;
            set => _a = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Creates a new XYZ colour.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        /// <param name="a">Alpha</param>
        public XYZColour(float x, float y, float z, float a = 1f)
        {
            _x = Mathf.Clamp(x, 0, 0.9505f);
            _y = Mathf.Clamp01(y);
            _z = Mathf.Clamp(z, 0, 1.089f);
            _a = Mathf.Clamp01(a);
        }

        /// <summary>
        /// Creates a new XYZ colour from an RGB colour.
        /// </summary>
        /// <param name="rgbColour">The RGB colour</param>
        public XYZColour(Color rgbColour)
        {
            // Values are rounded to prevent floating point errors causing colours to drift.
            _x = Round(rgbColour.r * 0.4124f + rgbColour.g * 0.3576f + rgbColour.b * 0.1805f);
            _y = Round(rgbColour.r * 0.2126f + rgbColour.g * 0.7152f + rgbColour.b * 0.0722f);
            _z = Round(rgbColour.r * 0.0193f + rgbColour.g * 0.1192f + rgbColour.b * 0.950f);

            _a = rgbColour.a;
        }

        /// <summary>
        /// Creates an RGB colour from the XYZ colour.
        /// </summary>
        /// <returns>The RGB colour</returns>
        public Color ToRGB()
        {
            // (Observer = 2°, Illuminant = D65) Values are rounded to prevent floating point errors causing colours to drift.
            float r = Round(_x * 3.2406f + _y * -1.5372f + _z * -0.4986f);
            float g = Round(_x * -0.9689f + _y * 1.8758f + _z * 0.0415f);
            float b = Round(_x * 0.0557f + _y * -0.2040f + _z * 1.0570f);

            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), _a);
        }

        static float Round(float value)
        {
            return Mathf.Round(value * 1000f) / 1000f;
        }
    }
}
