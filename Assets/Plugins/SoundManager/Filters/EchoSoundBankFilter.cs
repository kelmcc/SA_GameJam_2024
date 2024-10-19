using System;
using Framework;
using UnityEngine;

namespace SoundManager
{
    [Serializable]
    public class EchoSoundBankFilter : ComponentSoundBankFilter<AudioEchoFilter>
    {
        [Clamp(10, 5000)]
        [Tooltip("Echo delay in ms. 10 to 5000. Default = 500.")]
        public int delay = 500;

        [Clamp]
        [Tooltip("Echo decay per delay. 0 to 1. 1.0 = No decay, 0.0 = total decay (ie simple 1 line delay). Default = 0.5.L")]
        public float decayRatio = 0.5f;

        [Clamp]
        [Tooltip("Volume of echo signal to pass to output. 0.0 to 1.0. Default = 1.0.")]
        public float wetMix = 1f;

        [Clamp]
        [Tooltip("Volume of original signal to pass to output. 0.0 to 1.0. Default = 1.0.")]
        public float dryMix = 1f;

        protected EchoSoundBankFilter()
        {
            Reset();
        }

        public EchoSoundBankFilter(int delay = 500, float decayRatio = 0.5f, float wetMix = 1f, float dryMix = 1f)
        {
            this.delay = delay;
            this.decayRatio = decayRatio;
            this.wetMix = wetMix;
            this.dryMix = dryMix;

            _isEnabled = true;
        }

        public override void OnAdded(EffectSoundInstance sound)
        {
            base.OnAdded(sound);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].delay = delay;
                _filterComponents[i].decayRatio = decayRatio;
                _filterComponents[i].wetMix = wetMix;
                _filterComponents[i].dryMix = dryMix;
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].delay = delay;
                _filterComponents[i].decayRatio = decayRatio;
                _filterComponents[i].wetMix = wetMix;
                _filterComponents[i].dryMix = dryMix;
            }
        }

        public override void Reset()
        {
            base.Reset();

            delay = 500;
            decayRatio = 0.5f;
            wetMix = 1f;
            dryMix = 1f;
        }
    }
}
