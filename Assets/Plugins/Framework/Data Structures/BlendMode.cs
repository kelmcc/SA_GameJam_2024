using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace Framework
{

    public enum BlendMode
    {
        Darken,
        Multiply,
        ColourBurn,
        LinearBurn,
        DarkerColour,

        Lighten,
        Screen,
        ColourDodge,
        LinearDodge,
        LighterColour,

        Overlay,
        SoftLight,
        HardLight,
        VividLight,
        LinearLight,
        PinLight,
        HardMix,

        Difference,
        Exclusion,
        Subtract,
        Divide,

        Hue,
        Colour,
        Saturation,
        Luminosity
    }

    public static class BlendModeExtensions
    {
        public static Color Blend(this BlendMode blendMode, Color source, Color destination)
        {
            Vector3 s = new Vector3(source.r, source.g, source.b);
            Vector3 d = new Vector3(destination.r, destination.g, destination.b);
            Vector3 c = Blend(blendMode, s, d);

            return new Color(c.x, c.y, c.z);
        }

        static Vector3 Blend(BlendMode blendMode, Vector3 s, Vector3 d)
        {
            switch (blendMode)
            {
                case BlendMode.Darken: return Darken(s, d);
                case BlendMode.Multiply: return Multiply(s, d);
                case BlendMode.ColourBurn: return ColourBurn(s, d);
                case BlendMode.LinearBurn: return LinearBurn(s, d);
                case BlendMode.DarkerColour: return DarkerColour(s, d);
                case BlendMode.Lighten: return Lighten(s, d);
                case BlendMode.Screen: return Screen(s, d);
                case BlendMode.ColourDodge: return ColourDodge(s, d);
                case BlendMode.LinearDodge: return LinearDodge(s, d);
                case BlendMode.LighterColour: return LighterColour(s, d);
                case BlendMode.Overlay: return Overlay(s, d);
                case BlendMode.SoftLight: return SoftLight(s, d);
                case BlendMode.HardLight: return HardLight(s, d);
                case BlendMode.VividLight: return VividLight(s, d);
                case BlendMode.LinearLight: return LinearLight(s, d);
                case BlendMode.PinLight: return PinLight(s, d);
                case BlendMode.HardMix: return HardMix(s, d);
                case BlendMode.Difference: return Difference(s, d);
                case BlendMode.Exclusion: return Exclusion(s, d);
                case BlendMode.Subtract: return Subtract(s, d);
                case BlendMode.Divide: return Divide(s, d);
                case BlendMode.Hue: return Hue(s, d);
                case BlendMode.Colour: return Colour(s, d);
                case BlendMode.Saturation: return Saturation(s, d);
                case BlendMode.Luminosity: return Luminosity(s, d);
            }

            throw new ArgumentOutOfRangeException(nameof(blendMode), blendMode, null);
        }

        static Vector3 Darken(Vector3 s, Vector3 d)
        {
            return Vector3.Min(s, d);
        }

        static Vector3 Multiply(Vector3 s, Vector3 d)
        {
            return s.MultiplyComponentWise(d);
        }

        static Vector3 ColourBurn(Vector3 s, Vector3 d)
        {
            return Vector3.one - (Vector3.one - d).DivideComponentWise(s);
        }

        static Vector3 LinearBurn(Vector3 s, Vector3 d)
        {
            return s + d - Vector3.one;
        }

        static Vector3 DarkerColour(Vector3 s, Vector3 d)
        {
            return (s.x + s.y + s.z < d.x + d.y + d.z) ? s : d;
        }

        static Vector3 Lighten(Vector3 s, Vector3 d)
        {
            return Vector3.Max(s, d);
        }

        static Vector3 Screen(Vector3 s, Vector3 d)
        {
            return s + d - s.MultiplyComponentWise(d);
        }

        static Vector3 ColourDodge(Vector3 s, Vector3 d)
        {
            return d.DivideComponentWise(Vector3.one - s);
        }

        static Vector3 LinearDodge(Vector3 s, Vector3 d)
        {
            return s + d;
        }

        static Vector3 LighterColour(Vector3 s, Vector3 d)
        {
            return (s.x + s.y + s.z > d.x + d.y + d.z) ? s : d;
        }


        static Vector3 Overlay(Vector3 s, Vector3 d)
        {
            float Overlay(float a, float b)
            {
                return (b < 0.5f) ? 2f * a * b : 1f - 2f * (1f - a) * (1f - b);
            }

            return new Vector3(Overlay(s.x, d.x), Overlay(s.y, d.y), Overlay(s.z, d.z));
        }



        static Vector3 SoftLight(Vector3 s, Vector3 d)
        {
            float SoftLight(float a, float b)
            {
                return (a < 0.5f) ? b - (1f - 2f * a) * b * (1f - b) : (b < 0.25f) ? b + (2f * a - 1f) * b * ((16f * b - 12f) * b + 3f) : b + (2f * a - 1f) * (Mathf.Sqrt(b) - b);
            }

            return new Vector3(SoftLight(s.x, d.x), SoftLight(s.y, d.y), SoftLight(s.z, d.z));
        }

        static Vector3 HardLight(Vector3 s, Vector3 d)
        {
            float HardLight(float a, float b)
            {
                return (a < 0.5f) ? 2f * a * b : 1f - 2f * (1f - a) * (1f - b);
            }

            return new Vector3(HardLight(s.x, d.x), HardLight(s.y, d.y), HardLight(s.z, d.z));
        }

        static Vector3 VividLight(Vector3 s, Vector3 d)
        {
            float VividLight(float a, float b)
            {
                return (a < 0.5f) ? 1f - (1f - b) / (2f * a) : b / (2f * (1f - a));
            }

            return new Vector3(VividLight(s.x, d.x), VividLight(s.y, d.y), VividLight(s.z, d.z));
        }

        static Vector3 LinearLight(Vector3 s, Vector3 d)
        {
            return 2f * s + d.SubtractConstant(1f);
        }

        static Vector3 PinLight(Vector3 s, Vector3 d)
        {
            float PinLight(float a, float b)
            {
                return (2f * a - 1f > b) ? 2f * a - 1f : (a < 0.5 * b) ? 2f * a : b;
            }

            return new Vector3(PinLight(s.x, d.x), PinLight(s.y, d.y), PinLight(s.z, d.z));
        }

        static Vector3 HardMix(Vector3 s, Vector3 d)
        {
            return MathUtils.Floor(s + d);
        }

        static Vector3 Difference(Vector3 s, Vector3 d)
        {
            return MathUtils.Abs(d - s);
        }

        static Vector3 Exclusion(Vector3 s, Vector3 d)
        {
            return s + d - 2f * s.MultiplyComponentWise(d);
        }

        static Vector3 Subtract(Vector3 s, Vector3 d)
        {
            return s - d;
        }

        static Vector3 Divide(Vector3 s, Vector3 d)
        {
            return s.DivideComponentWise(d);
        }

        static Vector3 Hue(Vector3 s, Vector3 d)
        {
            d = ToHSV(d);
            d.x = ToHSV(s).x;
            return ToRGB(d);
        }

        static Vector3 Colour(Vector3 s, Vector3 d)
        {
            s = ToHSV(s);
            s.z = ToHSV(d).z;
            return ToRGB(s);
        }

        static Vector3 Saturation(Vector3 s, Vector3 d)
        {
            d = ToHSV(d);
            d.y = ToHSV(s).y;
            return ToRGB(d);
        }

        static Vector3 Luminosity(Vector3 s, Vector3 d)
        {
            float dLum = Vector3.Dot(d, new Vector3(0.3f, 0.59f, 0.11f));
            float sLum = Vector3.Dot(s, new Vector3(0.3f, 0.59f, 0.11f));
            Vector3 c = d.AddConstant(sLum - dLum);

            float minC = Mathf.Min(Mathf.Min(c.x, c.y), c.z);
            float maxC = Mathf.Max(Mathf.Max(c.x, c.y), c.z);

            if (minC < 0f) return (c.SubtractConstant(sLum) * sLum / (sLum - minC)).AddConstant(sLum);
            if (maxC > 1f) return (c.SubtractConstant(sLum) * (1f - sLum) / (maxC - sLum)).AddConstant(sLum);

            return c;
        }

        static Vector3 ToHSV(Vector3 c)
        {
            HSVColour colour = new Color(c.x, c.y, c.z).ToHSV();
            return new Vector3(colour.H, colour.S, colour.V);
        }

        static Vector3 ToRGB(Vector3 c)
        {
            Color colour = new HSVColour(c.x, c.y, c.z).ToRGB();
            return new Vector3(colour.r, colour.g, colour.b);
        }
    }

}