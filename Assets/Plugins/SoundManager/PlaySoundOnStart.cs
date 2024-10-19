using Framework;
using UnityEngine;

namespace SoundManager
{
    public class PlaySoundOnStart : MonoBehaviour
    {
        public enum SoundPosition
        {
            TwoDimensional,
            TransformPosition,
            FollowTransform
        }

        [SerializeField]
        private SoundBank _soundBank;

        [SerializeField]
        private SoundPosition _position;

        [SerializeField]
        private bool _loop;

        [SerializeField]
        [MinValue(0)]
        private float _fadeInDuration = 0;

        private SoundInstance _soundInstance;

        void Start()
        {
            switch (_position)
            {
                case SoundPosition.TwoDimensional:
                    _soundInstance = _soundBank.Play();
                    break;
                case SoundPosition.TransformPosition:
                    _soundInstance = _soundBank.Play(transform.position);
                    break;
                case SoundPosition.FollowTransform:
                    _soundInstance = _soundBank.Play(transform);
                    break;
            }

            if (_loop)
            {
                EffectSoundInstance effectSound = _soundInstance as EffectSoundInstance;
                if (effectSound != null)
                {
                    effectSound.SetLooping(true);
                }
            }

            if (_soundInstance != null && _fadeInDuration > 0)
            {
                _soundInstance.FadeIn(_fadeInDuration, true);
            }
        }

        public void StopSound(float fadeDuration)
        {
            EffectSoundInstance effectSound = _soundInstance as EffectSoundInstance;
            if (effectSound != null)
            {
                effectSound.FadeOutAndDestroy(fadeDuration, true);
            }
        }

    }
}
