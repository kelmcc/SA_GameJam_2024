using System;
using Framework;
using UnityEngine;

namespace SoundManager
{
    [Serializable]
    public class HighPassSoundBankFilter : ComponentSoundBankFilter<AudioHighPassFilter>
    {
        [Range(10, 22000)]
        [Tooltip("Highpass cutoff frequency in Hertz (range 10.0 to 22000.0, default = 5000.0).")]
        public int cutoffFrequency = 5000;

        [Clamp(1, 10)]
        [Tooltip("Highpass resonance quality value (range 1.0 to 10.0, default = 1.0).")]
        public float highpassResonanceQ = 1f;

        protected HighPassSoundBankFilter()
        {
            Reset();
        }

        public HighPassSoundBankFilter(int cutoffFrequency = 5000, float highpassResonanceQ = 1f)
        {
            this.cutoffFrequency = cutoffFrequency;
            this.highpassResonanceQ = highpassResonanceQ;
            _isEnabled = true;
        }

        public override void OnAdded(EffectSoundInstance sound)
        {
            base.OnAdded(sound);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].cutoffFrequency = cutoffFrequency;
                _filterComponents[i].highpassResonanceQ = highpassResonanceQ;
            }
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].cutoffFrequency = cutoffFrequency;
                _filterComponents[i].highpassResonanceQ = highpassResonanceQ;
            }
        }

        public override void Reset()
        {
            base.Reset();

            cutoffFrequency = 5000;
            highpassResonanceQ = 1f;
        }
    }
}