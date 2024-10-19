using System;
using Framework;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundManager
{
    [CreateAssetMenu(fileName = "New Ambience Bank", menuName = "Sound Bank/Ambience Bank")]
    public class AmbienceSoundBank : SoundBank
    {

        [Serializable]
        public class EffectData
        {
            public enum SpatialMode
            {
                XZHemisphere,
                XZCircle,
                XYCircle,
                Sphere
            }

            [Tooltip("The effect sound. Can either be an Audio Clip or another Sound Bank.")]
            public ClipOrBank Sound = new ClipOrBank(true);

            [Tooltip("The method for choosing where an effect is placed when it plays. These positions are relative to the ambience source.\n\nXZ Hemisphere: Spawn randomly inside a hemisphere with its base on the XZ plane and the top of the dome facing upwards along the Y axis.\nXZ Circle: Spawn randomly inside a circle on the XZ plane.\nXY Circle: Spawn randomly inside a circle on the XY plane.\nSphere: Spawn randomly inside a sphere.")]
            public SpatialMode SpawnMode = SpatialMode.XZHemisphere;

            [Clamp(0, Mathf.Infinity)]
            [Tooltip("Controls the distance from the ambience source that effects can spawn.\n\nMin: Minimum distance from the source.\nMaximum distance from the source.")]
            public FloatRange SpawnDistance = new FloatRange(5, 10);

            [Clamp(0.01f, Mathf.Infinity)]
            [Tooltip("Controls the rate at which effects are randomly spawned. This is the amount of time (in seconds) between consecutive plays of this effect.\n\nMin: Minimum time to wait between plays.\nMax: Maximum time to wait between plays.")]
            public FloatRange Cooldown = new FloatRange(1f, 5f);

            [SerializeField]
            private bool _initialized;
        }

        [Serializable]
        public class LoopData
        {
            [Tooltip("The loop sound. Can either be an Audio Clip or another Sound Bank.")]
            public ClipOrBank Sound = new ClipOrBank(false);

            [Clamp(0, Mathf.Infinity)]
            [Tooltip("Controls the attenuation of the loop as the listener moves away from the source.\n\nMin: Distance at which sound is still 100% volume.\nMax: Distance at which sound is now 0% volume.")]
            public FloatRange RolloffDistance = new FloatRange(1, 10);

            [Clamp]
            [Tooltip("Controls the volume levels the loop will fluctuate between based on perlin noise.\n\nMin: Minimum volume level loop will ever be\nMax: Maximum volume lvel loop will ever be")]
            public FloatRange VolumeVariationRange = new FloatRange(0.5f, 0.5f);

            [MinValue(0)]
            [Tooltip("Speed at which loop volume randomly fluctuates.")]
            public float VaritaionFrequency = 0.1f;

            [SerializeField]
            private bool _initialized;
        }

        public AudioMixerGroup OutputMixer => _outputMixer;
        public LoopData[] Loops => _loops;
        public EffectData[] Effects => _effects;

        [SerializeField]
        [Tooltip("The audio mixer group that the audio source will ouput to when playing.")]
        private AudioMixerGroup _outputMixer;

        [SerializeField]
        [Tooltip("Array of loops to constantly play.")]
        private LoopData[] _loops;

        [SerializeField]
        [Tooltip("Array of effects to play randomly.")]
        private EffectData[] _effects;

#if UNITY_EDITOR
        private static AudioListener _audioListener;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _audioListener = null;
        }
#endif

        public AmbienceSoundInstance Fetch(ISoundPool soundPool)
        {
            if (!IsPlayable()) return null;

            AmbienceSoundInstance sound = soundPool.FetchFromPool<AmbienceSoundInstance>();
            sound.SetIgnoreListenerSettings(_ignoreListenerSettings);
            sound.SetDopplerLevel(_dopplerLevel);
            sound.SetOutOfSightVolumeMultiplier(_outOfSightVolumeMultiplier);
            sound.SetOffscreenVolumeMultiplier(_offscreenVolumeMultiplier);
            sound.SetDelay(_defaultDelay);
            sound.name = name;


            for (int i = 0; i < _effects.Length; i++)
            {
                if (!_effects[i].Sound.IsEmpty)
                {
                    sound.AddEffect(_effects[i]);
                }
            }

            for (int i = 0; i < _loops.Length; i++)
            {
                if (!_loops[i].Sound.IsEmpty)
                {
                    sound.AddLoop(_loops[i]);
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
            for (int i = 0; i < _effects.Length; i++)
            {
                if (_effects[i].Sound.HasValue) return true;
            }

            for (int i = 0; i < _loops.Length; i++)
            {
                if (_loops[i].Sound.HasValue) return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public override SoundInstance TestInEditor(ISoundPool soundPool)
        {
            AmbienceSoundInstance sound = Fetch(soundPool);

            if (sound != null)
            {
                if (_audioListener == null)
                {
                    _audioListener = FindAnyObjectByType<AudioListener>();
                }

                if (_audioListener != null)
                {
                    sound.Play3D(_audioListener.transform.position);
                }
                else
                {
                    sound.Play2D();
                }
                OnPlayed(sound);

                return sound;
            }

            return null;
        }
#endif

        [ContextMenu("Apply Default Values")]
        public override void ApplyDefaultValues()
        {
            SoundManagerSettings.AmbienceSoundBankData data = SoundManagerSettings.Instance.AmbienceBank;
            _outOfSightVolumeMultiplier = data.OutOfSightVolumeMultiplier;
            _offscreenVolumeMultiplier = data.OffScreenVolumeMultiplier;
            _ignoreListenerSettings = data.IgnoreListenerSettings;
            _dopplerLevel = data.DopplerLevel;
            _defaultDelay = data.DefaultDelay;
            _outputMixer = data.OutputMixer;
        }


    }



    public static class AmbienceSoundBankExtenstions
    {
        /// <summary>
        /// Fetches and then plays a non-spatial sound, ie. one that does not emit from a specific location and rolloff.
        /// </summary>
        /// <param name="soundBank">The sound bank used to set-up the sound instance</param>
        public static AmbienceSoundInstance Play(this AmbienceSoundBank soundBank)
        {
            if (soundBank != null)
            {
                AmbienceSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
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
        public static AmbienceSoundInstance Play(this AmbienceSoundBank soundBank, Vector3 position)
        {
            if (soundBank != null)
            {
                AmbienceSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
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
        public static AmbienceSoundInstance Play(this AmbienceSoundBank soundBank, Transform followTransform)
        {
            if (soundBank != null)
            {
                AmbienceSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
                sound.Play3D(followTransform);
                soundBank.OnPlayed(sound);

                return sound;
            }

            return null;
        }
    }
}