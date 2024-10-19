using System;
using UnityEngine;

namespace SoundManager
{
    [Serializable]
    public class ReverbSoundBankFilter : ComponentSoundBankFilter<AudioReverbFilter>
    {
        [Tooltip("Custom reverb presets, select user to create your own customized reverbs.")]
        public AudioReverbPreset reverbPreset = AudioReverbPreset.User;

        [Range(-10000, 0)]
        [Tooltip("Mix level of dry signal in output in mB. Ranges from –10000.0 to 0.0. Default is 0.")]
        public int dryLevel = 0;

        [Range(-10000, 0)]
        [Tooltip("Room effect level at low frequencies in mB. Ranges from –10000.0 to 0.0. Default is 0.0.")]
        public int room = 0;

        [Range(-10000, 0)]
        [Tooltip("Room effect high-frequency level in mB. Ranges from –10000.0 to 0.0. Default is 0.0.")]
        public int roomHF = 0;

        [Range(-10000, 0)]
        [Tooltip("Room effect low-frequency level in mB. Ranges from –10000.0 to 0.0. Default is 0.0.")]
        public int roomLF = 0;

        [Range(0.1f, 20f)]
        [Tooltip("Reverberation decay time at low-frequencies in seconds. Ranges from 0.1 to 20.0. Default is 1.0.")]
        public float decayTime = 1;

        [Range(0.1f, 2f)]
        [Tooltip("Decay HF Ratio : High-frequency to low-frequency decay time ratio. Ranges from 0.1 to 2.0. Default is 0.5.")]
        public float decayHFRatio = 0.5f;

        [Range(-10000, 1000)]
        [Tooltip("Early reflections level relative to room effect in mB. Ranges from –10000.0 to 1000.0. Default is –10000.0.")]
        public int reflectionsLevel = -10000;

        [Range(0f, 0.3f)]
        [Tooltip("Early reflections delay time relative to room effect in mB. Ranges from 0 to 0.3. Default is 0.0.")]
        public float reflectionsDelay = 0;

        [Range(-10000, 2000)]
        [Tooltip("	Late reverberation level relative to room effect in mB. Ranges from –10000.0 to 2000.0. Default is 0.0.")]
        public int reverbLevel = 0;

        [Range(0, 0.1f)]
        [Tooltip("Late reverberation delay time relative to first reflection in seconds. Ranges from 0.0 to 0.1. Default is 0.04.")]
        public float reverbDelay = 0.04f;

        [Range(1000, 20000)]
        [Tooltip("Reference high frequency in Hz. Ranges from 1000.0 to 20000.0. Default is 5000.0.")]
        public int HFReference = 5000;

        [Range(20, 1000)]
        [Tooltip("Reference low-frequency in Hz. Ranges from 20.0 to 1000.0. Default is 250.0.")]
        public int LFReference = 250;

        [Range(0f, 100f)]
        [Tooltip("Reverberation diffusion (echo density) in percent. Ranges from 0.0 to 100.0. Default is 100.0.")]
        public float diffusion = 100f;

        [Range(0f, 100f)]
        [Tooltip("Reverberation density (modal density) in percent. Ranges from 0.0 to 100.0. Default is 100.0.")]
        public float density = 100f;

        protected ReverbSoundBankFilter()
        {
            Reset();
        }

        public ReverbSoundBankFilter(AudioReverbPreset preset)
        {
            reverbPreset = preset;
            _isEnabled = true;
        }

        public ReverbSoundBankFilter(int dryLevel = 0, int room = 0, int roomHF = 0, int roomLF = 0, float decayTime = 1, float decayHFRatio = 0.5f, int reflectionsLevel = -10000, float reflectionsDelay = 0, int reverbLevel = 0, float reverbDelay = 0.04f, int HFReference = 5000, int LFReference = 250, float diffusion = 100f, float density = 100f)
        {
            reverbPreset = AudioReverbPreset.User;
            this.dryLevel = dryLevel;
            this.room = room;
            this.roomHF = roomHF;
            this.roomLF = roomLF;
            this.decayTime = decayTime;
            this.decayHFRatio = decayHFRatio;
            this.reflectionsLevel = reflectionsLevel;
            this.reflectionsDelay = reflectionsDelay;
            this.reverbLevel = reverbLevel;
            this.reverbDelay = reverbDelay;
            this.HFReference = HFReference;
            this.LFReference = LFReference;
            this.diffusion = diffusion;
            this.density = density;

            _isEnabled = true;
        }

        public override void OnAdded(EffectSoundInstance sound)
        {
            base.OnAdded(sound);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].reverbPreset = reverbPreset;
                _filterComponents[i].dryLevel = dryLevel;
                _filterComponents[i].room = room;
                _filterComponents[i].roomHF = roomHF;
                _filterComponents[i].roomLF = roomLF;
                _filterComponents[i].decayTime = decayTime;
                _filterComponents[i].decayHFRatio = decayHFRatio;
                _filterComponents[i].reflectionsLevel = reflectionsLevel;
                _filterComponents[i].reflectionsDelay = reflectionsDelay;
                _filterComponents[i].reverbLevel = reverbLevel;
                _filterComponents[i].reverbDelay = reverbDelay;
                _filterComponents[i].hfReference = HFReference;
                _filterComponents[i].lfReference = LFReference;
                _filterComponents[i].diffusion = diffusion;
                _filterComponents[i].density = density;
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                _filterComponents[i].reverbPreset = reverbPreset;
                _filterComponents[i].dryLevel = dryLevel;
                _filterComponents[i].room = room;
                _filterComponents[i].roomHF = roomHF;
                _filterComponents[i].roomLF = roomLF;
                _filterComponents[i].decayTime = decayTime;
                _filterComponents[i].decayHFRatio = decayHFRatio;
                _filterComponents[i].reflectionsLevel = reflectionsLevel;
                _filterComponents[i].reflectionsDelay = reflectionsDelay;
                _filterComponents[i].reverbLevel = reverbLevel;
                _filterComponents[i].reverbDelay = reverbDelay;
                _filterComponents[i].hfReference = HFReference;
                _filterComponents[i].lfReference = LFReference;
                _filterComponents[i].diffusion = diffusion;
                _filterComponents[i].density = density;
            }
        }

        public override void Reset()
        {
            base.Reset();

            reverbPreset = AudioReverbPreset.User;
            dryLevel = 0;
            room = 0;
            roomHF = 0;
            roomLF = 0;
            decayTime = 1;
            decayHFRatio = 0.5f;
            reflectionsLevel = -10000;
            reflectionsDelay = 0;
            reverbLevel = 0;
            reverbDelay = 0.04f;
            HFReference = 5000;
            LFReference = 250;
            diffusion = 100f;
            density = 100f;
        }
    }
}
