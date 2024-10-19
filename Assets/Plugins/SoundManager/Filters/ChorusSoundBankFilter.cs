using System;
using Framework;
using UnityEngine;

namespace SoundManager
{
    [Serializable]
    public class ChorusSoundBankFilter : ComponentSoundBankFilter<AudioChorusFilter>
    {
        [Clamp]
        [Tooltip("Volume of original signal to pass to output. 0.0 to 1.0. Default = 0.5.")]
        public float dryMix = 0.5f;

        [Clamp]
        [Tooltip("Volume of 1st chorus tap. 0.0 to 1.0. Default = 0.5.")]
        public float wetMix1 = 0.5f;

        [Clamp]
        [Tooltip("Volume of 2nd chorus tap. This tap is 90 degrees out of phase of the first tap. 0.0 to 1.0. Default = 0.5.")]
        public float wetMix2 = 0.5f;

        [Clamp]
        [Tooltip("Volume of 3rd chorus tap. This tap is 90 degrees out of phase of the second tap. 0.0 to 1.0. Default = 0.5.")]
        public float wetMix3 = 0.5f;

        [Clamp(0.1f, 100f)]
        [Tooltip("The LFO’s delay in ms. 0.1 to 100.0. Default = 40.0 ms")]
        public float delay = 40f;

        [Clamp(0, 20)]
        [Tooltip("The LFO’s modulation rate in Hz. 0.0 to 20.0. Default = 0.8 Hz.")]
        public float rate = 0.8f;

        [Clamp]
        [Tooltip("Chorus modulation depth. 0.0 to 1.0. Default = 0.03.")]
        public float depth = 0.03f;

        protected ChorusSoundBankFilter()
        {
            Reset();
        }

        public ChorusSoundBankFilter(float dryMix = 0.5f, float wetMix1 = 0.5f, float wetMix2 = 0.5f, float wetMix3 = 0.5f, float delay = 40f, float rate = 0.8f, float depth = 0.03f)
        {
            this.dryMix = dryMix;
            this.wetMix1 = wetMix1;
            this.wetMix2 = wetMix2;
            this.wetMix3 = wetMix3;
            this.delay = delay;
            this.rate = rate;
            this.depth = depth;

            _isEnabled = true;
        }

        public override void OnAdded(EffectSoundInstance sound)
        {
            base.OnAdded(sound);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].dryMix = dryMix;
                _filterComponents[i].wetMix1 = wetMix1;
                _filterComponents[i].wetMix2 = wetMix2;
                _filterComponents[i].wetMix3 = wetMix3;
                _filterComponents[i].delay = delay;
                _filterComponents[i].rate = rate;
                _filterComponents[i].depth = depth;
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].dryMix = dryMix;
                _filterComponents[i].wetMix1 = wetMix1;
                _filterComponents[i].wetMix2 = wetMix2;
                _filterComponents[i].wetMix3 = wetMix3;
                _filterComponents[i].delay = delay;
                _filterComponents[i].rate = rate;
                _filterComponents[i].depth = depth;
            }
        }

        public override void Reset()
        {
            base.Reset();

            dryMix = 0.5f;
            wetMix1 = 0.5f;
            wetMix2 = 0.5f;
            wetMix3 = 0.5f;
            delay = 40f;
            rate = 0.8f;
            depth = 0.03f;
        }
    }
}