using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Framework
{
    /// <summary>
    /// Data structure representing the set of integers between a lower bound and an upper bound.
    /// </summary>
    [Serializable]
    public struct IntRange
    {
        public float AverageValue => (_min + _max) * 0.5f;

        /// <summary>
        /// The lower bound.
        /// </summary>
        public int Min
        {
            get => _min;
            set
            {
                Assert.IsTrue(value <= _max, "Invalid range bounds");
                _min = value;
                _chosenValue = null;
            }
        }

        /// <summary>
        /// The upper bound.
        /// </summary>
        public int Max
        {
            get => _max;
            set
            {
                Assert.IsTrue(_min <= value, "Invalid range bounds");
                _max = value;
                _chosenValue = null;
            }
        }

        /// <summary>
        /// A random value between the bounds that is always the same, unless the bounds are changed.
        /// </summary>
        public int ChosenValue
        {
            get { if (_chosenValue == null) _chosenValue = Random.Range(_min, _max + 1); return _chosenValue.Value; }
        }

        /// <summary>
        /// The difference between the upper and lower bounds.
        /// </summary>
        public int Length => _max - _min;

        [SerializeField, Delayed]
        private int _min;
        [SerializeField, Delayed]
        private int _max;

        private int? _chosenValue;

        /// <summary>
        /// Creates a new IntRange.
        /// </summary>
        /// <param name="min">The lower bound</param>
        /// <param name="max">The upper bound</param>
        public IntRange(int min, int max)
        {
            Assert.IsTrue(min <= max, "Invalid range bounds");
            _min = min;
            _max = max;
            _chosenValue = null;
        }

        /// <summary>
        /// Checks whether an integer is between the range's bounds.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is between the bounds</returns>
        public bool InRange(int value)
        {
            return value >= _min && value <= _max;
        }

        /// <summary>
        /// Checks whether a float is between the range's bounds.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is between the bounds</returns>
        public bool InRange(float value)
        {
            return value >= _min && value <= _max;
        }

        /// <summary>
        /// Maps a proportion to a value in the range.
        /// </summary>
        /// <param name="proportion">A normalized value to be mapped onto the range</param>
        /// <returns>The nearest value at the given proportion in the range</returns>
        public int GetValue(float proportion)
        {
            return Mathf.RoundToInt(Mathf.Lerp(_min, _max, proportion));
        }

        /// <summary>
        /// Checks how close a value is to the end of the range.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>A normalized float representing the value's position in the range</returns>
        public float GetProportion(float value)
        {
            return Mathf.InverseLerp(_min, _max, value);
        }

        /// <summary>
        /// Checks how close a value is to the end of the range.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>A normalized float representing the value's position in the range</returns>
        public float GetProportion(int value)
        {
            return Mathf.InverseLerp(_min, _max, value);
        }

        /// <summary>
        /// Clamps an integer to the range's bounds.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <returns>The clamped value</returns>
        public int Clamp(int value)
        {
            return Mathf.Clamp(value, _min, _max);
        }

        /// <summary>
        /// Clamps a float to the range's bounds.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <returns>The clamped value</returns>
        public int Clamp(float value)
        {
            return Mathf.FloorToInt(Mathf.Clamp(value, _min, _max));
        }

        public float Wrap(float value)
        {
            int distance = _max - _min + 1;
            if (value < _min) value += distance * ((_min - value) / distance + 1);

            return _min + (value - _min) % distance;
        }


        /// <summary>
        /// Test whether an float falls within the range's bounds (inclusive)
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>Whether the value falls within the range</returns>
        public bool Contains(float value)
        {
            return value >= _min && value <= _max;
        }

        /// <summary>
        /// Chooses a random value that is between the range's bounds.
        /// </summary>
        /// <returns>An integer between the range's bounds</returns>
        public int ChooseRandom()
        {
            return Random.Range(_min, _max + 1);
        }


        public List<int> GetValues(bool inclusive = true)
        {
            int min = inclusive ? _min : _min + 1;
            int max = inclusive ? _max + 1 : _max;

            List<int> values = new List<int>(_max - _min);
            for (int i = min; i < max; i++)
            {
                values.Add(i);
            }

            return values;
        }


    }


    /// <summary>
    /// Data structure representing the set of floats between a lower bound and an upper bound.
    /// </summary>
    [Serializable]
    public struct FloatRange
    {
        public float AverageValue => (_min + _max) * 0.5f;

        /// <summary>
        /// The lower bound.
        /// </summary>
        public float Min
        {
            get => _min;
            set
            {
                Assert.IsTrue(value <= _max, "Invalid range bounds");
                _min = value;
                _chosenValue = null;
            }
        }

        /// <summary>
        /// The upper bound.
        /// </summary>
        public float Max
        {
            get => _max;
            set
            {
                Assert.IsTrue(_min <= value, "Invalid range bounds");
                _max = value;
                _chosenValue = null;
            }
        }

        /// <summary>
        /// A random value between the bounds that is always the same, unless the bounds are changed.
        /// </summary>
        public float ChosenValue
        {
            get { if (_chosenValue == null) _chosenValue = Random.Range(_min, _max); return _chosenValue.Value; }
        }

        /// <summary>
        /// The difference between the upper and lower bounds.
        /// </summary>
        public float Length => _max - _min;

        [SerializeField, Delayed]
        private float _min;
        [SerializeField, Delayed]
        private float _max;

        private float? _chosenValue;

        /// <summary>
        /// Creates a new FloatRange.
        /// </summary>
        /// <param name="min">The lower bound</param>
        /// <param name="max">The upper bound</param>
        public FloatRange(float min, float max)
        {
            Assert.IsTrue(min <= max, "Invalid range bounds");
            _min = min;
            _max = max;
            _chosenValue = null;
        }

        /// <summary>
        /// Checks whether a float is between the range's bounds.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is between the bounds</returns>
        public bool InRange(float value)
        {
            return value >= _min && value <= _max;
        }


        /// <summary>
        /// Checks how close a value is to the end of the range.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>A normalized float representing the value's position in the range</returns>
        public float GetProportion(float value)
        {
            return Mathf.InverseLerp(_min, _max, value);
        }

        /// <summary>
        /// Maps a proportion to a value in the range.
        /// </summary>
        /// <param name="proportion">A normalized value to be mapped onto the range</param>
        /// <returns>The value at the given proportion in the range</returns>
        public float GetValue(float proportion)
        {
            return Mathf.Lerp(_min, _max, proportion);
        }

        /// <summary>
        /// Maps a proportion to a value in the range.
        /// </summary>
        /// <param name="t">A normalized value to be mapped onto the range</param>
        /// <returns>The value at the given proportion in the range</returns>
        public float Lerp(float t)
        {
            return Mathf.Lerp(_min, _max, t);
        }

        /// <summary>
        /// Clamps an integer to the range's bounds. Will round to the nearest integer in the range if necessary. 
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <returns>The clamped value</returns>
        public int Clamp(int value)
        {
            return (int)Mathf.Clamp(value, _min, _max);
        }

        /// <summary>
        /// Test whether an float falls within the range's bounds (inclusive)
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>Whether the value falls within the range</returns>
        public bool Contains(float value)
        {
            return value >= _min && value <= _max;
        }

        /// <summary>
        /// Test whether an integer falls within the range's bounds (inclusive)
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>Whether the value falls within the range</returns>
        public bool Contains(int value)
        {
            return value >= _min && value <= _max;
        }

        /// <summary>
        /// Clamps a float to the range's bounds.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <returns>The clamped value</returns>
        public float Clamp(float value)
        {
            return Mathf.Clamp(value, _min, _max);
        }

        public float Wrap(float value)
        {
            float distance = _max - _min;
            float times = (float)Math.Floor((value - _min) / distance);

            return value - (times * distance);
        }

        public float PingPong(float value)
        {
            return _min + Mathf.PingPong(value, _max - _min);
        }

        /// <summary>
        /// Chooses a random value that is between the range's bounds.
        /// </summary>
        /// <returns>A float between the range's bounds</returns>
        public float ChooseRandom()
        {
            return Random.Range(_min, _max);
        }


    }
}