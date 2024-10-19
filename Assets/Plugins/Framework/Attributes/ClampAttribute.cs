using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Forces an int, float or range field to be clamped between two values.
    /// </summary>
    public class ClampAttribute : PropertyAttribute
    {

        public float MinFloat => _min;
        public float MaxFloat => _max;
        public int MinInt => GetInt(_min);
        public int MaxInt => GetInt(_max);

        private float _min = 0f;
        private float _max = 1f;

        int GetInt(float value)
        {
            if (value == Mathf.Infinity) return int.MaxValue;
            if (value == Mathf.NegativeInfinity) return int.MinValue;

            return Mathf.RoundToInt(value);
        }

        /// <summary>
        /// Forces an int, float or range field to be clamped between zero and some maximum. 
        /// </summary>
        /// <param name="_max">The maximum value</param>
        public ClampAttribute()
        {
            _min = 0f;
            _max = 1f;
        }

        /// <summary>
        /// Forces an int, float or range field to be clamped between two values.
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        public ClampAttribute(float min, float max)
        {
            _min = min;
            _max = max;
        }

    }
}
