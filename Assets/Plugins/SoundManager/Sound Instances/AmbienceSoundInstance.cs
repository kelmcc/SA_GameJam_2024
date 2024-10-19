using System.Collections.Generic;
using System.Text;
using Framework;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace SoundManager
{
    public class AmbienceSoundInstance : MultiSoundInstance
    {

        private List<SoundInstance> _effectSounds = new List<SoundInstance>();
        private List<SoundInstance> _loopSounds = new List<SoundInstance>();
        private List<AmbienceSoundBank.EffectData> _effectData = new List<AmbienceSoundBank.EffectData>();
        private List<AmbienceSoundBank.LoopData> _loopData = new List<AmbienceSoundBank.LoopData>();
        private List<float> _loopSeeds = new List<float>();
        private List<float> _effectCooldowns = new List<float>();

        public override void Reset()
        {
            base.Reset();

            _effectSounds.Clear();
            _loopSounds.Clear();
            _effectData.Clear();
            _loopData.Clear();
            _loopSeeds.Clear();
            _effectCooldowns.Clear();
        }


        public AmbienceSoundInstance AddLoop(AmbienceSoundBank.LoopData loopData)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");
            Assert.IsNotNull(loopData);

            SoundInstance loop = loopData.Sound.Fetch(_soundPool);
            EffectSoundInstance effectLoop = loop as EffectSoundInstance;

            if (effectLoop != null)
            {
                effectLoop.SetLooping(true);
            }

            loop.SetIgnoreListenerSettings(_ignoreListenerSettings);
            loop.SetOutOfSightVolumeMultiplier(_outOfSightVolumeMultiplier);
            loop.SetOffscreenVolumeMultiplier(_offscreenVolumeMultiplier);
            loop.SetDelay(_delayDuration);
            loop.SetAutoDestroy(false);

            if (loopData.Sound.IsClip)
            {
                effectLoop.SetRolloffDistance(loopData.RolloffDistance.Min, loopData.RolloffDistance.Max);
            }

            _loopSounds.Add(loop);
            _loopData.Add(loopData);
            _loopSeeds.Add(Random.value);

            RegisterChildSound(loop);

            return this;
        }

        public AmbienceSoundInstance AddEffect(AmbienceSoundBank.EffectData effectData)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");
            Assert.IsNotNull(effectData);

            _effectData.Add(effectData);
            _effectCooldowns.Add(effectData.Cooldown.ChooseRandom());

            return this;
        }


        public override void UpdateSound(float deltaTime)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            base.UpdateSound(deltaTime);

            if (_state == State.Playing)
            {
                for (int i = 0; i < _loopSounds.Count; i++)
                {
                    if (!_loopSounds[i].IsPlaying)
                    {
                        _loopSounds[i].Play(_positionMode, _followTransform, transform.position);
                    }

                    _loopSeeds[i] += deltaTime * _loopData[i].VaritaionFrequency;
                    _loopSounds[i].SetParentMultipliers(_loopData[i].VolumeVariationRange.GetValue(Noise.GetNormalizedPerlin(_loopSeeds[i])) * _volumeMultiplier * _parentVolumeMultiplier, _pitchMultiplier * _parentPitchMultiplier);

                }

                for (int i = 0; i < _effectData.Count; i++)
                {
                    _effectCooldowns[i] -= deltaTime;

                    if (_effectCooldowns[i] <= 0)
                    {
                        SoundInstance effect = _effectData[i].Sound.Fetch(_soundPool);

                        effect.SetIgnoreListenerSettings(_ignoreListenerSettings);
                        effect.SetOutOfSightVolumeMultiplier(_outOfSightVolumeMultiplier);
                        effect.SetOffscreenVolumeMultiplier(_offscreenVolumeMultiplier);
                        effect.SetDelay(_delayDuration);
                        effect.SetParentMultipliers(_volumeMultiplier * _parentVolumeMultiplier, _pitchMultiplier * _parentPitchMultiplier);

                        for (int j = 0; j < _filters.Count; j++)
                        {
                            effect.AddFilter(_filters[j]);
                        }

                        if (_mixerGroup != null)
                        {
                            effect.SetMixerGroup(_mixerGroup);
                        }

                        if (_effectData[i].Sound.IsClip)
                        {
                            (effect as EffectSoundInstance).SetRolloffDistance(_effectData[i].SpawnDistance.Min, _effectData[i].SpawnDistance.Max);
                        }

                        switch (_positionMode)
                        {
                            case PositionMode.TwoDimensional:
                                effect.Play(PositionMode.TwoDimensional, null, Vector3.zero);
                                break;
                            case PositionMode.FixedPosition:
                                effect.Play(PositionMode.FixedPosition, null, GetEffectPosition(_effectData[i], transform.position));
                                break;
                            case PositionMode.FollowingTransform:
                                effect.Play(PositionMode.FixedPosition, null, GetEffectPosition(_effectData[i], _followTransform.position));
                                break;
                        }

                        _effectCooldowns[i] = _effectData[i].Cooldown.ChooseRandom();
                        _effectSounds.Add(effect);

                        RegisterChildSound(effect);

                        effect.OnFinish(() => DeregisterEffect(effect));
                    }
                }
            }

            if (HasFinished && _autoDestroy)
            {
                StopAndDestroy();
            }
        }

        Vector3 GetEffectPosition(AmbienceSoundBank.EffectData effectData, Vector3 center)
        {
            Vector3 offset = Vector3.zero;

            switch (effectData.SpawnMode)
            {
                case AmbienceSoundBank.EffectData.SpatialMode.XZHemisphere:
                    offset = Random.insideUnitSphere * effectData.SpawnDistance.ChooseRandom();
                    offset.y = Mathf.Abs(offset.y);
                    break;

                case AmbienceSoundBank.EffectData.SpatialMode.XZCircle:
                    offset = RandomUtils.InsideUnitCircleOnPlane(Vector3.up) * effectData.SpawnDistance.ChooseRandom();
                    break;

                case AmbienceSoundBank.EffectData.SpatialMode.XYCircle:
                    offset = RandomUtils.InsideUnitCircleOnPlane(Vector3.forward) * effectData.SpawnDistance.ChooseRandom();
                    break;

                case AmbienceSoundBank.EffectData.SpatialMode.Sphere:
                    offset = Random.insideUnitSphere * effectData.SpawnDistance.ChooseRandom();
                    break;
            }

            return center + offset;
        }

        void DeregisterEffect(SoundInstance effect)
        {
            DeregisterChildSound(effect);
            _effectSounds.Remove(effect);
        }

        public override string GetGUIStatusString()
        {
            if (_effectSounds.Count == 0 && _loopSounds.Count == 0)
            {
                return "No effects or loops";
            }

            StringBuilder status = new StringBuilder();

            for (int i = 0; i < _loopSounds.Count; i++)
            {
                status.AppendLine("Loop: " + _loopData[i].Sound.GetName() + " (" + Noise.GetNormalizedPerlin(_loopSeeds[i]).ToString("F2") + "%)");
            }

            for (int i = 0; i < _effectSounds.Count; i++)
            {
                status.AppendLine("Effect: " + _effectData[i].Sound.GetName() + " (Distance: " + _effectSounds[i].transform.position.To(transform.position).magnitude.ToString("F2") + ")");
            }

            return status.ToString();
        }
    }
}
