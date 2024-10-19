using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Framework
{
    /// <summary>
    /// Contains extension methods for collections.
    /// </summary>
    public static class CollectionExtensions
    {
        public static bool ContainsSameElements<T>(this IList<T> list, IList<T> otherList, bool mustBeSameOrder)
        {
            if (list.Count != otherList.Count) return false;

            if (mustBeSameOrder)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (!list[i].Equals(otherList[i])) return false;
                }

                return true;
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (!otherList.Contains(list[i])) return false;
            }

            return true;
        }

        public static T GetNextElement<T>(this IList<T> list, T currentElement, bool wrap = true)
        {
            if (wrap) return list[MathUtils.WrapIndex(list.IndexOf(currentElement) + 1, list.Count)];
            return list[list.IndexOf(currentElement) + 1];
        }

        public static T GetPreviousElement<T>(this IList<T> list, T element, bool wrap = true)
        {
            if (wrap) return list[MathUtils.WrapIndex(list.IndexOf(element) - 1, list.Count)];
            return list[list.IndexOf(element) - 1];
        }

        public static T[] Concat<T>(this T[] sourceArray, params T[][] addedArrays)
        {
            int totalLength = sourceArray.Length;
            int startIndex = totalLength;

            for (int i = 0; i < addedArrays.Length; i++)
            {
                totalLength += addedArrays[i].Length;
            }

            T[] result = new T[totalLength];
            sourceArray.CopyTo(result, 0);

            for (int i = 0; i < addedArrays.Length; i++)
            {
                addedArrays[i].CopyTo(result, startIndex);
                startIndex += addedArrays[i].Length;
            }

            return result;
        }


        public static T[] Concat<T>(this T[] array, T[] extraArray)
        {
            T[] result = new T[array.Length + extraArray.Length];
            array.CopyTo(result, 0);
            extraArray.CopyTo(result, array.Length);

            return result;
        }

        public static void AddIfNotContained<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }

        public static void SetValues<T>(this IList<T> list, T value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = value;
            }
        }

        public static void SetValues<T>(this IList<T> list, IList<T> values)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = values[i];
            }
        }

        public static void SetValues<T>(this T[] array, T[] values)
        {
            Array.Copy(values, array, array.Length);
        }

        public static TResult[] SelectArray<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector)
        {
            return enumerable.Select(selector).ToArray();
        }
        public static TResult[] SelectArray<T, TResult>(this IEnumerable<T> enumerable, Func<T, int, TResult> selector)
        {
            return enumerable.Select(selector).ToArray();
        }

        public static List<TResult> SelectList<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector)
        {
            return enumerable.Select(selector).ToList();
        }
        public static List<TResult> SelectList<T, TResult>(this IEnumerable<T> enumerable, Func<T, int, TResult> selector)
        {
            return enumerable.Select(selector).ToList();
        }

        public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            T item = list[oldIndex];
            list.RemoveAt(oldIndex);
            if (newIndex > oldIndex) newIndex--;
            list.Insert(newIndex, item);
        }

        public static void Move<T>(this List<T> list, T element, int newIndex)
        {
            int oldIndex = list.IndexOf(element);
            list.RemoveAt(oldIndex);
            if (newIndex > oldIndex) newIndex--;
            list.Insert(newIndex, element);
        }

        public static void PushInOrder<T>(this Stack<T> queue, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                queue.Push(item);
            }
        }

        public static void EnqueInOrder<T>(this Queue<T> queue, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                queue.Enqueue(item);
            }
        }

        public static bool ContainsAny<T>(this IList<T> collection, IList<T> elements)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                for (int j = 0; j < elements.Count; j++)
                {
                    if (collection[i].Equals(elements[j])) return true;
                }
            }

            return false;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> collection, IEnumerable<T> elements)
        {
            foreach (T a in collection)
            {
                foreach (T b in elements)
                {
                    if (b.Equals(a)) return true;
                }
            }

            return false;
        }

        public static bool Contains<T>(this T[] array, T element)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null && array[i].Equals(element)) return true;
            }

            return false;
        }

        public static bool Contains<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            foreach (T element in collection)
            {
                if (predicate(element)) return true;
            }

            return false;
        }

        public static List<T> AddIfNotNull<T>(this List<T> list, T element) where T : class
        {
            if (element != null)
            {
                list.Add(element);
            }

            return list;
        }

        public static List<T> WithAddedElement<T>(this List<T> list, T element)
        {
            list.Add(element);
            return list;
        }

        public static T[] WithAddedElement<T>(this T[] array, T element)
        {
            T[] newArray = new T[array.Length + 1];

            Array.Copy(array, 0, newArray, 0, array.Length);
            newArray[newArray.Length - 1] = element;

            return newArray;
        }

        public static T[] WithInsertedElement<T>(this T[] array, T element, int index)
        {
            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            T[] newArray = new T[array.Length + 1];

            if (index == 0)
            {
                Array.Copy(array, 0, newArray, 1, array.Length);
            }
            else if (index == array.Length)
            {
                Array.Copy(array, 0, newArray, 0, array.Length);
            }
            else
            {
                Array.Copy(array, 0, newArray, 0, index);
                Array.Copy(array, index, newArray, index + 1, array.Length - index);
            }

            newArray[index] = element;

            return newArray;
        }

        public static List<T> RemoveDuplicates<T>(this IList<T> list)
        {
            HashSet<T> set = new HashSet<T>();
            List<T> newList = new List<T>(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                if (!set.Contains(item))
                {
                    set.Add(item);
                    newList.Add(item);
                }
            }

            return newList;
        }

        public static List<T> RemoveNullValues<T>(this IList<T> list) where T : class
        {

            List<T> newList = new List<T>(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null) newList.Add(list[i]);
            }

            return newList;
        }

        public static void Remove<T>(this List<T> list, IList<T> elementsToRemove)
        {
            for (int i = 0; i < elementsToRemove.Count; i++)
            {
                list.Remove(elementsToRemove[i]);
            }
        }

        public static void FastRemoveAt<T>(this List<T> list, int index)
        {
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }

        public static void InsertSorted<T>(this List<T> list, T item, IComparer<T> comparer)
        {
            if (list.Count == 0)
            {
                list.Add(item);
                return;
            }

            if (comparer.Compare(item, list[list.Count - 1]) <= 0)
            {
                list.Add(item);
                return;
            }

            if (comparer.Compare(item, list[0]) >= 0)
            {
                list.Insert(0, item);
                return;
            }

            int index = list.BinarySearch(item);

            if (index < 0)
            {
                index = ~index;
            }

            list.Insert(index, item);
        }

        public static void InsertSorted<T>(this List<T> list, T item) where T : IComparable<T>
        {
            if (list.Count == 0)
            {
                list.Add(item);
                return;
            }

            if (list[list.Count - 1].CompareTo(item) <= 0)
            {
                list.Add(item);
                return;
            }

            if (list[0].CompareTo(item) >= 0)
            {
                list.Insert(0, item);
                return;
            }

            int index = list.BinarySearch(item);

            if (index < 0)
            {
                index = ~index;
            }

            list.Insert(index, item);
        }

        public static bool ElementsAreEqual<T>(this IList<T> listA, IList<T> listB)
        {
            if (listB == null || listA.Count != listB.Count) return false;

            for (int i = 0; i < listA.Count; i++)
            {
                if (!listA[i].Equals(listB[i])) return false;
            }

            return true;
        }

        public static void AddRange<T>(this ICollection<T> collection, IList<T> range, int startIndex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                collection.Add(range[startIndex + i]);
            }
        }

        public static void AddRange<T>(this ICollection<T> collection, IList<T> range)
        {
            for (int i = 0; i < range.Count; i++)
            {
                collection.Add(range[i]);
            }
        }

        public static T[] WithAddedRange<T>(this T[] array, IList<T> range)
        {
            T[] result = new T[array.Length + range.Count];
            Array.Copy(array, result, array.Length);

            for (int i = 0; i < range.Count; i++)
            {
                result[array.Length + i] = range[i];
            }

            return result;
        }

        public static T GetNearest<T>(this IList<T> list, Vector3 point) where T : Component
        {
            float minDist = Mathf.Infinity;
            T nearest = null;

            for (int i = 0; i < list.Count; i++)
            {
                float dist = (list[i].transform.position - point).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = list[i];
                }
            }

            return nearest;
        }

        public static T GetNearestWhen<T>(this IList<T> list, Vector3 point, Predicate<T> filter) where T : Component
        {
            float minDist = Mathf.Infinity;
            T nearest = null;

            for (int i = 0; i < list.Count; i++)
            {
                float dist = (list[i].transform.position - point).sqrMagnitude;
                if (dist < minDist && filter(list[i]))
                {
                    minDist = dist;
                    nearest = list[i];
                }
            }

            return nearest;
        }

        public static T GetFurthest<T>(this IList<T> list, Vector3 point) where T : Component
        {
            float maxDist = -Mathf.Infinity;
            T furthest = null;

            for (int i = 0; i < list.Count; i++)
            {
                float dist = (list[i].transform.position - point).sqrMagnitude;
                if (dist > maxDist)
                {
                    maxDist = dist;
                    furthest = list[i];
                }
            }

            return furthest;
        }

        public static T GetFurthestWhen<T>(this IList<T> list, Vector3 point, Predicate<T> filter) where T : Component
        {
            float maxDist = -Mathf.Infinity;
            T furthest = null;

            for (int i = 0; i < list.Count; i++)
            {
                float dist = (list[i].transform.position - point).sqrMagnitude;
                if (dist > maxDist && filter(list[i]))
                {
                    maxDist = dist;
                    furthest = list[i];
                }
            }

            return furthest;
        }

        public static Vector3 GetNearest(this IList<Vector3> list, Vector3 point)
        {
            float minDist = Mathf.Infinity;
            Vector3 nearest = Vector3.zero;

            for (int i = 0; i < list.Count; i++)
            {
                float dist = (list[i] - point).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = list[i];
                }
            }

            return nearest;
        }

        public static Vector3 GetFurthest(this IList<Vector3> list, Vector3 point)
        {
            float maxDist = -Mathf.Infinity;
            Vector3 furthest = Vector3.zero;

            for (int i = 0; i < list.Count; i++)
            {
                float dist = (list[i] - point).sqrMagnitude;
                if (dist > maxDist)
                {
                    maxDist = dist;
                    furthest = list[i];
                }
            }

            return furthest;
        }

        public static Vector2 GetNearest(this IList<Vector2> list, Vector2 point)
        {
            float minDist = Mathf.Infinity;
            Vector2 nearest = Vector2.zero;

            for (int i = 0; i < list.Count; i++)
            {
                float dist = (list[i] - point).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = list[i];
                }
            }

            return nearest;
        }

        public static Vector2 GetFurthest(this IList<Vector2> list, Vector2 point)
        {
            float maxDist = -Mathf.Infinity;
            Vector2 furthest = Vector2.zero;

            for (int i = 0; i < list.Count; i++)
            {
                float dist = (list[i] - point).sqrMagnitude;
                if (dist > maxDist)
                {
                    maxDist = dist;
                    furthest = list[i];
                }
            }

            return furthest;
        }

        public static TValue FindMax<TItem, TValue>(this IEnumerable<TItem> collection, Func<TItem, TValue> getValueFunction) where TValue : IComparable<TValue>
        {
            bool first = true;
            TValue max = default;

            foreach (TItem item in collection)
            {
                TValue value = getValueFunction(item);

                if (first)
                {
                    max = value;
                    first = false;
                }
                else if (value.CompareTo(max) > 0)
                {
                    max = value;
                }
            }

            return max;
        }

        public static TValue FindMax<TItem, TValue>(this IEnumerable<TItem> collection, Func<TItem, TValue> getValueFunction, out TItem maxItem) where TValue : IComparable<TValue>
        {
            bool first = true;
            TValue max = default;
            maxItem = default;

            foreach (TItem item in collection)
            {
                TValue value = getValueFunction(item);

                if (first)
                {
                    max = value;
                    maxItem = item;
                    first = false;
                }
                else if (value.CompareTo(max) > 0)
                {
                    max = value;
                    maxItem = item;
                }
            }

            return max;
        }

        public static T FindMax<T>(this IEnumerable<T> collection) where T : IComparable<T>
        {
            bool first = true;
            T max = default;

            foreach (T item in collection)
            {
                if (first)
                {
                    max = item;
                    first = false;
                }
                else if (item.CompareTo(max) > 0)
                {
                    max = item;
                }
            }

            return max;
        }

        public static TValue FindMin<TItem, TValue>(this IEnumerable<TItem> collection, Func<TItem, TValue> getValueFunction) where TValue : IComparable<TValue>
        {
            bool first = true;
            TValue min = default;

            foreach (TItem item in collection)
            {
                TValue value = getValueFunction(item);

                if (first)
                {
                    min = value;
                    first = false;
                }
                else if (value.CompareTo(min) < 0)
                {
                    min = value;
                }
            }

            return min;
        }

        public static TValue FindMin<TItem, TValue>(this IEnumerable<TItem> collection, Func<TItem, TValue> getValueFunction, out TItem maxItem) where TValue : IComparable<TValue>
        {
            bool first = true;
            TValue min = default;
            maxItem = default;

            foreach (TItem item in collection)
            {
                TValue value = getValueFunction(item);

                if (first)
                {
                    min = value;
                    maxItem = item;
                    first = false;
                }
                else if (value.CompareTo(min) < 0)
                {
                    min = value;
                    maxItem = item;
                }
            }

            return min;
        }

        public static T FindMin<T>(this IEnumerable<T> collection) where T : IComparable<T>
        {
            bool first = true;
            T min = default;

            foreach (T item in collection)
            {
                if (first)
                {
                    min = item;
                    first = false;
                }
                else if (item.CompareTo(min) < 0)
                {
                    min = item;
                }
            }

            return min;
        }


        public static T ChooseRandom<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static void SortByDistance<T>(this List<T> list, Vector3 point, bool nearestFirst = true) where T : Component
        {
            list.Sort(new SortByDistanceComparer(point, nearestFirst));
        }

        private struct SortByDistanceComparer : IComparer<Component>
        {
            private Vector3 _point;
            private bool _nearestFirst;

            public SortByDistanceComparer(Vector3 point, bool nearestFirst)
            {
                _point = point;
                _nearestFirst = nearestFirst;
            }

            public int Compare(Component x, Component y)
            {
                int comparison = (x.transform.position - _point).sqrMagnitude.CompareTo((y.transform.position - _point).sqrMagnitude);
                return _nearestFirst ? comparison : -comparison;
            }
        }

        public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, bool ascending)
        {
            return ascending ? source.OrderBy(selector) : source.OrderByDescending(selector);
        }

        public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector, bool ascending)
        {
            return ascending ? source.ThenBy(selector) : source.ThenByDescending(selector);
        }

        public static void SortByMultipleCriteria<T>(this List<T> list, IList<IComparer<T>> comparers, bool ascending = true)
        {
            if (comparers.Count > 0)
            {
                list.Sort(new MultiComparer<T>(comparers, ascending));
            }
        }

        public static void SortByMultipleCriteria<T>(this List<T> list, IList<Comparison<T>> compareFunctions, bool ascending = true)
        {
            if (compareFunctions.Count > 0)
            {
                list.Sort(new MultiComparison<T>(compareFunctions, ascending));
            }
        }

        private class MultiComparer<T> : IComparer<T>
        {
            private IList<IComparer<T>> _comparers;
            private bool _ascending;

            public MultiComparer(IList<IComparer<T>> comparers, bool ascending)
            {
                _comparers = comparers;
                _ascending = ascending;
            }

            public int Compare(T a, T b)
            {
                return _ascending ? Compare(a, b, 0) : -Compare(a, b, 0);
            }

            int Compare(T a, T b, int index)
            {
                int result = _comparers[index].Compare(a, b);

                if (result == 0 && index < _comparers.Count - 1)
                {
                    return Compare(a, b, index + 1);
                }

                return result;
            }
        }

        private class MultiComparison<T> : IComparer<T>
        {
            private IList<Comparison<T>> _comparers;
            private bool _ascending;

            public MultiComparison(IList<Comparison<T>> comparers, bool ascending)
            {
                _comparers = comparers;
                _ascending = ascending;
            }

            public int Compare(T a, T b)
            {
                return _ascending ? Compare(a, b, 0) : -Compare(a, b, 0);
            }

            int Compare(T a, T b, int index)
            {
                int result = _comparers[index].Invoke(a, b);

                if (result == 0 && index < _comparers.Count - 1)
                {
                    return Compare(a, b, index + 1);
                }

                return result;
            }
        }

        public static int IndexOf<T>(this IList<T> list, T element)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(element)) return i;
            }

            return -1;
        }


        public static object[] GetCollectionElements(this ICollection collection)
        {
            object[] array = new object[collection.Count];
            int index = 0;

            foreach (object element in collection)
            {
                array[index++] = element;
            }

            return array;
        }

        public static TV GetValueOrNull<TK, TV>(this Dictionary<TK, TV> dictionary, TK key) where TV : class
        {
            if (dictionary.TryGetValue(key, out TV value))
            {
                return value;
            }

            return null;
        }

        public static void SetBytes(this byte[] byteArray, int startIndex, byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[startIndex + i] = bytes[i];
            }
        }

        /// <summary>
        /// Shorthand method for Array.AsReadOnly(array)
        /// </summary>
        /// <returns> Returns a read only collection of the array elements</returns>
        public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array)
        {
            if (array == null) return null;

            return Array.AsReadOnly(array);
        }

        /// <summary>
        /// Randomizes the order of the elements in the collection. Uses Fisher-Yates algorithm.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static string GetElementNamesAsString(this IEnumerable<UnityEngine.Object> collection, bool oneLine = true)
        {
            StringBuilder builder = new StringBuilder();

            foreach (UnityEngine.Object obj in collection)
            {
                builder.Append(obj.name);
                builder.Append(oneLine ? ", " : "\n");
            }

            if (builder.Length > 0)
            {
                return builder.ToString(0, builder.Length - (oneLine ? 2 : 1));
            }

            return "NO ELEMENTS";
        }


        public static string GetElementsAsString(this IEnumerable collection, bool oneLine = true)
        {
            StringBuilder builder = new StringBuilder();

            foreach (object obj in collection)
            {
                builder.Append(obj);
                builder.Append(oneLine ? ", " : "\n");
            }

            if (builder.Length > 0)
            {
                return builder.ToString(0, builder.Length - (oneLine ? 2 : 1));
            }

            return "NO ELEMENTS";
        }

        public static string GetElementsAsString<T>(this IEnumerable<T> collection, string seperator, Func<T, string> toStringFunction = null)
        {


            StringBuilder builder = new StringBuilder();

            foreach (T obj in collection)
            {
                builder.Append(toStringFunction == null ? obj.ToString() : toStringFunction(obj));
                builder.Append(seperator);
            }

            if (builder.Length > 0)
            {
                return builder.ToString(0, builder.Length - seperator.Length);
            }

            return "NO ELEMENTS";
        }

        public static T[] SubArray<T>(this T[] array, int startIndex, int length)
        {
            T[] subset = new T[length];
            Array.Copy(array, startIndex, subset, 0, length);
            return subset;
        }



        public static T[] SubArray<T>(this T[] array, int startIndex)
        {
            T[] subset = new T[array.Length - startIndex];
            Array.Copy(array, startIndex, subset, 0, array.Length - startIndex);
            return subset;
        }

        public static void Resize<T>(this List<T> list, int size, T element = default(T))
        {
            int count = list.Count;

            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)
                {
                    list.Capacity = size;
                }

                list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }

        /// <summary>
        /// Returns an array containing the elements in the collection, but in a random order. Uses Fisher-Yates algorithm.
        /// </summary>
        /// <returns>The new shuffled array</returns>
        public static T[] Shuffled<T>(this IList<T> list)
        {
            T[] array = new T[list.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = list[i];
            }

            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }

            return array;
        }

        /// <summary>
        /// Removes the first occurence of an element from an array. Does so by performing at least one (usually two) array copies.
        /// </summary>
        /// <param name="element">The element to remove</param>
        /// <param name="result">The output array with the element removed</param>
        /// <returns>Whether or not the element was found and removed</returns>
        public static bool Remove<T>(this T[] array, T element, out T[] result)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(element))
                {
                    result = new T[array.Length - 1];

                    if (i > 0)
                    {
                        Array.Copy(array, 0, result, 0, i);
                    }

                    if (i < array.Length - 1)
                    {
                        Array.Copy(array, i + 1, result, i, array.Length - i - 1);
                    }

                    return true;
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Reverses the order of the elements in the collection.
        /// </summary>
        public static IList<T> Reverse<T>(this IList<T> list)
        {

            for (int i = 0; i < list.Count * 0.5f; i++)
            {
                T temp = list[list.Count - 1 - i];
                list[list.Count - 1 - i] = list[i];
                list[i] = temp;
            }

            return list;
        }

        /// <summary>
        /// Gets the element at an index that will be wrapped if it is out of range.
        /// </summary>
        /// <param name="index"> The index to get</param>
        /// <returns> The value at the wrapped index</returns>
        public static T GetWrapped<T>(this IList<T> list, int index)
        {
            Assert.IsTrue(list.Count > 0);

            return list[MathUtils.Wrap(index, 0, list.Count - 1)];
        }

        /// <summary>
        /// Removes and returns the item at the end of the list.
        /// </summary>
        public static T Pop<T>(this IList<T> list)
        {
            Assert.IsTrue(list.Count > 0);

            T result = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return result;
        }

        /// <summary>
        /// Fill the list with a value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value">Value to fill the list with</param>
        /// <param name="count">Number of values to fill</param>
        /// <param name="extend">When false the list will be cleared, when true the list will be filled until the list length reaches count</param>
        public static void Fill<T>(this IList<T> list, T value, int count, bool extend)
        {
            if (!extend)
                list.Clear();

            for (int i = 0; i < count; ++i)
            {
                list.Add(value);
            }
        }
        public static void AddRange<K, V>(this Dictionary<K, V> dict, K[] keys, V[] values)
        {
            Assert.IsTrue(keys.Length == values.Length);

            for (int i = 0; i < keys.Length; i++)
            {
                dict.Add(keys[i], values[i]);
            }
        }

        public static V[] GetValues<K, V>(this Dictionary<K, V> dict)
        {
            V[] values = new V[dict.Count];
            dict.Values.CopyTo(values, 0);

            return values;
        }

        public static K[] GetKeys<K, V>(this Dictionary<K, V> dict)
        {
            K[] keys = new K[dict.Count];
            dict.Keys.CopyTo(keys, 0);

            return keys;
        }

        /// <summary>
        /// Gets the value of the entry at specific index position in the dictionary, can be a useful shorthand for when you are iterating through all the entires in a dictionary.
        /// </summary>
        /// <typeparam name="K">Key type</typeparam>
        /// <typeparam name="V">Value type</typeparam>
        /// <param name="index">The index to lookup</param>
        /// <returns>The value retrieved</returns>
        public static V GetValueAt<K, V>(this Dictionary<K, V> dict, int index)
        {

            int count = 0;
            foreach (KeyValuePair<K, V> kvp in dict)
            {
                if (count == index)
                {
                    return kvp.Value;
                }
                count++;
            }
            throw new Exception("Dictionary does not contain a value at index " + index);
        }

        /// <summary>
        /// Gets the key of the entry at specific index position in the dictionary, can be a useful shorthand for when you are iterating through all the entires in a dictionary.
        /// </summary>
        /// <typeparam name="K">Key type</typeparam>
        /// <typeparam name="V">Value type</typeparam>
        /// <param name="index">The index to lookup</param>
        /// <returns>The key retrieved</returns>
        public static K GetKeyAt<K, V>(this Dictionary<K, V> dict, int index)
        {
            int count = 0;
            foreach (KeyValuePair<K, V> kvp in dict)
            {
                if (count == index)
                {
                    return kvp.Key;
                }
                count++;
            }
            throw new Exception("Dictionary does not contain a key at index " + index);
        }

        /// <summary>
        /// Returns whether or not the collection has no elements.
        /// </summary>
        /// <returns>Whether or not the collection has no elements</returns>
        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }

        /// <summary>
        /// Clones the collection.
        /// </summary>
        /// <returns>The cloned collection</returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> collection) where T : ICloneable
        {
            return collection.Select(item => (T)item.Clone());
        }
    }

    public class VectorMagnitudeComparer : IComparer<Vector3>
    {
        public int Compare(Vector3 x, Vector3 y)
        {
            return x.sqrMagnitude.CompareTo(y.sqrMagnitude);
        }
    }


    public class ExplicitOrderComparer<T> : IComparer<T>
    {
        public IList<T> Order;

        public ExplicitOrderComparer(IList<T> order)
        {
            Order = order;
        }

        public int Compare(T a, T b)
        {
            if (a == null && b == null) return 0;
            if (b == null) return 1;
            if (a == null) return -1;

            int indexA = -1;
            int indexB = -1;

            for (int i = 0; i < Order.Count; i++)
            {
                if (indexA == -1 && Order[i].Equals(a)) indexA = i;
                if (indexB == -1 && Order[i].Equals(b)) indexB = i;
            }

            return -indexA.CompareTo(indexB);
        }
    }
}