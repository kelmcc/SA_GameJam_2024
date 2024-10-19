using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Assertions;

namespace Framework
{
    /// <summary>
    /// A data structure that allows elements to be added and removed from either the head or the tail.
    /// </summary>
    /// <typeparam name="T">The type of the elements stored in this Dequeue</typeparam>
    public class DoubleEndedQueue<T> : IList<T>
    {

        private List<T> _elements = new List<T>();

        /// <summary>
        /// The number of elements in the queue.
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// Whether or not the queue is read-only. It is not.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Returns the item in the queue at a specfic index.
        /// </summary>
        /// <param name="index">The index to check</param>
        /// <returns>The item at the index</returns>
        public T this[int index]
        {
            get => _elements[index];
            set => _elements[index] = value;
        }

        /// <summary>
        /// Adds a new item to the front of the queue.
        /// </summary>
        /// <param name="item">The item</param>
        public void PushFront(T item)
        {
            _elements.Insert(0, item);
        }

        /// <summary>
        /// Adds a new item to the back of the queue/
        /// </summary>
        /// <param name="item">The item</param>
        public void PushBack(T item)
        {
            _elements.Add(item);
        }

        /// <summary>
        /// Returns, but does not remove the item at the front of the queue.
        /// </summary>
        /// <returns>The item at the front of the queue</returns>
        public T PeekFront()
        {
            Assert.IsTrue(_elements.Count > 0, "DoubleEndedQueue is empty.");
            return _elements[0];
        }

        /// <summary>
        /// Returns, but does not remove the item at the back of the queue.
        /// </summary>
        /// <returns>The item at the back of the queue</returns>
        public T PeekBack()
        {
            Assert.IsTrue(_elements.Count > 0, "DoubleEndedQueue is empty.");
            return _elements[_elements.Count - 1];
        }

        /// <summary>
        /// Removes and returns the item at the front of the queue.
        /// </summary>
        /// <returns>The item at the front of the queue</returns>
        public T PopFront()
        {
            Assert.IsTrue(_elements.Count > 0, "DoubleEndedQueue is empty.");
            T item = _elements[0];
            _elements.RemoveAt(0);
            return item;
        }

        /// <summary>
        /// Removes and returns the item at the back of the queue.
        /// </summary>
        /// <returns>The item at the back of the queue</returns>
        public T PopBack()
        {
            Assert.IsTrue(_elements.Count > 0, "DoubleEndedQueue is empty.");
            T item = _elements[_elements.Count - 1];
            _elements.RemoveAt(_elements.Count - 1);
            return item;
        }

        /// <summary>
        /// Adds a new item to the front of the queue.
        /// </summary>
        /// <param name="item">The item</param>
        public void Add(T item)
        {
            PushFront(item);
        }

        /// <summary>
        /// Removes all the items from the queue.
        /// </summary>
        public void Clear()
        {
            _elements.Clear();
        }

        /// <summary>
        /// Copies the entire queue to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The target array</param>
        /// <param name="arrayIndex">The starting index</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes an item from the queue.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>Whether or not the removal was successful</returns>
        public bool Remove(T item)
        {
            return _elements.Remove(item);
        }

        /// <summary>
        /// Checks whether or not the queue contains a specific item.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if the item exists in the queue</returns>
        public bool Contains(T item)
        {
            return _elements.Contains(item);
        }

        /// <summary>
        /// Returns the items in the queue in a one-dimensional array format.
        /// </summary>
        /// <returns>The array of queue elements</returns>
        public T[] ToArray()
        {
            return _elements.ToArray();
        }

        /// <summary>
        /// Returns the queue's enumerator.
        /// </summary>
        /// <returns>The queue's enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <summary>
        /// Finds the index at which an item is in the queue. 0 is the front of the queue.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>The zero-based index of the first occurrence of the item within the entire queue, if found; otherwise, –1.</returns>
        public int IndexOf(T item)
        {
            return _elements.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item into the queue at a specific poition in the queue.
        /// </summary>
        /// <param name="index">The index to insert the item</param>
        /// <param name="item">The item to insert</param>
        public void Insert(int index, T item)
        {
            _elements.Insert(index, item);
        }

        /// <summary>
        /// Removes the item at a specific position in the queue.
        /// </summary>
        /// <param name="index">The index at which to remove an item</param>
        public void RemoveAt(int index)
        {
            Assert.IsTrue(_elements.Count > 0, "DoubleEndedQueue is empty.");
            _elements.RemoveAt(index);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Double-ended Queue (" + _elements.Count + "): ");
            for (int i = 0; i < _elements.Count; ++i) sb.Append(_elements[i] + " ");
            return sb.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
