using Framework;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace SoundManager
{
    public abstract class SingleSoundBank : SoundBank
    {
        public enum ClipSelectionMode
        {
            Random,
            Shuffle,
            ClampedSequence,
            LoopingSequence
        }

        public float Cooldown => _cooldown;
        public AudioClip[] Clips => _audioClips;
        public FloatRange VolumeRange => _volumeRange;
        public FloatRange PitchRange => _pitchRange;
        public FloatRange RolloffDistance => _rolloffDistance;
        public AudioMixerGroup OutputMixer => _outputMixer;
        public ClipSelectionMode ClipSelection => _clipSelection;

        [SerializeField]
        [Clamp]
        [Tooltip("The sound will play at a random volume level between these two values.")]
        protected FloatRange _volumeRange = new FloatRange(0.5f, 0.5f);

        [SerializeField]
        [Tooltip("The sound will play at a random pitch between these two values. (1 = normal pitch).")]
        protected FloatRange _pitchRange = new FloatRange(1f, 1f);

        [SerializeField]
        [Clamp(0, Mathf.Infinity)]
        [Tooltip("Controls the attenuation of the sound as the listener moves away from the source.\n\nMin: Distance at which sound is still 100% volume.\nMax: Distance at which sound is now 0% volume.")]
        protected FloatRange _rolloffDistance = new FloatRange(1f, 100f);

        [SerializeField]
        [MinValue(0)]
        [Tooltip("The minimum amount of time (in seconds) that must pass before this sound is allowed to play again.")]
        protected float _cooldown;

        [SerializeField]
        [Tooltip("The audio mixer group that the audio source will ouput to when playing.")]
        protected AudioMixerGroup _outputMixer;

        [SerializeField]
        [Tooltip("The method for choosing a clip when the sound is played\n\n- Random: Completely random\n- Shuffle: Guaranteed that every clip will play before one is repeated.\n- Clamped Sequence: Play in order. Keep repeating the last clip after every clip has played.\n- Looping Sequence: Play in order. Start from the beginning once every clip has played.")]
        protected ClipSelectionMode _clipSelection;

        [SerializeField]
        [Tooltip("The array of audio clips to choose from.")]
        protected AudioClip[] _audioClips;

        private ShuffleBag<AudioClip> _shuffleBag;
        private int _clipIndex;



        public virtual AudioClip GetNextClip()
        {
            AudioClip clip = null;
            bool hasValidClip = false;

            for (int i = 0; i < _audioClips.Length; i++)
            {
                if (_audioClips[i] != null)
                {
                    hasValidClip = true;
                    break;
                }
            }

            if (hasValidClip)
            {
                while (clip == null)
                {
                    clip = ChooseClip(_audioClips);
                }
            }

            return clip;
        }

        protected virtual AudioClip ChooseClip(AudioClip[] clips)
        {
            switch (_clipSelection)
            {
                case ClipSelectionMode.Random:
                    return RandomUtils.Choose(clips);

                case ClipSelectionMode.Shuffle:
                    if (_shuffleBag == null)
                    {
                        _shuffleBag = new ShuffleBag<AudioClip>(clips);
                    }

                    return _shuffleBag.GetNext();

                case ClipSelectionMode.ClampedSequence:
                    return clips[Mathf.Min(_clipIndex++, clips.Length - 1)];

                case ClipSelectionMode.LoopingSequence:
                    return clips[MathUtils.WrapIndex(_clipIndex++, clips.Length)];
            }

            return null;
        }

        /// <summary>
        /// Sets the clips in this bank.
        /// </summary>
        /// <param name="audioClips">The new clips to be associated with this bank</param>
        public void SetClips(AudioClip[] audioClips)
        {
            Assert.IsNotNull(audioClips);

            _audioClips = audioClips;
        }

#if UNITY_EDITOR

        void OnValidate()
        {
            _shuffleBag = null;
            _clipIndex = 0;
        }

        protected override void ResetRuntimeValues()
        {
            base.ResetRuntimeValues();

            _shuffleBag = null;
            _clipIndex = 0;
        }
#endif

    }
}
