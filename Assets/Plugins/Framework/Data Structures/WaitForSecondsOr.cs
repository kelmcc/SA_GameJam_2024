using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    public class WaitForSecondsOr : CustomYieldInstruction
    {
        public override bool keepWaiting => Time.time - _startTime < _duration && !_breakCondition();

        private float _duration;
        private float _startTime;
        private Func<bool> _breakCondition;

        public WaitForSecondsOr(float duration, Func<bool> breakCondition)
        {
            _duration = duration;
            _startTime = Time.time;
            _breakCondition = breakCondition;
        }
    }

    public class WaitForSecondsRealtimeOr : CustomYieldInstruction
    {
        public override bool keepWaiting => Time.realtimeSinceStartup - _startTime < _duration && !_breakCondition();

        private float _duration;
        private float _startTime;
        private Func<bool> _breakCondition;

        public WaitForSecondsRealtimeOr(float duration, Func<bool> breakCondition)
        {
            _duration = duration;
            _startTime = Time.realtimeSinceStartup;
            _breakCondition = breakCondition;
        }
    }
}
