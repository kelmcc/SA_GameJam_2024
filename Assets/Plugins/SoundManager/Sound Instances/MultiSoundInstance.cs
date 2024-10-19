using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace SoundManager
{
    public abstract class MultiSoundInstance : SoundInstance
    {

        protected List<SoundInstance> _childSounds = new List<SoundInstance>();

        protected void RegisterChildSound(SoundInstance sound)
        {
            _childSounds.Add(sound);
        }

        protected void DeregisterChildSound(SoundInstance sound)
        {
            _childSounds.Remove(sound);
        }

        public override void Reset()
        {
            base.Reset();

            _childSounds.Clear();
        }

        public override void Resume()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            for (int i = 0; i < _childSounds.Count; i++)
            {
                _childSounds[i].Resume();
            }

            _state = State.Playing;
        }

        public override void Rewind()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            base.Rewind();

            for (int i = 0; i < _childSounds.Count; i++)
            {
                _childSounds[i].Rewind();
            }
        }


        public override void Pause()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            for (int i = 0; i < _childSounds.Count; i++)
            {
                _childSounds[i].Pause();
            }

            _state = State.Paused;
        }

        public override SoundInstance SetPersistant(bool persistant)
        {
            base.SetPersistant(persistant);

            for (int i = 0; i < _childSounds.Count; i++)
            {
                _childSounds[i].SetPersistant(persistant);
            }

            return this;
        }

        public override SoundInstance SetMixerGroup(AudioMixerGroup mixerGroup)
        {
            base.SetMixerGroup(mixerGroup);

            for (int i = 0; i < _childSounds.Count; i++)
            {
                _childSounds[i].SetMixerGroup(mixerGroup);
            }

            return this;
        }

        public override SoundInstance SetDelay(float delayDuration)
        {
            base.SetDelay(Mathf.Max(0, delayDuration));

            for (int i = 0; i < _childSounds.Count; i++)
            {
                _childSounds[i].SetDelay(Mathf.Max(0, delayDuration));
            }

            return this;
        }

        public override void AddFilter(SoundBankFilter filter)
        {
            base.AddFilter(filter);

            if (filter.IsEnabled)
            {
                for (int i = 0; i < _childSounds.Count; i++)
                {
                    _childSounds[i].AddFilter(filter);
                }
            }
        }

        public override SoundInstance StopAndDestroy()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            for (int i = 0; i < _childSounds.Count; i++)
            {
                _childSounds[i].StopAndDestroy();
            }

            return base.StopAndDestroy();
        }
    }
}
