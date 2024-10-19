using System;
using Framework;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundManager
{
    public class SoundManagerSettings : ProjectSettingsObject<SoundManagerSettings>
    {
        public static bool LogSounds;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            LogSounds = false;
        }

        [Serializable]
        public class EffectSoundBankData
        {
            public bool IgnoreListenerSettings;
            [MinValue(0)]
            public float OutOfSightVolumeMultiplier = 1f;
            [MinValue(0)]
            public float OffScreenVolumeMultiplier = 1f;
            [Clamp(0f, 5f)]
            public float DopplerLevel = 1f;
            [MinValue(0)]
            public float DefaultDelay;
            [Clamp]
            public FloatRange VolumeRange = new FloatRange(0.5f, 0.5f);
            public FloatRange PitchRange = new FloatRange(1f, 1f);
            [MinValue(0)]
            public FloatRange RolloffDistance = new FloatRange(1, 10);
            [MinValue(0)]
            public float Cooldown = 0;
            public AudioMixerGroup OutputMixer;
            public SingleSoundBank.ClipSelectionMode ClipSelection = SingleSoundBank.ClipSelectionMode.Random;

            [Space]
            public int EditorPoolSize = 15;
            public int BuildPoolSize = 50;
        }

        [Serializable]
        public class ImpactSoundBankData
        {
            public bool IgnoreListenerSettings;
            [MinValue(0)]
            public float OutOfSightVolumeMultiplier = 1f;
            [MinValue(0)]
            public float OffScreenVolumeMultiplier = 1f;
            [Clamp(0f, 5f)]
            public float DopplerLevel = 1f;
            [MinValue(0)]
            public float DefaultDelay;
            [Clamp]
            public FloatRange VolumeRange = new FloatRange(0.5f, 0.5f);
            public FloatRange PitchRange = new FloatRange(1f, 1f);
            [MinValue(0)]
            public FloatRange RolloffDistance = new FloatRange(1, 10);
            [MinValue(0)]
            public FloatRange VelocityRange = new FloatRange(2f, 5f);
            [MinValue(0)]
            public float Cooldown = 0;
            public AudioMixerGroup OutputMixer;
            public SingleSoundBank.ClipSelectionMode ClipSelection = SingleSoundBank.ClipSelectionMode.Random;

            [Space]
            public int EditorPoolSize = 15;
            public int BuildPoolSize = 50;
        }

        [Serializable]
        public class AmbienceSoundBankData
        {
            public bool IgnoreListenerSettings;
            [MinValue(0)]
            public float OutOfSightVolumeMultiplier = 1f;
            [MinValue(0)]
            public float OffScreenVolumeMultiplier = 1f;
            [Clamp(0f, 5f)]
            public float DopplerLevel = 1f;
            [MinValue(0)]
            public float DefaultDelay;
            public AudioMixerGroup OutputMixer;
            public AmbienceSoundBank.EffectData.SpatialMode EffectSpawnMode = AmbienceSoundBank.EffectData.SpatialMode.XZHemisphere;
            [MinValue(0)]
            public FloatRange EffectSpawnDistance = new FloatRange(5, 10);
            [MinValue(0)]
            public FloatRange EffectCooldown = new FloatRange(1f, 5f);
            [MinValue(0)]
            public FloatRange LoopRolloffDistance = new FloatRange(1, 10);
            [Clamp]
            public FloatRange LoopVariationVolumeRange = new FloatRange(0.5f, 0.5f);
            public float LoopVaritaionFrequency = 0.1f;

            [Space]
            public int EditorPoolSize = 3;
            public int BuildPoolSize = 10;
        }


        [Serializable]
        public class BlendSoundBankData
        {
            public bool IgnoreListenerSettings;
            [MinValue(0)]
            public float OutOfSightVolumeMultiplier = 1f;
            [MinValue(0)]
            public float OffScreenVolumeMultiplier = 1f;
            [Clamp(0f, 5f)]
            public float DopplerLevel = 1f;
            [MinValue(0)]
            public float DefaultDelay;
            public AudioMixerGroup OutputMixer;
            [MinValue(0)]
            public FloatRange LayerRolloffDistance = new FloatRange(1, 10);
            public AnimationCurve LayerBlendCurve = AnimationCurve.Linear(0, 0, 1, 1);
            public bool LayerLooping = false;

            [Space]
            public int EditorPoolSize = 3;
            public int BuildPoolSize = 10;
        }


        [Serializable]
        public class SequenceSoundBankData
        {
            public bool IgnoreListenerSettings;
            [MinValue(0)]
            public float OutOfSightVolumeMultiplier = 1f;
            [MinValue(0)]
            public float OffScreenVolumeMultiplier = 1f;
            [Clamp(0f, 5f)]
            public float DopplerLevel = 1f;
            [MinValue(0)]
            public float DefaultDelay;
            public AudioMixerGroup OutputMixer;
            [MinValue(0)]
            public FloatRange SectionRolloffDistance = new FloatRange(1, 10);
            public bool SectionLooping = false;
            [MinValue(1)]
            public int SectionRepititions = 1;
            public float SectionDelay = 0;

            [Space]
            public int EditorPoolSize = 3;
            public int BuildPoolSize = 10;
        }


        public EffectSoundBankData EffectBank => _effectBank;
        public ImpactSoundBankData ImpactBank => _impactBank;
        public AmbienceSoundBankData AmbienceBank => _ambienceBank;
        public BlendSoundBankData BlendBank => _blendBank;
        public SequenceSoundBankData SequenceBank => _sequenceBank;
        public LayerMask LayersThatBlockSoundLOS => _layersThatBlockSoundLOS;
        public float OffscreenVolumeTransitionSpeed => _offscreenVolumeTransitionSpeed;
        public float OutOfSightVolumeTransitionSpeed => _outOfSightVolumeTransitionSpeed;

        [SerializeField, Tooltip("These layers will be raycast against to determine whether an AudioSource is considered in LOS of the listener or not.")]
        private LayerMask _layersThatBlockSoundLOS = ~0;

        [SerializeField, Tooltip("How fast the volume should adjust when an AudioSource changes LOS status. 1 speed = 1 seconds to change from 0 to 1 volume. Always a linear transition.")]
        private float _outOfSightVolumeTransitionSpeed = 1f;

        [SerializeField, Tooltip("How fast the volume should adjust when an AudioSource changes offscreen status. 1 speed = 1 seconds to change from 0 to 1 volume. Always a linear transition.")]
        private float _offscreenVolumeTransitionSpeed = 1f;

        [Header("Default Bank Settings")]
        [SerializeField]
        private EffectSoundBankData _effectBank;

        [SerializeField]
        private ImpactSoundBankData _impactBank;

        [SerializeField]
        private AmbienceSoundBankData _ambienceBank;

        [SerializeField]
        private BlendSoundBankData _blendBank;

        [SerializeField]
        private SequenceSoundBankData _sequenceBank;

#if UNITY_EDITOR
        [UnityEditor.SettingsProvider]
        public static UnityEditor.SettingsProvider CreateSettingsProvider()
        {
            return CreateSettingsProviderInternal();
        }
#endif
    }
}
