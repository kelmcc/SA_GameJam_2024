using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* DOTween use cases in AF
 
_rippleEffect.DOScale(1.3f, 0.2f);

_ripples.transform.DOMoveY(_enemy.transform.position.y + 0.7f, 2f);
 

_mainCamera.DOShakePosition(10f, new Vector3(0.003f, 0.007f, 0.01f), 7, 90, false).SetLoops(-1, LoopType.Yoyo);
_mainCamera.DOShakeRotation(10f, 0.3f, 3, 90, false).SetLoops(-1, LoopType.Yoyo);
_mainCamera.transform.parent.DOLocalRotate(new Vector3(0f, -25f, 0f), 30f).SetRelative(true).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

transform.DOShakeRotation(_Duration, _RotStrength, 8, 45f, true);

_wheel.DOLocalRotate(new Vector3(0f, 0f, -720f), 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutCubic);

_flashImages[i].transform.DOShakePosition(0.5f, 20f, 10, 180f);

_overlayImage.DOFade(0f, 0.5f);

_levelTimeText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutQuart).SetDelay(1f).OnStart(() => _statAppearSound.Play());

stickersToShow[i].transform.DOScale(1f, 0.3f).From(2f).SetEase(Ease.OutBounce);
 
 */

namespace FrameworkWIP
{

    public abstract partial class Tween
    {
        public bool IsComplete => _elapsedTime >= _duration;
        public float ProportionComplete => Mathf.Clamp01(_elapsedTime / _duration);
        public float ElapsedTime => Mathf.Min(_elapsedTime, _duration);
        public float Duration => _duration;
        public bool IsRunning => _isRunning;
        public bool IsPaused => _isPaused;

        protected float _elapsedTime;
        protected float _duration;
        protected TweenMotion _motion;
        protected bool _useUnscaledTime;
        protected bool _isRunning;
        protected bool _isPaused;

        public void Start()
        {
            TweenManaager.RunTween(this);
        }

        public void Stop()
        {
            TweenManaager.StopRunningTween(this);
        }

        public void Pause()
        {
            SetPaused(true);
        }

        public void Resume()
        {
            SetPaused(false);
        }

        public Tween SetPaused(bool pause)
        {
            _isPaused = pause;
            return this;
        }

        protected void OnStarted()
        {
            _elapsedTime = 0f;
            _isRunning = true;
        }

        protected void OnStopped()
        {
            _isRunning = false;
        }

        protected void OnCompleted()
        {
            _isRunning = false;
        }

        protected abstract void Update(float deltaTime);
    }


    public abstract class Tween<TObject, TValue> : Tween
    {

        protected TObject _targetObject;
        protected TValue _startValue;
        protected TValue _targetValue;
        protected Action<TObject, TValue> _setValueFunction;


        public Tween(TObject targetObject, TValue targetValue, float duration, TweenMotion motion, Func<TObject, TValue> getValueFunction, Action<TObject, TValue> setValueFunction)
        {
            _targetObject = targetObject;
            _targetValue = targetValue;
            _duration = duration;
            _motion = motion;
            _startValue = getValueFunction(targetObject);
            _setValueFunction = setValueFunction;

            Start();
        }

        protected abstract TValue Evaluate(TValue startValue, TValue targetValue, float t, TweenMotion motion);

        protected override void Update(float deltaTime)
        {
            _elapsedTime += deltaTime;
            //  Debug.Log(_startValue + " > " + _targetValue);
            //  Debug.Log(_elapsedTime + " " + Mathf.Clamp01(_elapsedTime / _duration));
            _setValueFunction(_targetObject, Evaluate(_startValue, _targetValue, Mathf.Clamp01(_elapsedTime / _duration), _motion));
        }
    }

    public class FloatTween<TObject> : Tween<TObject, float>
    {
        public FloatTween(TObject targetObject, float targetValue, float duration, TweenMotion motion, Func<TObject, float> getValueFunction, Action<TObject, float> setValueFunction) : base(targetObject, targetValue, duration, motion, getValueFunction, setValueFunction)
        {
        }

        protected override float Evaluate(float startValue, float endValue, float t, TweenMotion motion) => motion.EvaluateFloat(startValue, endValue, t);

    }

    public class Vector3Tween<TObject> : Tween<TObject, Vector3>
    {
        public Vector3Tween(TObject targetObject, Vector3 targetValue, float duration, TweenMotion motion, Func<TObject, Vector3> getValueFunction, Action<TObject, Vector3> setValueFunction) : base(targetObject, targetValue, duration, motion, getValueFunction, setValueFunction)
        {
        }

        protected override Vector3 Evaluate(Vector3 startValue, Vector3 endValue, float t, TweenMotion motion) => motion.EvaluateVector3(startValue, endValue, t);

    }


}

