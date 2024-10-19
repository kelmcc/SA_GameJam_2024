using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Represents a colour in CIELAB space.
    /// </summary>
    [Serializable]
    public struct LABColour
    {
        /// <summary>
        /// The colour's lightness;
        /// </summary>
        public float L
        {
            get => _l;
            set => _l = Mathf.Clamp(value, 0, 100f);
        }

        /// <summary>
        /// The colour's A opponent dimension.
        /// </summary>
        public float A
        {
            get => _a;
            set => _a = Mathf.Clamp01(value);
        }

        /// <summary>
        /// The colour's B opponent dimension.
        /// </summary>
        public float B
        {
            get => _b;
            set => _b = Mathf.Clamp01(value);
        }

        /// <summary>
        /// The colour's alpha.
        /// </summary>
        public float Alpha
        {
            get => _alpha;
            set => _alpha = Mathf.Clamp01(value);
        }

        [SerializeField]
        private float _l;
        [SerializeField]
        private float _a;
        [SerializeField]
        private float _b;
        [SerializeField]
        private float _alpha;

        // D65 Illuminent white point
        const float CIEXYZ_D65_X = 0.9505f;
        const float CIEXYZ_D65_Y = 1f;
        const float CIEXYZ_D65_Z = 1.0890f;

        const float EPSILON = 0.008856f; // Intent is 216/24389
        const float KAPPA = 903.3f; // Intent is 24389/27

        /// <summary>
        /// Creates a new LAB colour.
        /// </summary>
        /// <param name="l">Lightness</param>
        /// <param name="a">A component dimension</param>
        /// <param name="b">B component dimension</param>
        /// <param name="alpha">Alpha</param>
        public LABColour(float l, float a, float b, float alpha = 1f)
        {
            _l = l;
            _a = a;
            _b = b;
            _alpha = Mathf.Clamp01(alpha);
        }

        /// <summary>
        /// Creates a new LAB colour from an RGB colour.
        /// </summary>
        /// <param name="rgbColour">The RGB colour</param>
        public LABColour(Color rgbColour)
        {
            XYZColour xyz = new XYZColour(rgbColour);

            _l = 116.0f * Fxyz(xyz.Y / CIEXYZ_D65_Y) - 16f;
            _a = 500.0f * (Fxyz(xyz.X / CIEXYZ_D65_X) - Fxyz(xyz.Y / CIEXYZ_D65_Y));
            _b = 200.0f * (Fxyz(xyz.Y / CIEXYZ_D65_Y) - Fxyz(xyz.Z / CIEXYZ_D65_Z));

            _alpha = rgbColour.a;
        }

        static float Fxyz(float t)
        {
            return ((t > 0.008856f) ? Mathf.Pow(t, (1.0f / 3.0f)) : (7.787f * t + 16f / 116.0f));
        }

        /// <summary>
        /// Creates an RGB colour from the LAB colour.
        /// </summary>
        /// <returns>The RGB colour</returns>
        public Color ToRGB()
        {
            float y = (_l + 16.0f) / 116.0f;
            float x = _a / 500.0f + y;
            float z = y - _b / 200.0f;

            float x3 = x * x * x;
            float z3 = z * z * z;

            x = CIEXYZ_D65_X * (x3 > EPSILON ? x3 : (x - 16f / 116.0f) / 7.787f);
            y = CIEXYZ_D65_Y * (_l > (KAPPA * EPSILON) ? Mathf.Pow(((_l + 16f) / 116.0f), 3f) : _l / KAPPA);
            z = CIEXYZ_D65_Z * (z3 > EPSILON ? z3 : (z - 16f / 116.0f) / 7.787f);

            return new XYZColour(x, y, z, _alpha).ToRGB();
        }

    }
}
