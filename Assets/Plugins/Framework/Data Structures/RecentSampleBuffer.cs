using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Buffer for storing samples of a value by time. Stores for a fixed duration after which old samples are discarded.
    /// </summary>
    /// <typeparam name="T">Sample type</typeparam>
    [Serializable]
    public class RecentSampleBuffer<T> : IEnumerable<T>
    {

        public int Count => _sampleValues.Count;

        [SerializeField] private Queue<float> _sampleTimes = new Queue<float>();
        [SerializeField] private Queue<T> _sampleValues = new Queue<T>();
        [SerializeField] private float _sampleDuration;

        public RecentSampleBuffer(float sampleDuration)
        {
            _sampleDuration = sampleDuration;
        }

        public void Update(T currentValue, float currentTime)
        {
            while (_sampleTimes.Count > 0 && currentTime - _sampleTimes.Peek() >= _sampleDuration)
            {
                _sampleTimes.Dequeue();
                _sampleValues.Dequeue();
            }

            _sampleTimes.Enqueue(currentTime);
            _sampleValues.Enqueue(currentValue);
        }

        public void Clear()
        {
            _sampleTimes.Clear();
            _sampleValues.Clear();

        }

        public IEnumerator<T> GetEnumerator()
        {
            return _sampleValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sampleValues.GetEnumerator();
        }

    }
}
