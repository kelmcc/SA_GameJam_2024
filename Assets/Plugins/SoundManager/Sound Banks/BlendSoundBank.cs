using System;
using Framework;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundManager
{
    [CreateAssetMenu(fileName = "New Blend Bank", menuName = "Sound Bank/Blend Bank")]
    public class BlendSoundBank : SoundBank
    {

        [Serializable]
        public class LayerData
        {
            [Tooltip("The layer sound. Can either be an Audio Clip or another Sound Bank.")]
            public ClipOrBank Sound = new ClipOrBank(true);

            [Tooltip("Controls the volume of this layer based on the value of the blend parameter.\n\nX Axis: Value of blend parameter (0 - 1).\nY Axis: Corresponding volume multiplier.")]
            public AnimationCurve BlendCurve = AnimationCurve.Linear(0, 0, 1, 1);

            [Clamp(0, Mathf.Infinity)]
            [Tooltip("Controls the attenuation of the sound as the listener moves away from the source.\n\nMin: Distance at which sound is still 100% volume.\nMax: Distance at which sound is now 0% volume.")]
            public FloatRange RolloffDistance = new FloatRange(1, 10);

            [Tooltip("Whether or not this section should continue looping once it has finished playing.")]
            public bool Loop;

            [SerializeField]
            private bool _initialized;
        }

        public AudioMixerGroup OutputMixer => _outputMixer;
        public LayerData[] Layers => _layers;

        [SerializeField]
        [Tooltip("The audio mixer group that the audio source will ouput to when playing.")]
        private AudioMixerGroup _outputMixer;

        [SerializeField]
        [Tooltip("Array of sounds to play and blend together between based on a parameter.")]
        private LayerData[] _layers;

        public BlendSoundInstance Fetch(ISoundPool soundPool)
        {
            if (!IsPlayable()) return null;

            BlendSoundInstance sound = soundPool.FetchFromPool<BlendSoundInstance>();
            sound.name = name;
            sound.SetIgnoreListenerSettings(_ignoreListenerSettings);
            sound.SetDopplerLevel(_dopplerLevel);
            sound.SetOutOfSightVolumeMultiplier(_outOfSightVolumeMultiplier);
            sound.SetOffscreenVolumeMultiplier(_offscreenVolumeMultiplier);
            sound.SetDelay(_defaultDelay);

            for (int i = 0; i < _layers.Length; i++)
            {
                if (!_layers[i].Sound.IsEmpty)
                {
                    sound.AddLayer(_layers[i]);
                }
            }


            if (_outputMixer != null)
            {
                sound.SetMixerGroup(_outputMixer);
            }

            AddFilters(sound);

            return sound;
        }

        bool IsPlayable()
        {
            for (int i = 0; i < _layers.Length; i++)
            {
                if (_layers[i].Sound.HasValue) return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public override SoundInstance TestInEditor(ISoundPool soundPool)
        {
            BlendSoundInstance sound = Fetch(soundPool);

            if (sound != null)
            {
                sound.Play2D();
                OnPlayed(sound);

                return sound;
            }

            return null;
        }
#endif

        [ContextMenu("Apply Default Values")]
        public override void ApplyDefaultValues()
        {
            SoundManagerSettings.BlendSoundBankData data = SoundManagerSettings.Instance.BlendBank;
            _outOfSightVolumeMultiplier = data.OutOfSightVolumeMultiplier;
            _offscreenVolumeMultiplier = data.OffScreenVolumeMultiplier;
            _ignoreListenerSettings = data.IgnoreListenerSettings;
            _dopplerLevel = data.DopplerLevel;
            _defaultDelay = data.DefaultDelay;
            _outputMixer = data.OutputMixer;
        }

    }


    public static class BlendSoundBankExtenstions
    {
        /// <summary>
        /// Fetches and then plays a non-spatial sound, ie. one that does not emit from a specific location and rolloff.
        /// </summary>
        /// <param name="soundBank">The sound bank used to set-up the sound instance</param>
        public static BlendSoundInstance Play(this BlendSoundBank soundBank)
        {
            if (soundBank != null)
            {
                BlendSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
                sound.Play2D();
                soundBank.OnPlayed(sound);

                return sound;
            }

            return null;
        }

        /// <summary>
        /// Fetches and then plays a sound at a specific position.
        /// </summary>
        /// <param name="position">The world-space position of the sound emission</param>
        public static BlendSoundInstance Play(this BlendSoundBank soundBank, Vector3 position)
        {
            if (soundBank != null)
            {
                BlendSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
                sound.Play3D(position);
                soundBank.OnPlayed(sound);

                return sound;
            }

            return null;
        }

        /// <summary>
        /// Fetches and then plays a sound that will follow a transform around.
        /// </summary>
        /// <param name="followTransform">The transform to follow</param>
        public static BlendSoundInstance Play(this BlendSoundBank soundBank, Transform followTransform)
        {
            if (soundBank != null)
            {
                BlendSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
                sound.Play3D(followTransform);
                soundBank.OnPlayed(sound);

                return sound;
            }

            return null;
        }
    }
}