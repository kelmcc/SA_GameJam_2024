using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace SoundManager
{


    public interface ISoundPool
    {
        T FetchFromPool<T>() where T : SoundInstance;
        void ReturnToPool<T>(T sound) where T : SoundInstance;
    }

    /// <summary>
    /// Auto-singleton object responsible for maintaining the pool of SoundInstances at runtime.
    /// </summary>
    public class RuntimeSoundPool : AutoSingletonBehaviour<RuntimeSoundPool>, ISoundPool
    {

        private TypeInstanceCollection<SoundInstance> _inactiveInstances = new TypeInstanceCollection<SoundInstance>();
        private List<SoundInstance> _activeInstances = new List<SoundInstance>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            EnsureInstanceExists();
        }

        static void OnSceneUnloaded(Scene scene)
        {
            Instance.StopNonPersistantSounds();
        }

        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);


            // Initialize pools
#if UNITY_EDITOR
            for (int i = 0; i < SoundManagerSettings.Instance.EffectBank.EditorPoolSize; i++) ExpandPool<EffectSoundInstance>();
            for (int i = 0; i < SoundManagerSettings.Instance.ImpactBank.EditorPoolSize; i++) ExpandPool<ImpactSoundInstance>();
            for (int i = 0; i < SoundManagerSettings.Instance.AmbienceBank.EditorPoolSize; i++) ExpandPool<AmbienceSoundInstance>();
            for (int i = 0; i < SoundManagerSettings.Instance.BlendBank.EditorPoolSize; i++) ExpandPool<BlendSoundInstance>();
            for (int i = 0; i < SoundManagerSettings.Instance.SequenceBank.EditorPoolSize; i++) ExpandPool<SequenceSoundInstance>();
#else
            for (int i = 0; i < SoundManagerSettings.Instance.EffectBank.BuildPoolSize; i++) ExpandPool<EffectSoundInstance>();
            for (int i = 0; i < SoundManagerSettings.Instance.ImpactBank.BuildPoolSize; i++) ExpandPool<ImpactSoundInstance>();
            for (int i = 0; i < SoundManagerSettings.Instance.AmbienceBank.BuildPoolSize; i++) ExpandPool<AmbienceSoundInstance>();
            for (int i = 0; i < SoundManagerSettings.Instance.BlendBank.BuildPoolSize; i++) ExpandPool<BlendSoundInstance>();
            for (int i = 0; i < SoundManagerSettings.Instance.SequenceBank.BuildPoolSize; i++) ExpandPool<SequenceSoundInstance>();
#endif
        }

        public void StopNonPersistantSounds()
        {
            SoundInstance[] activeInstances = _activeInstances.ToArray();

            for (int i = 0; i < activeInstances.Length; i++)
            {
                if (!activeInstances[i].IsPersistant && !activeInstances[i].IsPooled)
                {
                    activeInstances[i].StopAndDestroy();
                }
            }
        }

        private void ExpandPool<T>() where T : SoundInstance
        {
            GameObject newObject = new GameObject(typeof(T).Name);
            newObject.transform.parent = transform;
            newObject.transform.position = Vector3.zero;
            newObject.SetActive(false);

            T sound = newObject.AddComponent<T>();
            sound.Reset();

            _inactiveInstances.Add(sound);
        }


        /// <summary>
        /// Fetches a sound instance from the pool and associates it with a sound bank so it's ready to be played.
        /// </summary>
        /// <param name="soundBank">The sound bank used to set-up the sound instance</param>
        /// <returns>The de-pooled SoundInstance</returns>
        public T FetchFromPool<T>() where T : SoundInstance
        {
            if (_inactiveInstances.GetCount<T>(false) == 0)
            {
                ExpandPool<T>();
            }

            T sound = _inactiveInstances.GetFirstInstance<T>(false);

            _activeInstances.Add(sound);
            _inactiveInstances.Remove(sound);

            sound.OnFetchedFromPool(this);
            sound.gameObject.SetActive(true);

            return sound;
        }

        /// <summary>
        /// Returns a sound instance to the pool. For unreserved sounds, this is called automatically when the sound stops playing.
        /// </summary>
        /// <param name="sound">The sound instance to return to the pool</param>
        public void ReturnToPool<T>(T sound) where T : SoundInstance
        {
            Assert.IsNotNull(sound);

            _activeInstances.Remove(sound);
            sound.OnReturnedToPool();
            sound.name = typeof(T).Name;
            _inactiveInstances.Add(sound);

            sound.gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR

    public class EditorSoundPool : ISoundPool
    {

        private List<SoundInstance> _soundInstances = new List<SoundInstance>();
        private List<AudioSource> _audioSources = new List<AudioSource>();
        private bool _registered;
        private float _lastTimeStamp;


        void Register()
        {
            EditorApplication.update += Update;
            AssemblyReloadEvents.beforeAssemblyReload += Clear;
            _lastTimeStamp = (float)EditorApplication.timeSinceStartup;
            _registered = true;
        }

        public T FetchFromPool<T>() where T : SoundInstance
        {
            if (!_registered)
            {
                Register();
            }

            GameObject newObject = new GameObject("Editor Sound Instance: " + typeof(T).Name);
            newObject.hideFlags = HideFlags.HideAndDontSave;
            newObject.transform.position = Vector3.zero;

            T sound = newObject.AddComponent<T>();
            sound.OnFetchedFromPool(this);

            _soundInstances.Add(sound);

            return sound;
        }

        public AudioSource GetAudioSource()
        {
            if (!_registered)
            {
                Register();
            }

            GameObject newObject = new GameObject("Editor Audio Source");
            newObject.hideFlags = HideFlags.HideAndDontSave;
            newObject.transform.position = Vector3.zero;

            AudioSource source = newObject.AddComponent<AudioSource>();
            _audioSources.Add(source);

            return source;
        }

        public void ReturnToPool<T>(T sound) where T : SoundInstance
        {
            sound.OnReturnedToPool();
            _soundInstances.Remove(sound);

            UnityEngine.Object.DestroyImmediate(sound.gameObject);
        }

        void Update()
        {
            float deltaTime = Mathf.Clamp((float)EditorApplication.timeSinceStartup - _lastTimeStamp, 0, 0.333f);

            for (int i = 0; i < _soundInstances.Count; i++)
            {
                _soundInstances[i].UpdateSound(deltaTime);
            }

            _lastTimeStamp = (float)EditorApplication.timeSinceStartup;
        }



        public void Clear()
        {
            for (int i = 0; i < _soundInstances.Count; i++)
            {
                UnityEngine.Object.DestroyImmediate(_soundInstances[i].gameObject);
            }

            for (int i = 0; i < _audioSources.Count; i++)
            {
                UnityEngine.Object.DestroyImmediate(_audioSources[i].gameObject);
            }

            _audioSources.Clear();
            _soundInstances.Clear();
        }

    }

#endif
}