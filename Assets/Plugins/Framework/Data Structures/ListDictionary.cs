using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    /// <summary>
    /// A dictionry of lists.
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">List element type</typeparam>
    public class ListDictionary<TKey, TValue>
    {
        public int Count => _dictionary.Count;


        private Dictionary<TKey, List<TValue>> _dictionary = new Dictionary<TKey, List<TValue>>();


        public void Add(TKey key, TValue value)
        {
            List<TValue> list;
            if (!_dictionary.TryGetValue(key, out list))
            {
                list = new List<TValue>();
                _dictionary.Add(key, list);
            }

            list.Add(value);
        }

        public bool Remove(TKey key, TValue value)
        {

            List<TValue> list;
            if (_dictionary.TryGetValue(key, out list))
            {
                bool removed = list.Remove(value);
                if (removed && list.Count == 0)
                {
                    _dictionary.Remove(key);
                }

                return removed;
            }

            return false;
        }

        public int GetCount(TKey key)
        {
            List<TValue> list;
            if (_dictionary.TryGetValue(key, out list))
            {
                return list.Count;
            }

            return 0;
        }

        public TKey[] GetKeys()
        {
            return _dictionary.GetKeys();
        }

        public TValue GetValue(TKey key, int index)
        {
            List<TValue> list;
            if (_dictionary.TryGetValue(key, out list))
            {
                return list[index];
            }

            throw new KeyNotFoundException();
        }

        public TValue[] GetValues(TKey key)
        {
            List<TValue> list;
            if (_dictionary.TryGetValue(key, out list))
            {
                return list.ToArray();
            }

            throw new KeyNotFoundException();
        }

        public List<TValue> GetList(TKey key)
        {
            if (_dictionary.TryGetValue(key, out List<TValue> list))
            {
                return list;
            }

            return null;
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

    }
}
