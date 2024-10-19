using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework
{
    /// <summary>
    /// A data structure for random sampling from a set of N choices with a guarantee the every element will be slected at least once every N samples.
    /// </summary>
    /// <typeparam name="T">The type of the choices</typeparam>
    public class ShuffleBag<T>
    {
        /// <summary>
        /// The number of total choices in the GrabBag.
        /// </summary>
        public int NumChoices => _elements.Count;
        public List<T> Choices => _elements;

        protected List<T> _elements;
        protected int _cursor = -1;

        /// <summary>
        /// Creates a new ShuffleBag from a params array of choices.
        /// </summary>
        /// <param name="choices">The choices in the bag</param>
        public ShuffleBag(params T[] choices)
        {
            _elements = new List<T>(choices);
        }

        public ShuffleBag()
        {
            _elements = new List<T>();
        }

        /// <summary>
        /// Creates a new ShuffleBag from a IList of choices.
        /// </summary>
        /// <param name="choices">The choices in the bag</param>
        public ShuffleBag(IList<T> choices)
        {
            _elements = new List<T>(choices);
        }

        /// <summary>
        /// Gets the next random sample from the ShuffleBag.
        /// </summary>
        /// <returns>A random element from the bag</returns>
        public T GetNext()
        {
            if (_elements.Count == 0) throw new UnityException("ShuffleBag has no choices!");
            if (_elements.Count == 1) return _elements[0];

            if (_cursor == 0)
            {
                _cursor = _elements.Count - 1;
                return _elements[0];
            }

            int grabIndex;

            if (_cursor == -1) // First ever pick
            {
                grabIndex = Mathf.FloorToInt(Random.value * _elements.Count);
                _cursor = _elements.Count - 1;
            }
            else
            {
                grabIndex = _cursor == _elements.Count - 1 ? 1 + Mathf.FloorToInt(Random.value * _cursor) : Mathf.FloorToInt(Random.value * (_cursor + 1));

            }

            T temp = _elements[grabIndex];
            _elements[grabIndex] = _elements[_cursor];
            _elements[_cursor] = temp;

            _cursor--;

            return temp;
        }

        /// <summary>
        /// Adds a new item to the possible pool in a random position.
        /// </summary>
        /// <param name="item">The new element to add</param>
        public void Add(T item)
        {
            _elements.Insert(Random.Range(0, _elements.Count), item);
        }

        /// <summary>
        /// Adds multiple instances of an item to the possible pool in random positions.
        /// </summary>
        /// <param name="item">The new element to add</param>
        /// <param name="count">The number of times to add</param>
        public void Add(T item, int count)
        {
            for (int i = 0; i < count; i++)
            {
                _elements.Insert(Random.Range(0, _elements.Count), item);
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                _elements.Insert(Random.Range(0, _elements.Count), item);
            }
        }


    }

    /// <summary>
    /// A ShuffleBag with a range of integers as possible choices.
    /// </summary>
    public class ShuffleBag : ShuffleBag<int>
    {
        /// <summary>
        /// Creates a new int range ShuffleBag.
        /// </summary>
        /// <param name="min">The minimum possible value that can be selected (inclusive)</param>
        /// <param name="max">The maximum possible value that can be selected (inclusive)</param>
        public ShuffleBag(int min, int max)
        {
            _elements = new List<int>();
            for (int i = 0; i < max - min + 1; i++)
            {
                _elements.Add(min + i);
            }
        }
    }

    /// <summary>
    /// A ShuffleBag with enum values as possible choices.
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    public class EnumShuffleBag<T> : ShuffleBag<T> where T : struct, IComparable, IConvertible, IFormattable
    {

        /// <summary>
        /// Creates a new ShuffleBag with all possible enum values.
        /// </summary>
        public EnumShuffleBag()
        {
            DebugUtils.AssertIsEnumType<T>();

            _elements = new List<T>((T[])Enum.GetValues(typeof(T)));
        }

        /// <summary>
        /// Creates a new ShuffleBag with some enum values excluded.
        /// </summary>
        /// <param name="excludedValues">The excluded values, these are not put into the bag</param>
        public EnumShuffleBag(params T[] excludedValues)
        {
            DebugUtils.AssertIsEnumType<T>();

            _elements = new List<T>();

            T[] values = (T[])Enum.GetValues(typeof(T));
            for (int i = 0; i < values.Length; i++)
            {
                bool isExcluded = false;
                for (int j = 0; j < excludedValues.Length; j++)
                {
                    if (excludedValues[j].Equals(values[i]))
                    {
                        isExcluded = true;
                        break;
                    }
                }

                if (!isExcluded)
                {
                    _elements.Add(values[i]);
                }
            }

        }
    }
}