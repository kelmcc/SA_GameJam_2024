using System;
using UnityEngine;

namespace SoundManager
{
    [Serializable]
    public class PitchCurveSoundBankFilter : SoundBankFilter
    {
        [Tooltip("Controls the pitch of this sound throughout the duration of its play time.\n\nX Axis: Normalized time.\nY Axis: Corresponding pitch multiplier.")]
        public AnimationCurve pitchCurve = AnimationCurve.Constant(0, 1, 1f);

        protected PitchCurveSoundBankFilter()
        {
            Reset();
        }

        public PitchCurveSoundBankFilter(AnimationCurve pitchCurve)
        {
            this.pitchCurve = pitchCurve;
            _isEnabled = true;
        }

        public override void Reset()
        {
            base.Reset();

            pitchCurve = AnimationCurve.Constant(0, 1, 1f);
        }

        public override float GetPitchMultiplier(float time, float clipLength)
        {
            return pitchCurve.Evaluate(time / clipLength);
        }

    }
}