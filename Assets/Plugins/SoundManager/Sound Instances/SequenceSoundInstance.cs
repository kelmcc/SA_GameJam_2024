using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SoundManager
{
    public class SequenceSoundInstance : MultiSoundInstance
    {

        public bool HasNextSection => _effectiveSectionIndex < GetEffectiveSectionCount() - 1;
        public bool HasPreviousSection => _effectiveSectionIndex > 0;

        private List<SoundInstance> _sectionSounds = new List<SoundInstance>();
        private List<SequenceSoundBank.SectionData> _sectionData = new List<SequenceSoundBank.SectionData>();
        private int _effectiveSectionIndex;


        public override void Reset()
        {
            base.Reset();

            _sectionSounds.Clear();
            _sectionData.Clear();
        }


        public SequenceSoundInstance AddSection(SequenceSoundBank.SectionData sectionData)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");
            Assert.IsNotNull(sectionData);

            SoundInstance sound = sectionData.Sound.Fetch(_soundPool);
            EffectSoundInstance effectSound = sound as EffectSoundInstance;

            if (effectSound != null)
            {
                effectSound.SetLooping(sectionData.Loop);
            }

            sound.SetIgnoreListenerSettings(_ignoreListenerSettings);
            sound.SetOutOfSightVolumeMultiplier(_outOfSightVolumeMultiplier);
            sound.SetOffscreenVolumeMultiplier(_offscreenVolumeMultiplier);
            sound.SetDelay(sectionData.Delay);
            sound.SetAutoDestroy(false);

            if (sectionData.Sound.IsClip)
            {
                effectSound.SetRolloffDistance(sectionData.RolloffDistance.Min, sectionData.RolloffDistance.Max);
            }

            _sectionSounds.Add(sound);
            _sectionData.Add(sectionData);

            RegisterChildSound(sound);

            return this;
        }

        public override void UpdateSound(float deltaTime)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            base.UpdateSound(deltaTime);

            if (_state == State.Playing)
            {
                SoundInstance currentSound = GetCurrentSound();

                if (currentSound.HasFinished)
                {
                    if (HasNextSection)
                    {
                        PlayNextSection();
                    }
                    else
                    {
                        Finish();
                    }
                }
                else if (HasNextSection)
                {
                    SequenceSoundBank.SectionData nextData = GetNextData();
                    float delay = nextData == null ? 0 : nextData.Delay;

                    if (delay < 0)
                    {
                        EffectSoundInstance effectSound = currentSound as EffectSoundInstance;
                        if (effectSound != null && effectSound.TimeRemaining < Mathf.Abs(delay))
                        {
                            _effectiveSectionIndex++;
                            GetCurrentSound().Play(_positionMode, _followTransform, transform.position);
                        }
                    }
                }

                if (!HasFinished)
                {
                    currentSound = GetCurrentSound();
                    if (!currentSound.IsPlaying)
                    {
                        currentSound.Play(_positionMode, _followTransform, transform.position);
                    }

                    currentSound.SetParentMultipliers(_volumeMultiplier * _parentVolumeMultiplier, _pitchMultiplier * _parentPitchMultiplier);
                }

            }

            if (HasFinished && _autoDestroy)
            {
                StopAndDestroy();
            }
        }


        int GetEffectiveSectionCount()
        {
            int count = 0;
            for (int i = 0; i < _sectionData.Count; i++)
            {
                count += _sectionData[i].EffectiveRepititions;
            }

            return count;
        }

        SoundInstance GetCurrentSound()
        {
            int index = 0;
            for (int i = 0; i < _sectionData.Count; i++)
            {
                index += _sectionData[i].EffectiveRepititions;
                if (index > _effectiveSectionIndex)
                {
                    return _sectionSounds[i];
                }
            }

            return null;
        }

        SequenceSoundBank.SectionData GetCurrentData()
        {
            int index = 0;
            for (int i = 0; i < _sectionData.Count; i++)
            {
                index += _sectionData[i].EffectiveRepititions;
                if (index > _effectiveSectionIndex)
                {
                    return _sectionData[i];
                }
            }

            return null;
        }

        SequenceSoundBank.SectionData GetNextData()
        {
            int index = 0;
            for (int i = 0; i < _sectionData.Count; i++)
            {
                index += _sectionData[i].EffectiveRepititions;
                if (index > _effectiveSectionIndex + 1)
                {
                    return _sectionData[i];
                }
            }

            return null;
        }

        public void PlayPreviousSection()
        {
            if (HasPreviousSection)
            {
                SoundInstance currentSound = GetCurrentSound();

                currentSound.Pause();
                currentSound.Rewind();

                _effectiveSectionIndex--;
                GetCurrentSound().Play(_positionMode, _followTransform, transform.position);
            }
        }

        public void PlayNextSection()
        {
            if (HasNextSection)
            {
                SoundInstance currentSound = GetCurrentSound();

                currentSound.Pause();
                currentSound.Rewind();

                _effectiveSectionIndex++;
                GetCurrentSound().Play(_positionMode, _followTransform, transform.position);
            }
        }

        public override string GetGUIStatusString()
        {
            SoundInstance currentSound = GetCurrentSound();

            if (currentSound == null)
            {
                return _effectiveSectionIndex >= GetEffectiveSectionCount() ? "Finished" : "No sections";
            }

            return GetCurrentData().Sound.GetName() + " (" + (_effectiveSectionIndex + 1) + "/" + GetEffectiveSectionCount() + ")";

        }

    }
}
