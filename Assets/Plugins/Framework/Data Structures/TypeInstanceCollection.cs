using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Framework
{
    /// <summary>
    /// Data structure for storing lists of objects by their type. Maintains a dictionary of lists of instances all belonging to a common base type.
    /// </summary>
    /// <typeparam name="T">The base type</typeparam>
    public class TypeInstanceCollection<T> : IEnumerable<T>
    {

        private Dictionary<Type, List<T>> _lists = new Dictionary<Type, List<T>>();
        private HashSet<Type> _instanceTypes = new HashSet<Type>();

        /// <summary>
        /// Adds a range of instances to the collection.
        /// </summary>
        /// <param name="items">The range of instances to add</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Removes a range of instances to the collection.
        /// </summary>
        /// <param name="items">The range of instances to remove</param>
        public void RemoveRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Adds an instance to the collection.
        /// </summary>
        /// <param name="item">The instance to add</param>
        public void Add(T item)
        {
            Type type = item.GetType();

            if (typeof(T).IsInterface)
            {

                Type[] interfaceTypes = type.GetInterfaces();

                for (int i = 0; i < interfaceTypes.Length; i++)
                {
                    if (typeof(T).IsAssignableFrom(interfaceTypes[i]))
                    {

                        _instanceTypes.Add(interfaceTypes[i]);

                        List<T> list;
                        if (_lists.TryGetValue(interfaceTypes[i], out list))
                        {
                            Assert.IsFalse(list.Contains(item), "Item already exists in TypeInstanceCollection");
                            list.Add(item);
                        }
                        else
                        {
                            _lists.Add(interfaceTypes[i], new List<T>() { item });
                        }
                    }
                }
            }
            else
            {


                _instanceTypes.Add(type);

                List<T> list;
                if (_lists.TryGetValue(type, out list))
                {
                    Assert.IsFalse(list.Contains(item), "Item already exists in TypeInstanceCollection");
                    list.Add(item);
                }
                else
                {
                    _lists.Add(type, new List<T>() { item });
                }
            }
        }

        /// <summary>
        /// Empties the collection.
        /// </summary>
        public void Clear()
        {
            _lists.Clear();
            _instanceTypes.Clear();
        }

        /// <summary>
        /// Finds the number instances of a specific type in the collection.
        /// </summary>
        /// <typeparam name="E">The type to check</typeparam>
        /// <param name="includeChildTypes">Whether or not to match child types, or just the exact type</param>
        /// <returns>The number of instances of the type in the collection</returns>
        public int GetCount<E>(bool includeChildTypes = true) where E : T
        {
            List<T> list;
            if (includeChildTypes)
            {
                int totalCount = 0;
                foreach (Type type in _instanceTypes)
                {
                    if (typeof(E).IsAssignableFrom(type) && _lists.TryGetValue(type, out list))
                    {
                        totalCount += list.Count;
                    }
                }

                return totalCount;
            }

            if (_lists.TryGetValue(typeof(E), out list))
            {
                return list.Count;
            }

            return 0;
        }

        /// <summary>
        /// Checks whether at least one instance of a type exists in the collection.
        /// </summary>
        /// <typeparam name="E">The type to check</typeparam>
        /// <param name="includeChildTypes">Whether or not to match child types, or just the exact type</param>
        /// <returns>True if an instance exists</returns>
        public bool HasAnyInstances<E>(bool includeChildTypes = true) where E : T
        {
            List<T> list;
            if (_lists.TryGetValue(typeof(E), out list))
            {
                if (list.Count > 0) return true;
            }

            if (includeChildTypes)
            {
                foreach (Type type in _instanceTypes)
                {
                    if (typeof(E).IsAssignableFrom(type) && _lists.TryGetValue(type, out list))
                    {
                        if (list.Count > 0) return true;
                    }
                }

            }

            return false;
        }

        /// <summary>
        /// Finds the first instance of a specific type in the collection.
        /// </summary>
        /// <typeparam name="E">The type to check</typeparam>
        /// <param name="includeChildTypes">Whether or not to match child types, or just the exact type</param>
        /// <returns>The first instance of the type in the collection, or default(E) if none was found (null for reference types)</returns>
        public E GetFirstInstance<E>(bool includeChildTypes = true) where E : T
        {
            List<T> list;
            if (includeChildTypes)
            {
                foreach (Type type in _instanceTypes)
                {
                    if (typeof(E).IsAssignableFrom(type) && _lists.TryGetValue(type, out list))
                    {
                        return (E)list[0];
                    }
                }
            }

            if (_lists.TryGetValue(typeof(E), out list))
            {
                return (E)list[0];
            }

            return default(E);
        }

        /// <summary>
        /// Finds all instances of a specific type in the collection.
        /// </summary>
        /// <typeparam name="E">The type to check</typeparam>
        /// <param name="includeChildTypes">Whether or not to match child types, or just the exact type</param>
        /// <returns>An array of all the instances of the type in the collection</returns>
        public List<E> GetInstances<E>(bool includeChildTypes = true) where E : T
        {
            List<T> list;
            if (includeChildTypes)
            {
                List<T> completeList = new List<T>();
                foreach (Type type in _instanceTypes)
                {
                    if (typeof(E).IsAssignableFrom(type) && _lists.TryGetValue(type, out list))
                    {
                        completeList.AddRange(list);
                    }
                }

                return completeList.Cast<E>().ToList();
            }

            if (_lists.TryGetValue(typeof(E), out list))
            {
                return list.Cast<E>().ToList();
            }

            return new List<E>();
        }

        /// <summary>
        /// Tries to remove an instance from the collection.
        /// </summary>
        /// <param name="item">The instance to remove</param>
        /// <returns>True if the instance was removed, false if it was not found</returns>
        public bool Remove(T item)
        {
            Type type = item.GetType();
            List<T> list;
            if (_lists.TryGetValue(type, out list) && list.Remove(item))
            {
                if (list.Count == 0)
                {
                    _lists.Remove(type);
                    _instanceTypes.Remove(type);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets an enumerator that enumerates through all the instances (of a specific type) in the collection.
        /// </summary>
        /// <returns>The instance enumerator</returns>
        public IEnumerator<E> GetEnumerator<E>(bool includeChildTypes = true) where E : T
        {
            List<T> list;
            if (includeChildTypes)
            {
                foreach (Type type in _instanceTypes)
                {
                    if (typeof(E).IsAssignableFrom(type) && _lists.TryGetValue(type, out list))
                    {
                        List<E> specficList = list as List<E>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            yield return specficList[i];
                        }
                    }
                }

            }

            if (_lists.TryGetValue(typeof(E), out list))
            {
                List<E> specficList = list as List<E>;
                for (int i = 0; i < list.Count; i++)
                {
                    yield return specficList[i];
                }
            }
        }

        /// <summary>
        /// Gets an enumerator that enumerates through all the instances (of all types) in the collection.
        /// </summary>
        /// <returns>The instance enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (List<T> list in _lists.Values)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    yield return list[i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}