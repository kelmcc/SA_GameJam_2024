using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class SeedList : IEnumerable<float>
    {
        public int Count
        {
            get => _seeds.Count;
            set => SetCount(value);
        }
        public bool IsReadOnly => false;

        public float this[int index]
        {
            get { EnsureCount(index + 1); return _seeds[index]; }
            set { EnsureCount(index + 1); _seeds[index] = value; }
        }

        private List<float> _seeds = new List<float>();
        private float _seedRange;
        private int _index;

        public SeedList(float seedRange = 1f)
        {
            _seedRange = seedRange;
        }

        public float GetNext()
        {
            float value = GetValue(_index);
            _index++;
            return value;
        }

        public float GetNextTime(float multiplier = 1f)
        {
            float value = GetValue(_index);
            _index++;
            return (value + Time.time) * multiplier;
        }

        public float GetNextSin(float speed, float magnitude = 1f)
        {
            float value = GetValue(_index);
            _index++;

            return Mathf.Sin(value + Time.time * speed) * magnitude;
        }

        public float GetNextNormalizedSin(float speed, float magnitude = 1f)
        {
            float value = GetValue(_index);
            _index++;

            return MathUtils.NormalizedSin(value + Time.time * speed) * magnitude;
        }

        public float GetNextPerlin(float speed, float magnitude = 1f)
        {
            float value = GetValue(_index);
            _index++;

            return Noise.GetAnimatedPerlin(value, speed, magnitude);
        }

        public Vector2 GetNextPerlin2D(float speed, float magnitude = 1f)
        {
            float value = GetValue(_index);
            _index++;

            return Noise.GetAnimatedPerlin2D(value, speed, magnitude);
        }

        public Vector2 GetNextPerlin3D(float speed, float magnitude = 1f)
        {
            float value = GetValue(_index);
            _index++;

            return Noise.GetAnimatedPerlin2D(value, speed, magnitude);
        }


        public float GetNextNormalizedPerlin(float speed, float magnitude = 1f)
        {
            float value = GetValue(_index);
            _index++;

            return Noise.GetNormalizedAnimatedPerlin(value, speed, magnitude);
        }

        public Vector2 GetNextNormalizedPerlin2D(float speed, float magnitude = 1f)
        {
            float value = GetValue(_index);
            _index++;

            return Noise.GetNormalizedAnimatedPerlin2D(value, speed, magnitude);
        }

        public Vector2 GetNextNormalizedPerlin3D(float speed, float magnitude = 1f)
        {
            float value = GetValue(_index);
            _index++;

            return Noise.GetNormalizedAnimatedPerlin3D(value, speed, magnitude);
        }

        public void ResetIndex()
        {
            _index = 0;
        }

        public IEnumerator<float> GetEnumerator()
        {
            return _seeds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public float GetValue(int index)
        {
            EnsureCount(index + 1);

            return _seeds[index];
        }

        public void Randomize()
        {
            for (int i = 0; i < _seeds.Count; i++)
            {
                _seeds[i] = Random.value * _seedRange;
            }
        }

        public void Clear()
        {
            _seeds.Clear();
        }

        void EnsureCount(int count)
        {
            if (_seeds.Count < count)
            {
                while (_seeds.Count < count)
                {
                    _seeds.Add(Random.value * _seedRange);
                }
            }
        }

        void SetCount(int count)
        {
            if (_seeds.Count > count)
            {
                _seeds.RemoveRange(count, _seeds.Count - count);
            }
            else
            {
                while (_seeds.Count < count)
                {
                    _seeds.Add(Random.value * _seedRange);
                }
            }
        }

        public void CopyTo(float[] array, int arrayIndex)
        {
            _seeds.CopyTo(array, arrayIndex);
        }

    }
}
