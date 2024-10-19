using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace Framework
{

    public abstract class SpringBase<T>
    {
        public float Frequency
        {
            get => _frequency;
            set => _frequency = Mathf.Max(0, value);
        }

        public float Damping
        {
            get => _damping;
            set => _damping = Mathf.Clamp01(value);
        }

        public bool UseUnscaledTime
        {
            get => _useUnscaledTime;
            set => _useUnscaledTime = value;
        }

        public T TargetValue
        {
            get => _targetValue;
            set => _targetValue = value;
        }

        public T CurrentValue
        {
            get => _currentValue;
            set => _currentValue = value;
        }

        public T Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        [SerializeField, MinValue(0)]
        protected float _frequency = 10f;

        [SerializeField, Clamp]
        protected float _damping = 0.5f;

        [SerializeField]
        protected bool _useUnscaledTime;

        protected T _currentValue;
        protected T _targetValue;
        protected T _velocity;

        public SpringBase(float frequency, float damping, bool useUnscaledTime = false)
        {
            _frequency = Mathf.Max(0, frequency);
            _damping = Mathf.Clamp01(damping);
            _useUnscaledTime = useUnscaledTime;
        }

        public abstract void Nudge(T force);

        public abstract T Update();

        public T Update(T targetValue)
        {
            _targetValue = targetValue;
            return Update();
        }

    }

    [Serializable]
    public class Spring : SpringBase<float>
    {

        public Spring(float frequency, float damping, bool useUnscaledTime = false) : base(frequency, damping, useUnscaledTime)
        {
        }

        public Spring(float frequency, float damping, float startingValue, bool useUnscaledTime = false) : base(frequency, damping, useUnscaledTime)
        {
            _currentValue = startingValue;
            _targetValue = startingValue;
        }

        public Spring(float frequency, float damping, float startingValue, float targetValue, bool useUnscaledTime = false) : base(frequency, damping, useUnscaledTime)
        {
            _currentValue = startingValue;
            _targetValue = targetValue;
        }

        public Spring(Spring spring) : base(spring._frequency, spring._damping, spring._useUnscaledTime)
        {
            _currentValue = spring._currentValue;
            _targetValue = spring._targetValue;
        }

        public override void Nudge(float force)
        {
            _velocity += force;
        }

        public override float Update()
        {
            MathUtils.CalculateDampedSpringMotionParameters(_damping, _frequency, _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime, out float pp, out float pv, out float vp, out float vv);
            MathUtils.SimpleHarmonicMotion(ref _currentValue, ref _velocity, _targetValue, pp, pv, vp, vv);

            return _currentValue;
        }
    }

    [Serializable]
    public class Spring2 : SpringBase<Vector2>
    {
        public Spring2(float frequency, float damping, bool useUnscaledTime = false) : base(frequency, damping, useUnscaledTime)
        {
        }

        public Spring2(float frequency, float damping, Vector2 startingValue, bool useUnscaledTime = false) : base(frequency, damping, useUnscaledTime)
        {
            _currentValue = startingValue;
            _targetValue = startingValue;
        }

        public Spring2(float frequency, float damping, Vector2 startingValue, Vector2 targetValue, bool useUnscaledTime = false) : base(frequency, damping, useUnscaledTime)
        {
            _currentValue = startingValue;
            _targetValue = targetValue;
        }

        public Spring2(Spring2 spring) : base(spring._frequency, spring._damping, spring._useUnscaledTime)
        {
            _currentValue = spring._currentValue;
            _targetValue = spring._targetValue;
        }


        public override void Nudge(Vector2 force)
        {
            _velocity += force;
        }

        public override Vector2 Update()
        {
            MathUtils.CalculateDampedSpringMotionParameters(_damping, _frequency, _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime, out float pp, out float pv, out float vp, out float vv);

            float velocityX = _velocity.x;
            float stateX = _currentValue.x;
            float targetX = _targetValue.x;

            MathUtils.SimpleHarmonicMotion(ref stateX, ref velocityX, targetX, pp, pv, vp, vv);

            float velocityY = _velocity.y;
            float stateY = _currentValue.y;
            float targetY = _targetValue.y;

            MathUtils.SimpleHarmonicMotion(ref stateY, ref velocityY, targetY, pp, pv, vp, vv);

            _velocity = new Vector2(velocityX, velocityY);
            _currentValue = new Vector2(stateX, stateY);

            return _currentValue;
        }
    }

    [Serializable]
    public class Spring3 : SpringBase<Vector3>
    {
        public Spring3(float frequency, float damping, bool useUnscaledTime = false) : base(frequency, damping, useUnscaledTime)
        {
        }

        public Spring3(float frequency, float damping, Vector3 startingValue, bool useUnscaledTime = false) : base(frequency, damping, useUnscaledTime)
        {
            _currentValue = startingValue;
            _targetValue = startingValue;
        }

        public Spring3(float frequency, float damping, Vector3 startingValue, Vector3 targetValue, bool useUnscaledTime = false) : base(frequency, damping, useUnscaledTime)
        {
            _currentValue = startingValue;
            _targetValue = targetValue;
        }

        public Spring3(Spring3 spring) : base(spring._frequency, spring._damping, spring._useUnscaledTime)
        {
            _currentValue = spring._currentValue;
            _targetValue = spring._targetValue;
        }

        public override void Nudge(Vector3 force)
        {
            _velocity += force;
        }

        public override Vector3 Update()
        {
            MathUtils.CalculateDampedSpringMotionParameters(_damping, _frequency, _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime, out float pp, out float pv, out float vp, out float vv);

            float velocityX = _velocity.x;
            float stateX = _currentValue.x;
            float targetX = _targetValue.x;

            MathUtils.SimpleHarmonicMotion(ref stateX, ref velocityX, targetX, pp, pv, vp, vv);

            float velocityY = _velocity.y;
            float stateY = _currentValue.y;
            float targetY = _targetValue.y;

            MathUtils.SimpleHarmonicMotion(ref stateY, ref velocityY, targetY, pp, pv, vp, vv);

            float velocityZ = _velocity.z;
            float stateZ = _currentValue.z;
            float targetZ = _targetValue.z;

            MathUtils.SimpleHarmonicMotion(ref stateZ, ref velocityZ, targetZ, pp, pv, vp, vv);

            _velocity = new Vector3(velocityX, velocityY, velocityZ);
            _currentValue = new Vector3(stateX, stateY, stateZ);
            return _currentValue;
        }
    }
}