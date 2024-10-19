using System;
using Framework;
using UnityEngine;

namespace SoundManager
{
    [Serializable]
    public class StartFadeSoundBankFilter : SoundBankFilter
    {

        [MinValue(0)]
        [Tooltip("The length (in seconds) of the fade.")]
        public float duration = 1f;

        [MinValue(0)]
        [Tooltip("Time (in seconds) before the fade starts.")]
        public float delay = 0f;

        [Clamp]
        [Tooltip("The volume level at the beginning of the fade.")]
        public float startVolume = 0;

        [Clamp]
        [Tooltip("The final volume level at the end of the fade.")]
        public float endVolume = 1f;

        [Tooltip("Controls the volume of this sound after the start of its play time.\n\nX Axis: Normalized time (based on duration).\nY Axis: Normalized value to use for interpolating between start volume and end volume.")]
        public AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

        protected StartFadeSoundBankFilter()
        {
            Reset();
        }

        public StartFadeSoundBankFilter(float duration = 1f, float delay = 0f, float startVolume = 0, float endVolume = 1f)
        {
            fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

            this.duration = duration;
            this.delay = delay;
            this.startVolume = startVolume;
            this.endVolume = endVolume;

            _isEnabled = true;
        }

        public override void Reset()
        {
            base.Reset();

            duration = 1f;
            delay = 0f;
            startVolume = 0;
            endVolume = 1f;
            fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        }

        public override float GetVolumeMultiplier(float time, float clipLength)
        {
            time = Mathf.InverseLerp(delay, delay + duration, time);

            if (time <= 0) return startVolume;
            if (time >= 1f || duration == 0) return endVolume;

            return Mathf.Lerp(startVolume, endVolume, fadeCurve.Evaluate(time));
        }

    }
}
