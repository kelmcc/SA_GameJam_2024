using System;
using UnityEngine;

namespace SoundManager
{
    [Serializable]
    public class VolumeCurveSoundBankFilter : SoundBankFilter
    {
        [Tooltip("Controls the volume of this sound throughout the duration of its play time.\n\nX Axis: Normalized time.\nY Axis: Corresponding volume multiplier.")]
        public AnimationCurve volumeCurve = AnimationCurve.Linear(0, 0, 1, 1);

        protected VolumeCurveSoundBankFilter()
        {
            Reset();
        }

        public VolumeCurveSoundBankFilter(AnimationCurve volumeCurve)
        {
            this.volumeCurve = volumeCurve;
            _isEnabled = true;
        }

        public override void Reset()
        {
            base.Reset();

            volumeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        }

        public override float GetVolumeMultiplier(float time, float clipLength)
        {
            return volumeCurve.Evaluate(time / clipLength);
        }

    }
}