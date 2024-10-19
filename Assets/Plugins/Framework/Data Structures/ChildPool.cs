using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{
    /// <summary>
    /// Simple class to manage a pool of GameObjects parented to the same transform.
    /// </summary>
    public class ChildPool<T> : IEnumerable<T> where T : Component
    {
        public int ElementCount => _activeElementCount;
        public int PoolSize => _elementPool.Count;
        public T this[int index] => GetElement(index);

        private List<T> _elementPool = new List<T>();
        private int _activeElementCount;
        private Transform _parentTransform;
        private GameObject _elementPrefab;
        private bool _inAddMode;

        /// <summary>
        /// Creates a new Child pool of the specified MonoBehaviour type.
        /// </summary>
        /// <param name="parentTransform">The transform that all the elements will be childed to</param>
        /// <param name="elementPrefab">The prefab to instantiate for each element</param>
        /// <param name="initialPoolSize">The initial number of elements to instantiate in the pool</param>
        public ChildPool(Transform parentTransform, GameObject elementPrefab, int initialPoolSize = 3)
        {
            Assert.IsNotNull(elementPrefab, "Child Pool prefab is null");
            Assert.IsNotNull(parentTransform, "Child Pool parent is null");
            Assert.IsTrue(initialPoolSize >= 0);
            Assert.IsNotNull(elementPrefab.GetComponentInChildren<T>(true));

            _parentTransform = parentTransform;
            _elementPrefab = elementPrefab;

            if (!elementPrefab.IsPrefabAsset())
            {
                elementPrefab.SetActive(false);
            }

            SetElementCount(initialPoolSize);
            Clear();
        }

        /// <summary>
        /// Creates a new Child pool of the specified MonoBehaviour type.
        /// </summary>
        /// <param name="prototype">The prototype object to clone for each element</param>
        /// <param name="initialPoolSize">The initial number of elements to instantiate in the pool</param>
        public ChildPool(GameObject prototype, int initialPoolSize = 3)
        {
            Assert.IsNotNull(prototype, "Child Pool prototype is null");
            Assert.IsTrue(prototype.scene.name != null, "Child Pool prototype is not a scene object");
            Assert.IsTrue(initialPoolSize >= 0);
            Assert.IsNotNull(prototype.GetComponentInChildren<T>(true));

            _parentTransform = prototype.transform.parent;
            _elementPrefab = prototype;

            prototype.SetActive(false);

            SetElementCount(initialPoolSize);
            Clear();
        }

        /// <summary>
        /// Finds the
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetElement(int index)
        {
            Assert.IsTrue(index >= 0);
            Assert.IsTrue(index < _activeElementCount);

            return _elementPool[index];
        }

        public void Clear()
        {
            SetElementCount(0);
        }

        public void BeginAdding()
        {
            if (_inAddMode)
            {
                throw new UnityException("ChildPool cannot enter add mode - Already in add mode! Ensure you always call EndAdding() after BeginAdding()");
            }

            _activeElementCount = 0;
            _inAddMode = true;
        }

        public void EndAdding()
        {
            if (!_inAddMode)
            {
                throw new UnityException("ChildPool cannot exit add mode - Not in add mode! Ensure you always call BeginAdding() before EndAdding()");
            }

            for (int i = 0; i < _elementPool.Count; i++)
            {
                if (i < _activeElementCount)
                {
                    _elementPool[i].gameObject.SetActive(true);
                }
                else
                {
                    _elementPool[i].gameObject.SetActive(false);
                }
            }

            _inAddMode = false;
        }

        public T Add()
        {


            _activeElementCount++;
            if (_activeElementCount - 1 == _elementPool.Count)
            {
                GrowPool();
            }

            if (!_inAddMode)
            {
                _elementPool[_activeElementCount - 1].gameObject.SetActive(true);
            }

            return _elementPool[_activeElementCount - 1];
        }

        public T Peek()
        {
            if (_activeElementCount > 0)
            {
                return _elementPool[_activeElementCount - 1];
            }
            return null;
        }

        public bool Remove(T item)
        {
            Assert.IsNotNull(item);

            if (_elementPool.Remove(item))
            {
                _elementPool.Add(item);
                item.gameObject.SetActive(false);
                _activeElementCount--;
                return true;
            }
            return false;
        }


        public void SetElementCount(int numElements)
        {
            Assert.IsTrue(numElements >= 0);

            _activeElementCount = numElements;

            for (int i = _elementPool.Count; i < _activeElementCount; i++)
            {
                GrowPool();
            }

            for (int i = 0; i < _elementPool.Count; i++)
            {
                if (i < _activeElementCount)
                {
                    _elementPool[i].gameObject.SetActive(true);
                }
                else
                {
                    _elementPool[i].gameObject.SetActive(false);
                }
            }
        }

        private void GrowPool()
        {
            T newElement = Object.Instantiate(_elementPrefab, _parentTransform).GetComponentInChildren<T>(true);
            newElement.transform.localScale = Vector3.one;
            _elementPool.Add(newElement);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _activeElementCount; i++)
            {
                yield return _elementPool[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
