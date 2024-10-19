using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public static class ColourExtensions
    {


        public static float GetHDRIntensity(this Color c)
        {
            if (c.maxColorComponent == 0) return 0f;
            return Mathf.Log(255f / (191f / c.maxColorComponent)) / Mathf.Log(2f);
        }

        public static Color WithHDRIntensity(this Color c, float intensity)
        {
            return c * Mathf.Pow(2.0f, intensity);
            //   return c * Mathf.Pow(2.0f, intensity - 0.4169f);
        }

        public static float GetLuminance(this Color c)
        {
            return Mathf.Sqrt((c.r * c.r * 0.241f) + (c.g * c.g * 0.691f) + (c.b * c.b * 0.068f));
        }

        /// <summary>
        /// Returns the hue value of the colour.
        /// </summary>
        /// <returns>The hue value (normalised)</returns>
        public static float GetHue(this Color c)
        {
            return new HSVColour(c).H;
        }

        /// <summary>
        /// Returns the saturation of the colour.
        /// </summary>
        /// <returns>The saturation value (normalised)</returns>
        public static float GetSaturation(this Color c)
        {
            return new HSVColour(c).S;
        }

        /// <summary>
        /// Returns the brightness of the colour.
        /// </summary>
        /// <returns>The colour's value (normalised)</returns>
        public static float GetValue(this Color c)
        {
            return new HSVColour(c).V;
        }

        /// <summary>
        /// Returns a duplicate of the colour block.
        /// </summary>
        /// <returns>Duplicate of the existing colour block</returns>
        public static ColorBlock WithDefaultValues(this ColorBlock block)
        {
            ColorBlock newBlock = new ColorBlock();
            newBlock.colorMultiplier = 1f;
            newBlock.fadeDuration = 0.1f;
            newBlock.highlightedColor = Color.white;
            newBlock.normalColor = Color.white;
            newBlock.pressedColor = Color.white;
            newBlock.disabledColor = Color.white;
            return newBlock;
        }

        /// <summary>
        /// Returns a duplicate of the colour block.
        /// </summary>
        /// <returns>Duplicate of the existing colour block</returns>
        public static ColorBlock Duplicated(this ColorBlock block)
        {
            ColorBlock newBlock = new ColorBlock();
            newBlock.colorMultiplier = block.colorMultiplier;
            newBlock.fadeDuration = block.fadeDuration;
            newBlock.highlightedColor = block.highlightedColor;
            newBlock.normalColor = block.normalColor;
            newBlock.pressedColor = block.pressedColor;
            newBlock.disabledColor = block.disabledColor;
            return newBlock;
        }

        /// <summary>
        /// Returns a duplicate of the colour block, but with each colour multiplied blended with an input colour.
        /// </summary>
        /// <param name="colour">The colour to multiply by</param>
        /// <returns>The new blended colour block</returns>
        public static ColorBlock Mulitplied(this ColorBlock block, Color colour)
        {
            ColorBlock newBlock = new ColorBlock();
            newBlock.colorMultiplier = block.colorMultiplier;
            newBlock.fadeDuration = block.fadeDuration;
            newBlock.highlightedColor = block.highlightedColor * colour;
            newBlock.normalColor = block.normalColor * colour;
            newBlock.pressedColor = block.pressedColor * colour;
            newBlock.disabledColor = block.disabledColor * colour;
            return newBlock;
        }

        /// <summary>
        /// Returns a duplicate of the colour block, but with each colour shifted on any of the hue, saturation or value axes.
        /// </summary>
        /// <param name="hueShiftAmount">The amount to hue shift (normalized)</param>
        /// <param name="saturationShiftAmount">The amount to saturation shift (normalized)</param>
        /// <param name="valueShiftAmount">The amount to value shift (normalized)</param>
        /// <returns>The new shifted colur block</returns>
        public static ColorBlock Shifted(this ColorBlock block, float hueShiftAmount, float saturationShiftAmount, float valueShiftAmount)
        {
            ColorBlock newBlock = new ColorBlock();
            newBlock.colorMultiplier = block.colorMultiplier;
            newBlock.fadeDuration = block.fadeDuration;
            newBlock.highlightedColor = block.highlightedColor.WithHSVShift(hueShiftAmount, saturationShiftAmount, valueShiftAmount);
            newBlock.normalColor = block.normalColor.WithHSVShift(hueShiftAmount, saturationShiftAmount, valueShiftAmount);
            newBlock.pressedColor = block.pressedColor.WithHSVShift(hueShiftAmount, saturationShiftAmount, valueShiftAmount);
            newBlock.disabledColor = block.disabledColor.WithHSVShift(hueShiftAmount, saturationShiftAmount, valueShiftAmount);
            return newBlock;
        }

        /// <summary>
        /// Converts this colour to a HSV colour.
        /// </summary>
        /// <returns>The HSV colour</returns>
        public static HSVColour ToHSV(this Color colour)
        {
            return new HSVColour(colour);
        }

        /// <summary>
        /// Converts this colour to a HCL colour.
        /// </summary>
        /// <returns>The HCL colour</returns>
        public static HCLColour ToHCL(this Color colour)
        {
            return new HCLColour(colour);
        }

        /// <summary>
        /// Converts this colour to a XYZ colour.
        /// </summary>
        /// <returns>The XYZ colour</returns>
        public static XYZColour ToXYZ(this Color colour)
        {
            return new XYZColour(colour);
        }

        /// <summary>
        /// Converts this colour to a LAB colour.
        /// </summary>
        /// <returns>The LAB colour</returns>
        public static LABColour ToLAB(this Color colour)
        {
            return new LABColour(colour);
        }

        /// <summary>
        /// Gets the hex string of this colour.
        /// </summary>
        /// <returns>The colour's hex string</returns>
        public static string ToHex(this Color colour)
        {
            return ColorUtility.ToHtmlStringRGBA(colour);
        }

        public static int ToInt(this Color colour)
        {
            Color32 color32 = colour;
            return ((color32.r & 0xff) | ((color32.g & 0xff) << 8) | ((color32.b & 0xff) << 16) | ((color32.a & 0xff) << 24));
        }

        /// <summary>
        /// Returns this colour, but shifted on any of the hue, saturation or value axes.
        /// </summary>
        /// <param name="hueShiftAmount">The amount to hue shift (normalized)</param>
        /// <param name="saturationShiftAmount">The amount to saturation shift (normalized)</param>
        /// <param name="valueShiftAmount">The amount to value shift (normalized)</param>
        /// <returns>The new shifted colour</returns>
        public static Color WithHSVShift(this Color c, float hueShiftAmount, float saturationShiftAmount, float valueShiftAmount)
        {
            HSVColour hsv = new HSVColour(c);
            hsv.H += hueShiftAmount;
            hsv.S += saturationShiftAmount;
            hsv.V += valueShiftAmount;
            return hsv.ToRGB();
        }

        /// <summary>
        /// Returns this colour, but hue shifted by a certain amount.
        /// </summary>
        /// <param name="hueShiftAmount">The amount to hue shift (normalized)</param>
        /// <returns>The new hue shifted colour</returns>
        public static Color WithHueShift(this Color c, float hueShiftAmount)
        {
            HSVColour hsv = new HSVColour(c);
            hsv.H += hueShiftAmount;
            return hsv.ToRGB();
        }

        /// <summary>
        /// Returns this colour, but the saturation shifted by a certain amount.
        /// </summary>
        /// <param name="saturationShiftAmount">The amount to saturation shift (normalized)</param>
        /// <returns>The new saturation shifted colour</returns>
        public static Color WithSaturationShift(this Color c, float saturationShiftAmount)
        {
            HSVColour hsv = new HSVColour(c);
            hsv.S += saturationShiftAmount;
            return hsv.ToRGB();
        }

        /// <summary>
        /// Returns this colour, but with the brightness shifted by a certain amount.
        /// </summary>
        /// <param name="valueShiftAmount">The amount to value shift (normalized)</param>
        /// <returns>The new value shifted colour</returns>
        public static Color WithValueShift(this Color c, float valueShiftAmount)
        {
            HSVColour hsv = new HSVColour(c);
            hsv.V += valueShiftAmount;
            return hsv.ToRGB();
        }

        /// <summary>
        /// Returns this colour, but with a different hue.
        /// </summary>
        /// <param name="hue">The value of the tone (normalized)</param>
        /// <returns>The new colour</returns>
        public static Color WithHue(this Color c, float hue)
        {
            HSVColour hsv = new HSVColour(c);
            hsv.H = hue;
            return hsv.ToRGB();
        }

        /// <summary>
        /// Returns this colour, but with a specific saturation level.
        /// </summary>
        /// <param name="saturation">The saturation of the colour (normalized)</param>
        /// <returns>The new colour</returns>
        public static Color WithSaturation(this Color c, float saturation)
        {
            HSVColour hsv = new HSVColour(c);
            hsv.S = saturation;
            return hsv.ToRGB();
        }

        /// <summary>
        /// Returns this colour, but with a specific brightness.
        /// </summary>
        /// <param name="value">The value of the colour (normalized)</param>
        /// <returns>The new colour</returns>
        public static Color WithValue(this Color c, float value)
        {
            HSVColour hsv = new HSVColour(c);
            hsv.V = value;
            return hsv.ToRGB();
        }

        /// <summary>
        /// Returns this colour, but with a different alpha value.
        /// </summary>
        /// <param name="alpha">The desired alpha value</param>
        /// <returns>The "new" colour</returns>
        public static Color WithAlpha(this Color c, float alpha)
        {
            return new Color(c.r, c.g, c.b, alpha);
        }

        /// <summary>
        /// Returns this colour, but with a different red value.
        /// </summary>
        /// <param name="alpha">The desired red value</param>
        /// <returns>The "new" colour</returns>
        public static Color WithRed(this Color c, float red)
        {
            return new Color(red, c.g, c.b, c.a);
        }

        /// <summary>
        /// Returns this colour, but with a different green value.
        /// </summary>
        /// <param name="alpha">The desired green value</param>
        /// <returns>The "new" colour</returns>
        public static Color WithGreen(this Color c, float green)
        {
            return new Color(c.r, green, c.b, c.a);
        }

        /// <summary>
        /// Returns the colour, but with a different blue value.
        /// </summary>
        /// <param name="alpha">The desired blue value</param>
        /// <returns>The "new" colour</returns>
        public static Color WithBlue(this Color c, float blue)
        {
            return new Color(c.r, c.g, blue, c.a);
        }

        /// <summary>
        /// Returns the colour, but inverted.
        /// </summary>
        /// <param name="invertAlpha">Whether or not to also invert the colour's alpha</param>
        /// <returns>The inverted colour</returns>
        public static Color Inverted(this Color c, bool invertAlpha = false)
        {
            return new Color(1 - c.r, 1 - c.g, 1 - c.b, invertAlpha ? 1 - c.a : c.a);
        }



    }
}
