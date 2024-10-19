using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Framework
{
    /// <summary>
    /// A queue in which elements are always sorted. Uses a binary heap implementation.
    /// </summary>
    /// <typeparam name="T">The type of element to store in the queue</typeparam>
    public class PriorityQueue<T>
    {

        protected List<T> _elements = new List<T>();
        protected IComparer<T> _comparer;
        protected Comparison<T> _comparsionFunction;

        /// <summary>
        /// The number of elements in the queue.
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// Whether or not the queue is read only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Creates a queue that is always sorted by comparing elements using the Type's default Comparer.
        /// </summary>
        public PriorityQueue()
        {
            _comparer = Comparer<T>.Default;
        }

        /// <summary>
        /// Creates a queue that is always sorted by comparing elements using a specific Comparer.
        /// </summary>
        /// <param name="comparer">The comparer to sort with</param>
        public PriorityQueue(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        /// <summary>
        /// Creates a queue that is always sorted by comparing elements using a comparison function.
        /// </summary>
        /// <param name="comparisonFunction">The comparison function to sort with</param>
        public PriorityQueue(Comparison<T> comparisonFunction)
        {
            _comparsionFunction = comparisonFunction;
        }

        /// <summary>
        /// Adds a new item to the queue, inserting it in its corret position.
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Enqueue(T item)
        {

            _elements.Add(item);
            int ci = _elements.Count - 1; // child index; start at end
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // parent index
                if (Compare(_elements[ci], _elements[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
                T tmp = _elements[ci]; _elements[ci] = _elements[pi]; _elements[pi] = tmp;
                ci = pi;
            }

        }

        /// <summary>
        /// Removes and returns the head of the queue.
        /// </summary>
        /// <returns>The first item from the queue</returns>
        public T Dequeue()
        {
            Assert.IsTrue(_elements.Count > 0, "PriorityQueue is empty.");

            int li = _elements.Count - 1; // last index (before removal)
            T frontItem = _elements[0];   // fetch the front
            _elements[0] = _elements[li];
            _elements.RemoveAt(li);

            --li; // last index (after removal)
            int pi = 0; // parent index. start at front of pq
            while (true)
            {
                int ci = pi * 2 + 1; // left child index of parent
                if (ci > li) break;  // no children so done
                int rc = ci + 1;     // right child
                if (rc <= li && Compare(_elements[rc], _elements[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                    ci = rc;
                if (Compare(_elements[pi], _elements[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
                T tmp = _elements[pi]; _elements[pi] = _elements[ci]; _elements[ci] = tmp; // swap parent and child
                pi = ci;
            }
            return frontItem;
        }

        /// <summary>
        /// Clears all the elements in the queue.
        /// </summary>
        public void Clear()
        {
            _elements.Clear();
        }

        /// <summary>
        /// Returns but does not remove the head of the queue.
        /// </summary>
        /// <returns>The first item from the queue</returns>
        public T Peek()
        {
            Assert.IsTrue(_elements.Count > 0, "PriorityQueue is empty.");
            return _elements[0];
        }

        /// <summary>
        /// Checks whether the queue contains an item.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>Whether or not the queue contains the item</returns>
        public bool Contains(T item)
        {
            int lower = 0;
            int upper = _elements.Count - 1;

            while (lower <= upper)
            {
                int middle = lower + (upper - lower) / 2;
                int comparisonResult = Compare(item, _elements[middle]);

                if (comparisonResult == 0)
                {
                    return item.Equals(_elements[middle]);
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

            return false;
        }

        /// <summary>
        /// Copies the elements from the queue to an array.
        /// </summary>
        /// <param name="array">The array to copy to</param>
        /// <param name="startIndex">The position in the array at which copying begins</param>
        public void CopyTo(T[] array, int startIndex)
        {
            _elements.CopyTo(array, startIndex);
        }

        int Compare(T a, T b)
        {
            return _comparer == null ? _comparsionFunction(a, b) : _comparer.Compare(a, b);
        }


    }
}

