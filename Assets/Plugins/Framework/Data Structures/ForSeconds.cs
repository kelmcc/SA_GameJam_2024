using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Framework
{
    public abstract class ForTimeYieldInstruction : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get
            {
                if (!_firstUpdate && (_pauseCondition == null || !_pauseCondition()))
                {
                    _elapsedTime += _isRealTime ? Time.unscaledDeltaTime : Time.deltaTime;
                }

                _firstUpdate = false;

                if (_timeAction != null)
                {
                    if (_normalizeTimeParameter)
                    {
                        if (_duration <= 0f)
                        {
                            _timeAction(1f);
                        }
                        else
                        {
                            _timeAction(Mathf.Clamp01(_elapsedTime / _duration));
                        }
                    }
                    else
                    {
                        _timeAction(_elapsedTime);
                    }
                }
                else if (_action != null)
                {
                    _action();
                }

                if (_duration <= 0f) return false;
                if (_continueCondition != null && !_continueCondition()) return false;

                return _elapsedTime < _duration;
            }
        }

        protected bool _firstUpdate = true;
        protected float _elapsedTime;
        protected float _duration;
        protected bool _isRealTime;
        protected bool _normalizeTimeParameter;
        protected Action<float> _timeAction;
        protected Action _action;
        protected Func<bool> _continueCondition;
        protected Func<bool> _pauseCondition;
    }


    public class ForSeconds : ForTimeYieldInstruction
    {
        public ForSeconds(float duration, Action<float> action)
        {
            _duration = duration;
            _timeAction = action;
        }

        public ForSeconds(float duration, Action action)
        {
            _duration = duration;
            _action = action;
        }

    }

    public class ForSecondsNormalized : ForTimeYieldInstruction
    {
        public ForSecondsNormalized(float duration, Action<float> action)
        {
            _duration = duration;
            _timeAction = action;
            _normalizeTimeParameter = true;
        }
    }

    public class ForSecondsRealTime : ForTimeYieldInstruction
    {
        public ForSecondsRealTime(float duration, Action<float> action)
        {
            _duration = duration;
            _timeAction = action;
            _isRealTime = true;
        }

        public ForSecondsRealTime(float duration, Action action)
        {
            _duration = duration;
            _action = action;
            _isRealTime = true;
        }
    }



    public class ForSecondsNormalizedRealTime : ForTimeYieldInstruction
    {
        public ForSecondsNormalizedRealTime(float duration, Action<float> action)
        {
            _normalizeTimeParameter = true;
            _duration = duration;
            _timeAction = action;
            _isRealTime = true;
        }
    }

    public class ForSecondsWhile : ForSeconds
    {
        public ForSecondsWhile(float duration, Action action, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
        }

        public ForSecondsWhile(float duration, Action<float> action, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
        }
    }

    public class ForSecondsNormalizedWhile : ForSecondsNormalized
    {
        public ForSecondsNormalizedWhile(float duration, Action<float> action, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
        }
    }

    public class ForSecondsRealTimeWhile : ForSecondsRealTime
    {
        public ForSecondsRealTimeWhile(float duration, Action action, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
            _isRealTime = true;
        }

        public ForSecondsRealTimeWhile(float duration, Action<float> action, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
            _isRealTime = true;
        }
    }

    public class ForSecondsNormalizedRealTimeWhile : ForSecondsNormalizedRealTime
    {
        public ForSecondsNormalizedRealTimeWhile(float duration, Action<float> action, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
            _isRealTime = true;
        }
    }

    public class ForSecondsPausable : ForTimeYieldInstruction
    {
        public ForSecondsPausable(float duration, Action<float> action, Func<bool> pauseCondition)
        {
            _duration = duration;
            _timeAction = action;
            _pauseCondition = pauseCondition;
        }

        public ForSecondsPausable(float duration, Action action, Func<bool> pauseCondition)
        {
            _duration = duration;
            _action = action;
            _pauseCondition = pauseCondition;
        }

    }

    public class ForSecondsNormalizedPausable : ForTimeYieldInstruction
    {
        public ForSecondsNormalizedPausable(float duration, Action<float> action, Func<bool> pauseCondition)
        {
            _duration = duration;
            _timeAction = action;
            _normalizeTimeParameter = true;
            _pauseCondition = pauseCondition;
        }
    }

    public class ForSecondsPausableRealTime : ForTimeYieldInstruction
    {
        public ForSecondsPausableRealTime(float duration, Action<float> action, Func<bool> pauseCondition)
        {
            _duration = duration;
            _timeAction = action;
            _pauseCondition = pauseCondition;
            _isRealTime = true;
        }

        public ForSecondsPausableRealTime(float duration, Action action, Func<bool> pauseCondition)
        {
            _duration = duration;
            _action = action;
            _pauseCondition = pauseCondition;
            _isRealTime = true;
        }
    }



    public class ForSecondsNormalizedPausableRealTime : ForTimeYieldInstruction
    {
        public ForSecondsNormalizedPausableRealTime(float duration, Action<float> action, Func<bool> pauseCondition)
        {
            _normalizeTimeParameter = true;
            _duration = duration;
            _timeAction = action;
            _pauseCondition = pauseCondition;
            _isRealTime = true;
        }
    }

    public class ForSecondsPausableWhile : ForSeconds
    {
        public ForSecondsPausableWhile(float duration, Action action, Func<bool> pauseCondition, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
            _pauseCondition = pauseCondition;
        }

        public ForSecondsPausableWhile(float duration, Action<float> action, Func<bool> pauseCondition, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
            _pauseCondition = pauseCondition;
        }
    }

    public class ForSecondsNormalizedPausableWhile : ForSecondsNormalized
    {
        public ForSecondsNormalizedPausableWhile(float duration, Action<float> action, Func<bool> pauseCondition, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
            _pauseCondition = pauseCondition;
        }
    }

    public class ForSecondsPausableRealTimeWhile : ForSecondsRealTime
    {
        public ForSecondsPausableRealTimeWhile(float duration, Action action, Func<bool> pauseCondition, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
            _pauseCondition = pauseCondition;
            _isRealTime = true;
        }

        public ForSecondsPausableRealTimeWhile(float duration, Action<float> action, Func<bool> pauseCondition, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
            _pauseCondition = pauseCondition;
            _isRealTime = true;
        }
    }

    public class ForSecondsNormalizedPausableRealTimeWhile : ForSecondsNormalizedRealTime
    {
        public ForSecondsNormalizedPausableRealTimeWhile(float duration, Action<float> action, Func<bool> pauseCondition, Func<bool> continueCondition) : base(duration, action)
        {
            _continueCondition = continueCondition;
            _pauseCondition = pauseCondition;
            _isRealTime = true;
        }
    }

}
