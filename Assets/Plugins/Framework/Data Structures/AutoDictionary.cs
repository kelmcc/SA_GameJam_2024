using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// Dictionary that automatically creates a value when you use the indexer.
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public class AutoDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!_dictionary.TryGetValue(key, out value))
                {
                    if (!typeof(TValue).IsValueType)
                    {
                        value = Activator.CreateInstance<TValue>();
                    }

                    _dictionary.Add(key, value);
                }

                return value;
            }

            set
            {
                if (!_dictionary.ContainsKey(key))
                {
                    _dictionary[key] = value;
                }
                else
                {
                    _dictionary.Add(key, value);
                }
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary.TryGetValue(item.Key, out TValue value))
            {
                return item.Value.Equals(value);
            }

            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<TKey, TValue> pair in _dictionary)
            {
                array[arrayIndex] = pair;
                arrayIndex++;

                if (arrayIndex == array.Length) return;
            }

        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Remove(item.Key);
        }


    }
}
