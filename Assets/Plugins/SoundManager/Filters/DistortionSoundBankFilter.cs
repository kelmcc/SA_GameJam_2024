using System;
using Framework;
using UnityEngine;

namespace SoundManager
{
    [Serializable]
    public class DistortionSoundBankFilter : ComponentSoundBankFilter<AudioDistortionFilter>
    {
        [Clamp]
        [Tooltip("Distortion value. 0.0 to 1.0. Default = 0.5.")]
        public float distortionLevel = 0.5f;

        protected DistortionSoundBankFilter()
        {
            Reset();
        }


        public DistortionSoundBankFilter(float distortionLevel = 0.5f)
        {
            this.distortionLevel = distortionLevel;

            _isEnabled = true;
        }

        public override void OnAdded(EffectSoundInstance sound)
        {
            base.OnAdded(sound);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].distortionLevel = distortionLevel;
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].distortionLevel = distortionLevel;
            }
        }


        public override void Reset()
        {
            base.Reset();

            distortionLevel = 0.5f;
        }
    }
}
