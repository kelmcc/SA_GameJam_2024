using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Framework
{
    /// <summary>
    /// A list that is always sorted by using a comparer. Elements are inserted in their correct position using a binary search.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    public class SortedList<T> : ICollection<T>
    {

        /// <summary>
        /// The number of elements in the list.
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// Whether or not the list is read only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Finds the element stored at a specfic position in the list.
        /// </summary>
        /// <param name="index">The index to check</param>
        /// <returns>The element in the list at the specified index</returns>
        public T this[int index] => _elements[index];

        protected List<T> _elements = new List<T>();
        protected IComparer<T> _comparer;
        protected Comparison<T> _comparisonFunction;

        /// <summary>
        /// Creates a list that is always sorted by comparing elements using the Type's default Comparer.
        /// </summary>
        public SortedList()
        {
            _comparer = Comparer<T>.Default;
        }

        /// <summary>
        /// Creates a list that is always sorted by comparing elements using a specific Comparer.
        /// </summary>
        /// <param name="comparer">The comparer to sort with</param>
        public SortedList(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        /// <summary>
        /// Creates a list that is always sorted by comparing elements using a specific comparison function.
        /// </summary>
        /// <param name="comparer">The comparison function to sort with</param>
        public SortedList(Comparison<T> comparisonFunction)
        {
            _comparisonFunction = comparisonFunction;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <summary>
        /// Adds a new item to the list, inserting it in its corret position.
        /// </summary>
        /// <param name="item">The item to insert</param>
        public void Add(T item)
        {
            Insert(item);
        }

        /// <summary>
        /// Adds a new item to the list, inserting it in its corret position.
        /// </summary>
        /// <param name="item">The item to insert</param>
        public void Insert(T item)
        {
            int lower = 0;
            int upper = _elements.Count - 1;

            while (lower <= upper)
            {
                int middle = lower + (upper - lower) / 2;
                int comparisonResult = _comparer == null ? _comparisonFunction(item, _elements[middle]) : _comparer.Compare(item, _elements[middle]);

                if (comparisonResult == 0)
                {
                    _elements.Insert(middle, item);
                    return;
                }

                if (comparisonResult < 0)
                {
                    upper = middle - 1;
                }
                else
                {
                    lower = middle + 1;
                }
            }

            _elements.Insert(lower, item);
        }

        /// <summary>
        /// Ensures that all the elements in the list are still sorted.
        /// </summary>
        public void Resort()
        {
            if (_comparer == null)
            {
                _elements.Sort(_comparisonFunction);
            }
            else
            {
                _elements.Sort(_comparer);
            }
        }

        /// <summary>
        /// Clears all the elements in the list.
        /// </summary>
        public void Clear()
        {
            _elements.Clear();
        }

        /// <summary>
        /// Checks whether the list contains an item.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>Whether or not the list contains the item</returns>
        public bool Contains(T item)
        {
            return _elements.Contains(item);
        }

        /// <summary>
        /// Copies the elements from the list to an array.
        /// </summary>
        /// <param name="array">The array to copy to</param>
        /// <param name="startIndex">The position in the array at which copying begins</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Tries to remove an item from the list.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>Whether or not the item was successfully found and removed</returns>
        public bool Remove(T item)
        {
            return _elements.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _elements.RemoveAt(index);
        }

        public T[] ToArray()
        {
            return _elements.ToArray();
        }

        public void Enqueue(T item)
        {
            Insert(item);
        }

        public T Dequeue()
        {
            Assert.IsTrue(_elements.Count > 0);

            T element = _elements[0];
            _elements.RemoveAt(0);
            return element;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                if (item.Equals(_elements[i])) return i;
            }

            return -1;
        }
    }
}
