using System;
using Framework;
using UnityEngine;

namespace SoundManager
{
    [Serializable]
    public class LowPassSoundBankFilter : ComponentSoundBankFilter<AudioLowPassFilter>
    {
        [Range(0, 22000)]
        [Tooltip("Lowpass cutoff frequency in Hertz (range 0.0 to 22000.0, default = 5000.0).")]
        public int cutoffFrequency = 5000;

        [Clamp(1, 10)]
        [Tooltip("Lowpass resonance quality value (range 1.0 to 10.0, default = 1.0).")]
        public float lowpassResonanceQ = 1f;

        protected LowPassSoundBankFilter()
        {
            Reset();
        }

        public LowPassSoundBankFilter(int cutoffFrequency = 5000, float lowpassResonanceQ = 1f)
        {
            this.cutoffFrequency = cutoffFrequency;
            this.lowpassResonanceQ = lowpassResonanceQ;
            _isEnabled = true;
        }

        public override void OnAdded(EffectSoundInstance sound)
        {
            base.OnAdded(sound);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].cutoffFrequency = cutoffFrequency;
                _filterComponents[i].lowpassResonanceQ = lowpassResonanceQ;
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].cutoffFrequency = cutoffFrequency;
                _filterComponents[i].lowpassResonanceQ = lowpassResonanceQ;
            }
        }

        public override void Reset()
        {
            base.Reset();

            cutoffFrequency = 5000;
            lowpassResonanceQ = 1f;
        }
    }
}