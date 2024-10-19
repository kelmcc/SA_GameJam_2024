using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public struct FloatTimeScalePair
    {
        public float FloatValue => _floatValue;
        public bool UseUnscaledTime => _isUnscaled;

        [SerializeField]
        private float _floatValue;

        [SerializeField]
        private bool _isUnscaled;

        public FloatTimeScalePair(float value, bool useUnscaledTime)
        {
            _floatValue = value;
            _isUnscaled = useUnscaledTime;
        }

        public static implicit operator float(FloatTimeScalePair f) => f._floatValue;
        public static explicit operator FloatTimeScalePair(float f) => new FloatTimeScalePair(f, false);
    }
}
