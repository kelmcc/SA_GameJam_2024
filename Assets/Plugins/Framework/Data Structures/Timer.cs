using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public struct Timer
    {
        public bool HasFinished => _timeRemaining <= 0;
        public bool HasFinishedThisFrame => _frameFinished == Time.frameCount;
        public float TimeRemaining => _timeRemaining;
        public float TimeElapsed => _duration - _timeRemaining;
        public float ProportionElapsed => 1f - (_timeRemaining / _duration);
        public float ProportionRemaining => _timeRemaining / _duration;
        public float Duration => _duration;

        [SerializeField]
        private float _timeRemaining;

        [SerializeField]
        private float _duration;

        private int _frameFinished;

        public Timer(float duration)
        {
            _duration = Mathf.Max(0, duration);
            _timeRemaining = _duration;
            _frameFinished = -1;
        }


        public void Reset()
        {
            _timeRemaining = _duration;
        }

        public void Reset(float duration)
        {
            _duration = Mathf.Max(0, duration);
            _timeRemaining = _duration;
        }

        public bool Update()
        {
            if (_timeRemaining > 0 && _timeRemaining <= Time.deltaTime)
            {
                _frameFinished = Time.frameCount;
            }
            _timeRemaining -= Time.deltaTime;

            return _timeRemaining <= 0;
        }

        public bool Update(float elapsedTime)
        {
            if (_timeRemaining > 0 && _timeRemaining <= elapsedTime)
            {
                _frameFinished = Time.frameCount;
            }
            _timeRemaining -= elapsedTime;

            return _timeRemaining <= 0;
        }

        public override string ToString()
        {
            return _timeRemaining.ToString();
        }

        public static implicit operator float(Timer timer)
        {
            return timer._timeRemaining;
        }

        public static implicit operator Timer(float duration)
        {
            return new Timer(duration);
        }


    }
}

