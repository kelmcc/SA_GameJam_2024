using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace SoundManager
{
    public class BlendSoundInstance : MultiSoundInstance
    {

        public float BlendParameter
        {
            get => _blendParameter;
            set => SetBlendParameter(value);
        }
        public ReadOnlyCollection<SoundInstance> Layers => _layerSounds.AsReadOnly();
        public ReadOnlyCollection<BlendSoundBank.LayerData> LayerData => _layerData.AsReadOnly();
        public int NumLayers => _layerSounds.Count;

        private List<SoundInstance> _layerSounds = new List<SoundInstance>();
        private List<BlendSoundBank.LayerData> _layerData = new List<BlendSoundBank.LayerData>();

        private float _blendParameter;

        public override void Reset()
        {
            base.Reset();

            _blendParameter = 0f;
            _layerSounds.Clear();
            _layerData.Clear();

        }

        public override string GetGUIStatusString()
        {
            if (_layerSounds.Count == 0)
            {
                return "No layers";
            }

            StringBuilder status = new StringBuilder();

            for (int i = 0; i < _layerSounds.Count; i++)
            {
                status.AppendLine(_layerData[i].Sound.GetName() + " (" + (_layerData[i].BlendCurve.Evaluate(_blendParameter) * 100f).ToString("F0") + "%)");
            }

            return status.ToString();
        }

        public BlendSoundInstance AddLayer(BlendSoundBank.LayerData layerData)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");
            Assert.IsNotNull(layerData);

            SoundInstance sound = layerData.Sound.Fetch(_soundPool);
            EffectSoundInstance effectSound = sound as EffectSoundInstance;

            if (effectSound != null)
            {
                effectSound.SetLooping(layerData.Loop);
            }

            sound.SetIgnoreListenerSettings(_ignoreListenerSettings);
            sound.SetOutOfSightVolumeMultiplier(_outOfSightVolumeMultiplier);
            sound.SetOffscreenVolumeMultiplier(_offscreenVolumeMultiplier);
            sound.SetDelay(_delayDuration);
            sound.SetAutoDestroy(false);

            if (layerData.Sound.IsClip)
            {
                effectSound.SetRolloffDistance(layerData.RolloffDistance.Min, layerData.RolloffDistance.Max);
            }

            _layerSounds.Add(sound);
            _layerData.Add(layerData);

            RegisterChildSound(sound);

            return this;
        }

        public BlendSoundInstance SetBlendParameter(float value)
        {
            _blendParameter = Mathf.Clamp01(value);

            return this;
        }

        public override void UpdateSound(float deltaTime)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            base.UpdateSound(deltaTime);


            if (_state == State.Playing)
            {
                bool finished = true;
                for (int i = 0; i < _layerSounds.Count; i++)
                {
                    if (!_layerSounds[i].IsPlaying)
                    {
                        _layerSounds[i].Play(_positionMode, _followTransform, transform.position);
                        _layerSounds[i].SetPitchMultiplier(_pitchMultiplier);
                    }

                    _layerSounds[i].SetParentMultipliers(_layerData[i].BlendCurve.Evaluate(_blendParameter) * _volumeMultiplier * _parentVolumeMultiplier, _pitchMultiplier * _parentPitchMultiplier);

                    if (!_layerSounds[i].HasFinished)
                    {
                        finished = false;
                    }
                }

                if (finished)
                {
                    Finish();
                }
            }

            if (HasFinished && _autoDestroy)
            {
                StopAndDestroy();
            }

        }

    }
}
