using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace SoundManager
{
    public abstract class SoundInstance : MonoBehaviour
    {
        protected internal enum State
        {
            Pooled,
            Fetched,
            Playing,
            Paused
        }

        protected internal enum PositionMode
        {
            TwoDimensional,
            FixedPosition,
            FollowingTransform
        }

        /// <summary>
        /// A mulitplier value that can be used to affect the final playing volume of the sound.
        /// </summary>
        public float VolumeMultiplier
        {
            get => _volumeMultiplier;
            set => SetVolumeMultiplier(value);
        }
        /// <summary>
        /// A mulitplier value that can be used to affect the final playing pitch of the sound.
        /// </summary>
        public float PitchMultiplier
        {
            get => _pitchMultiplier;
            set => SetPitchMultiplier(value);
        }

        public float DopplerLevel
        {
            get => _dopplerLevel;
            set => SetDopplerLevel(value);
        }

        public float Spread
        {
            get => _spread;
            set => SetSpread(value);
        }

        public bool HasFinished => _hasFinished;
        public bool IsFadingIn => _fadeSpeed > 0;
        public bool IsFadingOut => _fadeSpeed < 0;
        public bool IsDelayed => _delayTimer > 0f;

        /// <summary>
        /// Whether or not this sound is currently residing in the pool.
        /// </summary>
        public bool IsPooled => _state == State.Pooled;

        /// <summary>
        /// Whether or not the sound is currently playing. Will return false if the sound is waiting for a delayed play.
        /// </summary>
        public bool IsPlaying => _state == State.Playing;

        /// <summary>
        /// Whether or not the sound is currently paused.
        /// </summary>
        public bool IsPaused => _state == State.Paused;

        public bool IgnoreListenerSettings
        {
            get => _ignoreListenerSettings;
            set => SetIgnoreListenerSettings(value);
        }

        public float OutOfSightVolumeMultiplier
        {
            get => _outOfSightVolumeMultiplier;
            set => SetOutOfSightVolumeMultiplier(value);
        }

        public float OffscreenVolumeMultiplier
        {
            get => _offscreenVolumeMultiplier;
            set => SetOffscreenVolumeMultiplier(value);
        }

        public bool IsPersistant
        {
            get => _isPersistant;
            set => SetPersistant(value);
        }

        public AudioMixerGroup MixerGroup
        {
            get => _mixerGroup;
            set => SetMixerGroup(value);
        }

        public bool AutoDestroy
        {
            get => _autoDestroy;
            set => SetAutoDestroy(value);
        }



        public float CurrentDelay
        {
            get => _delayTimer;
            set => SetDelay(value);
        }

        protected bool _ignoreListenerSettings;
        protected float _pitchMultiplier = 1f;
        protected float _volumeMultiplier = 1f;
        protected float _fadeVolumeMulitplier = 1f;
        protected float _parentPitchMultiplier = 1f;
        protected float _parentVolumeMultiplier = 1f;
        protected float _outOfSightVolumeMultiplier = 1f;
        protected float _offscreenVolumeMultiplier = 1f;
        protected float _fadeSpeed = 0f;
        protected float _dopplerLevel = 1f;
        protected float _spread = 0f;
        protected Transform _followTransform;
        protected PositionMode _positionMode;
        protected bool _isPersistant;
        protected AudioMixerGroup _mixerGroup;
        protected ISoundPool _soundPool;
        protected Action _onFinish;
        protected State _state;
        protected bool _autoDestroy = true;
        protected bool _destroyAtEndOfFade = false;
        protected SoundInstance _parentSound;
        protected List<SoundBankFilter> _filters = new List<SoundBankFilter>();
        protected float _delayTimer;
        protected float _delayDuration;
        private bool _hasFinished;


        public virtual void Reset()
        {
            _delayDuration = 0f;
            _delayTimer = 0f;
            _pitchMultiplier = 1f;
            _volumeMultiplier = 1f;
            _fadeVolumeMulitplier = 1f;
            _parentPitchMultiplier = 1f;
            _parentVolumeMultiplier = 1f;
            _offscreenVolumeMultiplier = 1f;
            _outOfSightVolumeMultiplier = 1f;
            _fadeSpeed = 0f;
            _onFinish = null;
            _isPersistant = false;
            _hasFinished = false;
            _followTransform = null;
            _mixerGroup = null;
            _autoDestroy = true;
            _destroyAtEndOfFade = false;
            _positionMode = PositionMode.TwoDimensional;
            _ignoreListenerSettings = false;
            _dopplerLevel = 1f;
            _spread = 0f;

            transform.position = Vector3.zero;

            RemoveFilters();
        }

        public void SetParentMultipliers(float volumeMultiplier, float pitchMultiplier)
        {
            _parentVolumeMultiplier = volumeMultiplier;
            _parentPitchMultiplier = pitchMultiplier;
        }

        public virtual void AddFilter(SoundBankFilter filter)
        {
            if (filter.IsEnabled)
            {
                _filters.Add(filter);
            }
        }

        public virtual void RemoveFilters()
        {
            _filters.Clear();
        }

        public void OnFinish(Action callback)
        {
            _onFinish += callback;
        }

        protected void Finish()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            if (!_hasFinished)
            {
                _hasFinished = true;

                if (_onFinish != null)
                {
                    _onFinish();
                }
            }
        }

        protected internal void Play(PositionMode mode, Transform followTransform, Vector3 position)
        {
            switch (mode)
            {
                case PositionMode.TwoDimensional:
                    Play2D();
                    break;
                case PositionMode.FixedPosition:
                    Play3D(position);
                    break;
                case PositionMode.FollowingTransform:
                    Play3D(followTransform);
                    break;
            }
        }

        public void Play3D(Vector3 position)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            transform.position = position;
            _positionMode = PositionMode.FixedPosition;
            _state = State.Playing;
        }

        public void Play3D(Transform followTransform)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");
            Assert.IsNotNull(followTransform);

            _followTransform = followTransform;
            _positionMode = PositionMode.FollowingTransform;
            _state = State.Playing;
        }

        public void Play2D()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _positionMode = PositionMode.TwoDimensional;
            _state = State.Playing;
        }

        public virtual void Rewind()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _hasFinished = false;
        }

        protected virtual void LateUpdate()
        {
            float deltaTime = Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime);
            UpdateSound((_ignoreListenerSettings || !AudioListener.pause) ? deltaTime : 0f);
        }

        public virtual void UpdateSound(float deltaTime)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");
            Assert.IsTrue(deltaTime >= 0);

            if (_fadeSpeed != 0f && _destroyAtEndOfFade && (_fadeVolumeMulitplier + _fadeSpeed * deltaTime > 1f || _fadeVolumeMulitplier + _fadeSpeed * deltaTime < 0f))
            {
                StopAndDestroy();
            }
            else
            {
                for (int i = 0; i < _filters.Count; i++)
                {
                    _filters[i].Update(deltaTime);
                }

                _fadeVolumeMulitplier = Mathf.Clamp01(_fadeVolumeMulitplier + _fadeSpeed * deltaTime);
            }
        }

        public virtual SoundInstance SetPositionModeFixed(Vector3 position)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _positionMode = PositionMode.FixedPosition;
            transform.position = position;

            return this;
        }

        public virtual SoundInstance SetPositionModeFollow(Transform followTransform)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _positionMode = PositionMode.FollowingTransform;
            _followTransform = followTransform;

            return this;
        }

        public virtual SoundInstance SetPositionMode2D()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _positionMode = PositionMode.TwoDimensional;

            return this;
        }

        public virtual SoundInstance SetPersistant(bool persistant)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _isPersistant = persistant;

            return this;
        }

        public virtual SoundInstance SetIgnoreListenerSettings(bool ignore)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _ignoreListenerSettings = ignore;

            return this;
        }

        public virtual SoundInstance SetOutOfSightVolumeMultiplier(float multiplier)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _outOfSightVolumeMultiplier = Mathf.Max(0, multiplier);

            return this;
        }

        public virtual SoundInstance SetDopplerLevel(float level)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _dopplerLevel = level;

            return this;
        }

        public virtual SoundInstance SetSpread(float spread)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _spread = spread;

            return this;
        }


        public virtual SoundInstance SetMixerGroup(AudioMixerGroup mixerGroup)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _mixerGroup = mixerGroup;

            return this;
        }

        public virtual SoundInstance SetAutoDestroy(bool autoDestroy)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _autoDestroy = autoDestroy;

            return this;
        }

        public virtual SoundInstance FadeOut(float fadeDuration, bool startFromOne)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            if (fadeDuration == 0)
            {
                _fadeVolumeMulitplier = 0;
            }
            else
            {
                if (startFromOne)
                {
                    _fadeVolumeMulitplier = 1f;
                }

                _fadeSpeed = -1f / Mathf.Max(0, fadeDuration);
            }

            return this;
        }

        public virtual SoundInstance FadeIn(float fadeDuration, bool startFromZero)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            if (fadeDuration == 0)
            {
                _fadeVolumeMulitplier = 1f;
            }
            else
            {
                if (startFromZero)
                {
                    _fadeVolumeMulitplier = 0f;
                }

                _fadeSpeed = 1f / Mathf.Max(0, fadeDuration);
            }

            return this;
        }

        public virtual SoundInstance FadeOutAndDestroy(float fadeDuration, bool startFromOne)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _destroyAtEndOfFade = true;
            FadeOut(fadeDuration, startFromOne);

            return this;
        }

        public virtual SoundInstance FadeInAndDestroy(float fadeDuration, bool startFromZero)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _destroyAtEndOfFade = true;
            FadeIn(fadeDuration, startFromZero);

            return this;
        }

        public virtual SoundInstance StopFading(bool resetFadeVolume)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _destroyAtEndOfFade = false;
            _fadeSpeed = 0f;

            if (resetFadeVolume)
            {
                _fadeVolumeMulitplier = 1f;
            }

            return this;
        }

        /// <summary>
        /// Sets the volume multiplier, the base volume will be multiplied by this number when determining the final output volume.
        /// </summary>
        /// <param name="multiplier">The volume multiplier value</param>
        public virtual SoundInstance SetVolumeMultiplier(float multiplier)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _volumeMultiplier = Mathf.Max(0, multiplier);

            return this;
        }

        /// <summary>
        /// Sets the pitch multiplier, the base pitch level will be multiplied by this number when determining the final output pitch.
        /// </summary>
        /// <param name="multiplier">The pitch multiplier value</param>
        public virtual SoundInstance SetPitchMultiplier(float multiplier)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _pitchMultiplier = Mathf.Max(0, multiplier);

            return this;
        }

        public virtual SoundInstance SetOffscreenVolumeMultiplier(float multiplier)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _offscreenVolumeMultiplier = Mathf.Max(0, multiplier);

            return this;
        }

        public virtual SoundInstance SetDelay(float delayDuration)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _delayTimer = Mathf.Max(0, delayDuration);
            _delayDuration = Mathf.Max(0, delayDuration);

            return this;
        }

        /// <summary>
        /// Callback for when this sound instance has just been fetched from the pool. This should only ever be called by the SoundPool.
        /// </summary>
        public virtual void OnFetchedFromPool(ISoundPool soundPool)
        {
            Assert.IsTrue(IsPooled);

            Reset();

            _soundPool = soundPool;
            _state = State.Fetched;
        }

        /// <summary>
        /// Callback for when this sound instance has just been returned to the pool. This should only ever be called by the SoundPool.
        /// </summary>
        public virtual void OnReturnedToPool()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            Reset();

            _soundPool = null;
            _state = State.Pooled;
        }


        public virtual SoundInstance StopAndDestroy()
        {
            _soundPool.ReturnToPool(this);

            return null;
        }

        public abstract void Resume();
        public abstract void Pause();
        public abstract string GetGUIStatusString();

        public override string ToString()
        {
            switch (_positionMode)
            {
                case PositionMode.TwoDimensional:
                    return "Position: 2D";
                case PositionMode.FixedPosition:
                    return "Position: " + transform.position;
                case PositionMode.FollowingTransform:
                    return "Following: " + _followTransform.name;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
