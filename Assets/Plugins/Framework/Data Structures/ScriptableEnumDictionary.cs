using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class ScriptableEnumDictionary<K, E> : IEnumerable<E> where K : ScriptableEnum
    {
        public E this[K key]
        {
            get => _elements[key.Index];
            set => _elements[key.Index] = value;
        }

        public E this[int index]
        {
            get => _elements[index];
            set => _elements[index] = value;
        }

        public int Count => _elements.Length;

        private E[] _elements;

        public ScriptableEnumDictionary()
        {
            _elements = new E[ScriptableEnum.GetCount<K>()];
        }

        public IEnumerator<E> GetEnumerator()
        {
            for (int i = 0; i < _elements.Length; i++)
            {
                yield return _elements[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _elements.Length; i++)
            {
                yield return _elements[i];
            }
        }

    }
}
