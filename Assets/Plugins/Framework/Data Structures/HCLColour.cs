using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// A representation of a colour in the Hue-Chroma-Lightness space. HCL colours are better for colour interpolation because they don't go through grey.
    /// </summary>
    [Serializable]
    public struct HCLColour
    {
        [SerializeField]
        private float _h;
        [SerializeField]
        private float _c;
        [SerializeField]
        private float _l;
        [SerializeField]
        private float _a;

        /// <summary>
        /// The colour's hue.
        /// </summary>
        public float H
        {
            get => _h;
            set => _h = value;
        }

        /// <summary>
        /// The colour's chroma.
        /// </summary>
        public float C
        {
            get => _c;
            set => _c = value;
        }

        /// <summary>
        /// The colour's lightness.
        /// </summary>
        public float L
        {
            get => _l;
            set => _l = value;
        }

        /// <summary>
        /// The colour's aplha.
        /// </summary>
        public float A
        {
            get => _a;
            set => _a = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Creates a new HCL colour.
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="c">Chroma</param>
        /// <param name="l">Lightness</param>
        /// <param name="a">Alpha</param>
        public HCLColour(float h, float c, float l, float a = 1f)
        {
            _h = h;
            _c = c;
            _l = l;
            _a = Mathf.Clamp01(a);
        }

        /// <summary>
        /// Creates a new HCL colour from an RGB colour
        /// </summary>
        /// <param name="rgbColour">The RGB colour</param>
        public HCLColour(Color rgbColour)
        {
            LABColour lab = new LABColour(rgbColour);
            _h = Mathf.Atan2(lab.B, lab.A);

            // convert from radians to degrees
            if (_h > 0)
            {
                _h = (_h / Mathf.PI) * 180.0f;
            }
            else
            {
                _h = 360 - (Mathf.Abs(_h) / Mathf.PI) * 180.0f;
            }

            if (_h < 0)
            {
                _h += 360.0f;
            }
            else if (_h >= 360f)
            {
                _h -= 360.0f;
            }

            _l = lab.L;
            _c = Mathf.Sqrt(lab.A * lab.A + lab.B * lab.B);
            _a = rgbColour.a;
        }

        /// <summary>
        /// Creates an RGB colour from the HCL colour.
        /// </summary>
        /// <returns>The RGB colour</returns>
        public Color ToRGB()
        {
            float hRadians = _h * Mathf.Deg2Rad;
            return new LABColour(_l, Mathf.Cos(hRadians) * _c, Mathf.Sin(hRadians) * _c, _a).ToRGB();
        }

        /// <summary>
        /// Interpolates two RGB colours using HCL interpolation.
        /// </summary>
        /// <param name="fromColour">Source colour</param>
        /// <param name="toColour">Destination colour</param>
        /// <param name="t">Normalized interpolation timee</param>
        /// <returns>The interpolated colour at time t</returns>
        public static HCLColour Lerp(Color fromColour, Color toColour, float t)
        {
            return Lerp(new HCLColour(fromColour), new HCLColour(toColour), t);
        }

        /// <summary>
        /// Interpolates two HCL colours.
        /// </summary>
        /// <param name="fromColour">Source colour</param>
        /// <param name="toColour">Destination colour</param>
        /// <param name="t">Normalized interpolation time</param>
        /// <returns>The interpolated colour at time t</returns>
        public static HCLColour Lerp(HCLColour fromColour, HCLColour toColour, float t)
        {
            t = Mathf.Clamp01(t);

            float diff = toColour.H - fromColour.H;

            if (Mathf.Abs(diff) > 180)
            {
                if (diff > 0)
                {
                    fromColour._h += 360;
                }
                else
                {
                    toColour._h += 360;
                }
            }

            float l = fromColour.L + (toColour.L - fromColour.L) * t;
            float c = fromColour.C + (toColour.C - fromColour.C) * t;
            float h = fromColour.H + (toColour.H - fromColour.H) * t;
            float a = fromColour.A + (toColour.A - fromColour.A) * t;

            return new HCLColour(h, c, l, a);
        }

        public static HCLColour InterpolateWeighted(IList<HCLColour> colours, IList<float> weights)
        {
            float totalWeight = 0f;

            for (int i = 0; i < weights.Count; i++)
            {
                totalWeight += Mathf.Clamp01(weights[i]);
            }

            totalWeight = 1f / totalWeight;

            float l = 0;
            float c = 0;
            float h = 0;
            float a = 0;

            for (int i = 0; i < colours.Count; i++)
            {
                l += colours[i]._l * totalWeight;
                c += colours[i]._c * totalWeight;
                h += (colours[i]._h - (((float)Math.Floor(colours[i]._h / 360f)) * 360f)) * totalWeight;
                a += colours[i]._a * totalWeight;
            }

            return new HCLColour(h, c, l, a);
        }

    }
}
