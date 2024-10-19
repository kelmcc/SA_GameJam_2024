using System;
using UnityEngine;
using UnityEngine.Assertions;

/*
 * Adapted from code by Jonathan Czeck (aarku)
 */

namespace Framework
{
    /// <summary>
    /// A colour represented by hue, saturation and value values.
    /// </summary>
    [Serializable]
    public struct HSVColour
    {

        [SerializeField]
        private float _h;
        [SerializeField]
        private float _s;
        [SerializeField]
        private float _v;
        [SerializeField]
        private float _a;

        /// <summary>
        /// Hue of the colour.
        /// </summary>
        public float H
        {
            get => _h;
            set => _h = MathUtils.Wrap01(value);
        }

        /// <summary>
        /// Saturation of the colour.
        /// </summary>
        public float S
        {
            get => _s;
            set => _s = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Value of the colour.
        /// </summary>
        public float V
        {
            get => _v;
            set => _v = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Alpha of the colour.
        /// </summary>
        public float A
        {
            get => _a;
            set => _a = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Creates a new colour from hue, saturation and value values.
        /// </summary>
        /// <param name="hue">The colour's hue (normalized)</param>
        /// <param name="saturation">The colour's saturation (normalized)</param>
        /// <param name="value">The colour's value (normalized)</param>
        /// <param name="alpha">The colour's alpha (normalized)</param>
        public HSVColour(float hue, float saturation, float value, float alpha)
        {
            _h = MathUtils.Wrap01(hue);
            _s = Mathf.Clamp01(saturation);
            _v = Mathf.Clamp01(value);
            _a = Mathf.Clamp01(alpha);
        }

        /// <summary>
        /// Creates a new colour from hue, saturation and value values.
        /// </summary>
        /// <param name="hue">The colour's hue (normalized)</param>
        /// <param name="saturation">The colour's saturation (normalized)</param>
        /// <param name="value">The colour's value (normalized)</param>
        public HSVColour(float hue, float saturation, float value)
        {
            _h = MathUtils.Wrap01(hue);
            _s = Mathf.Clamp01(saturation);
            _v = Mathf.Clamp01(value);
            _a = 1f;
        }

        /// <summary>
        /// Creates a new colour from an existing HSVColour.
        /// </summary>
        /// <param name="hue">colour colour</param>
        public HSVColour(HSVColour colour)
        {
            this = new HSVColour(colour._h, colour._s, colour._v, colour._a);
        }

        /// <summary>
        /// Creates a new colour from a hex string.
        /// </summary>
        /// <param name="hex"></param>
        public HSVColour(string hex)
        {
            hex.Replace("#", "");
            Assert.IsTrue(hex.Length == 6, "Invalid hex string");

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            this = new HSVColour(new Color32(r, g, b, 255));
        }

        /// <summary>
        /// Creates a new HSB colour by converting an RGB colour.
        /// </summary>
        /// <param name="colour">The RGB colour</param>
        public HSVColour(Color colour)
        {
            float r = colour.r;
            float g = colour.g;
            float b = colour.b;

            float h = 0;
            float s = 0;
            float max = Mathf.Max(r, Mathf.Max(g, b));

            if (max > 0)
            {

                float min = Mathf.Min(r, Mathf.Min(g, b));
                float diff = max - min;

                if (max > min)
                {
                    if (g == max)
                    {
                        h = (b - r) / diff * 60f + 120f;
                    }
                    else if (b == max)
                    {
                        h = (r - g) / diff * 60f + 240f;
                    }
                    else if (b > g)
                    {
                        h = (g - b) / diff * 60f + 360f;
                    }
                    else
                    {
                        h = (g - b) / diff * 60f;
                    }
                    if (h < 0)
                    {
                        h = h + 360f;
                    }
                }
                else
                {
                    h = 0;
                }

                h *= 1f / 360f;
                s = (diff / max) * 1f;
            }

            _h = MathUtils.Wrap01(h);
            _s = Mathf.Clamp01(s);
            _v = Mathf.Clamp01(max);
            _a = colour.a;
        }


        /// <summary>
        /// Creates an RGB colour from the HSV colour.
        /// </summary>
        /// <returns>The RGB colour</returns>
        public Color ToRGB()
        {
            float r = _v;
            float g = _v;
            float b = _v;

            if (_s != 0)
            {
                float max = _v;
                float diff = _v * _s;
                float min = _v - diff;

                float h = _h * 360f;

                if (h < 60f)
                {
                    r = max;
                    g = h * diff / 60f + min;
                    b = min;
                }
                else if (h < 120f)
                {
                    r = -(h - 120f) * diff / 60f + min;
                    g = max;
                    b = min;
                }
                else if (h < 180f)
                {
                    r = min;
                    g = max;
                    b = (h - 120f) * diff / 60f + min;
                }
                else if (h < 240f)
                {
                    r = min;
                    g = -(h - 240f) * diff / 60f + min;
                    b = max;
                }
                else if (h < 300f)
                {
                    r = (h - 240f) * diff / 60f + min;
                    g = min;
                    b = max;
                }
                else if (h <= 360f)
                {
                    r = max;
                    g = min;
                    b = -(h - 360f) * diff / 60 + min;
                }
                else
                {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }

            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), _a);
        }

        /// <summary>
        /// Hue shifts the colour by a specified amount.
        /// </summary>
        /// <param name="amount">The amount to hue shift (normalized)</param>
        public void HueShift(float amount)
        {
            H += amount;
        }

        /// <summary>
        /// Linearly interpolates two HSV colours based on a normalized time value.
        /// </summary>
        /// <param name="a">The starting colour</param>
        /// <param name="b">The finishing colour</param>
        /// <param name="t">Normalized time value</param>
        /// <returns>The interpolated colour</returns>
        public static HSVColour Lerp(Color a, Color b, float t)
        {
            return Lerp(new HSVColour(a), new HSVColour(b), t);
        }

        /// <summary>
        /// Linearly interpolates two HSV colours based on a normalized time value.
        /// </summary>
        /// <param name="a">The starting HSV colour</param>
        /// <param name="b">The finishing HSV colour</param>
        /// <param name="t">Normalized time value</param>
        /// <returns>The interpolated colour</returns>
        public static HSVColour Lerp(HSVColour a, HSVColour b, float t)
        {

            float h, s;

            if (a._v == 0)
            {
                h = b._h;
                s = b._s;
            }
            else if (b._v == 0)
            {
                h = a._h;
                s = a._s;
            }
            else
            {
                if (a._s == 0)
                {
                    h = b._h;
                }
                else if (b._s == 0)
                {
                    h = a._h;
                }
                else
                {
                    float angle = MathUtils.Wrap(Mathf.LerpAngle(a._h * 360f, b._h * 360f, t), 0, 360);
                    h = angle / 360f;
                }
                s = Mathf.Lerp(a._s, b._s, t);
            }
            return new HSVColour(h, s, Mathf.Lerp(a._v, b._v, t), Mathf.Lerp(a._a, b._a, t));
        }

        /// <summary>
        /// Gets the hex string of this colour.
        /// </summary>
        /// <returns>The colour's hex string</returns>
        public string GetHex()
        {
            return ToRGB().ToHex();
        }

        public override string ToString()
        {
            return "H:" + _h + " S:" + _s + " V:" + _v;
        }



    }
}