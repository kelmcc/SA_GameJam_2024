using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public struct ColourPair
    {
        public Color First { get => _first; set => _first = value; }
        public Color Second { get => _second; set => _second = value; }

        [SerializeField]
        private Color _first;

        [SerializeField]
        private Color _second;

        public ColourPair(Color firstColour, Color secondColour)
        {
            _first = firstColour;
            _second = secondColour;
        }

        public Color Lerp(float t)
        {
            return Color.Lerp(_first, _second, t);
        }

        public Color LerpHSV(float t)
        {
            return HSVColour.Lerp(new HSVColour(_first), new HSVColour(_second), t).ToRGB();
        }

        public Color LerpHCL(float t)
        {
            return HCLColour.Lerp(new HCLColour(_first), new HCLColour(_second), t).ToRGB();
        }
    }
}
