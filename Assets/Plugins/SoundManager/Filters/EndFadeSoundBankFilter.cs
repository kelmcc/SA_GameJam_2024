using System;
using Framework;
using UnityEngine;

namespace SoundManager
{
    [Serializable]
    public class EndFadeSoundBankFilter : SoundBankFilter
    {
        [MinValue(0)]
        [Tooltip("The length (in seconds) of the fade.")]
        public float duration = 1f;

        [MinValue(0)]
        [Tooltip("Time (in seconds) before the end of the clip by which the fade must have fully completed.")]
        public float endDelay = 0f;

        [Clamp]
        [Tooltip("The volume level at the beginning of the fade.")]
        public float startVolume = 1f;

        [Clamp]
        [Tooltip("The final volume level at the end of the fade.")]
        public float endVolume = 0f;

        [Tooltip("Controls the volume of this sound before the end of its play time.\n\nX Axis: Normalized time (based on duration).\nY Axis: Normalized value to use for interpolating between start volume and end volume.")]
        public AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

        protected EndFadeSoundBankFilter()
        {
            Reset();
        }

        public EndFadeSoundBankFilter(float duration = 1f, float endDelay = 0f, float startVolume = 1f, float endVolume = 0f)
        {
            fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

            this.duration = duration;
            this.endDelay = endDelay;
            this.startVolume = startVolume;
            this.endVolume = endVolume;

            _isEnabled = true;
        }

        public EndFadeSoundBankFilter(AnimationCurve fadeCurve, float duration = 1f, float endDelay = 0f, float startVolume = 1f, float endVolume = 0f)
        {
            this.fadeCurve = fadeCurve;
            this.duration = duration;
            this.endDelay = endDelay;
            this.startVolume = startVolume;
            this.endVolume = endVolume;

            _isEnabled = true;
        }

        public override void Reset()
        {
            base.Reset();

            duration = 1f;
            endDelay = 0f;
            startVolume = 1f;
            endVolume = 0f;
            fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        }

        public override float GetVolumeMultiplier(float time, float clipLength)
        {
            time = Mathf.InverseLerp(clipLength - duration - endDelay, clipLength - duration - endDelay + duration, time);

            if (time <= 0) return startVolume;
            if (time >= 1f || duration == 0) return endVolume;

            return Mathf.Lerp(startVolume, endVolume, fadeCurve.Evaluate(time));
        }
    }
}
