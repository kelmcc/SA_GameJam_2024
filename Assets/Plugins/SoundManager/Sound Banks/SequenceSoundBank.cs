using System;
using Framework;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundManager
{
    [CreateAssetMenu(fileName = "New Sequence Bank", menuName = "Sound Bank/Sequence Bank")]
    public class SequenceSoundBank : SoundBank
    {


        [Serializable]
        public class SectionData
        {
            public int EffectiveRepititions => Loop ? 1 : Repititions;

            [Tooltip("The section sound. Can either be an Audio Clip or another Sound Bank.")]
            public ClipOrBank Sound = new ClipOrBank(true);

            [Clamp(0, Mathf.Infinity)]
            [Tooltip("Controls the attenuation of the sound as the listener moves away from the source.\n\nMin: Distance at which sound is still 100% volume.\nMax: Distance at which sound is now 0% volume.")]
            public FloatRange RolloffDistance = new FloatRange(1, 10);

            [Tooltip("Whether or not this section should continue looping until it is manually skipped past.")]
            public bool Loop;

            [MinValue(1)]
            [Tooltip("The number of times this section should play before moving on to the next one.")]
            public int Repititions = 1;

            [Tooltip("Delay (in seconds) before this section stars playing. Note that this can be negative if the section should start before the previous one is finished.")]
            public float Delay;

            [SerializeField]
            private bool _initialized;
        }

        public AudioMixerGroup OutputMixer => _outputMixer;
        public SectionData[] Sections => _sections;

        [SerializeField]
        [Tooltip("The audio mixer group that the audio source will ouput to when playing.")]
        private AudioMixerGroup _outputMixer;

        [SerializeField]
        [Tooltip("Array of sounds to play in order.")]
        private SectionData[] _sections;

        public SequenceSoundInstance Fetch(ISoundPool soundPool)
        {
            if (!IsPlayable()) return null;

            SequenceSoundInstance sound = soundPool.FetchFromPool<SequenceSoundInstance>();
            sound.name = name;
            sound.SetDelay(_defaultDelay);
            sound.SetIgnoreListenerSettings(_ignoreListenerSettings);
            sound.SetDopplerLevel(_dopplerLevel);
            sound.SetOffscreenVolumeMultiplier(_offscreenVolumeMultiplier);
            sound.SetOutOfSightVolumeMultiplier(_outOfSightVolumeMultiplier);

            for (int i = 0; i < _sections.Length; i++)
            {
                if (!_sections[i].Sound.IsEmpty)
                {
                    sound.AddSection(_sections[i]);
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
            for (int i = 0; i < _sections.Length; i++)
            {
                if (_sections[i].Sound.HasValue) return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public override SoundInstance TestInEditor(ISoundPool soundPool)
        {
            SequenceSoundInstance sound = Fetch(soundPool);

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
            SoundManagerSettings.SequenceSoundBankData data = SoundManagerSettings.Instance.SequenceBank;
            _outOfSightVolumeMultiplier = data.OutOfSightVolumeMultiplier;
            _offscreenVolumeMultiplier = data.OffScreenVolumeMultiplier;
            _ignoreListenerSettings = data.IgnoreListenerSettings;
            _dopplerLevel = data.DopplerLevel;
            _defaultDelay = data.DefaultDelay;
            _outputMixer = data.OutputMixer;
        }


    }


    public static class SequenceSoundBankExtenstions
    {
        /// <summary>
        /// Fetches and then plays a non-spatial sound, ie. one that does not emit from a specific location and rolloff.
        /// </summary>
        /// <param name="soundBank">The sound bank used to set-up the sound instance</param>
        public static SequenceSoundInstance Play(this SequenceSoundBank soundBank)
        {
            if (soundBank != null)
            {
                SequenceSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
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
        public static SequenceSoundInstance Play(this SequenceSoundBank soundBank, Vector3 position)
        {
            if (soundBank != null)
            {
                SequenceSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
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
        public static SequenceSoundInstance Play(this SequenceSoundBank soundBank, Transform followTransform)
        {
            if (soundBank != null)
            {
                SequenceSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
                sound.Play3D(followTransform);
                soundBank.OnPlayed(sound);

                return sound;
            }

            return null;
        }
    }
}