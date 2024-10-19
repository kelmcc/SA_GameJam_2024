using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    /// <summary>
    /// Static class to summarize the runtime platform information
    /// </summary>
    public static class Runtime
    {

        /// <summary>
        /// Whether or not the game is running on Windows.
        /// </summary>
        public static bool IsWindows => Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;

        /// <summary>
        /// Whether or not the game is running on OSX.
        /// </summary>
        public static bool IsOSX => Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;

        /// <summary>
        /// Whether or not the game is running on Linux.
        /// </summary>
        public static bool IsLinux => Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor;

        /// <summary>
        /// Whether or not the game is running on Android.
        /// </summary>
        public static bool IsAndroid => Application.platform == RuntimePlatform.Android;

        /// <summary>
        /// Whether or not the game is running on IOS.
        /// </summary>
        public static bool IsIOS => Application.platform == RuntimePlatform.IPhonePlayer;

        /// <summary>
        /// Whether or not the game is running in the Unity Editor.
        /// </summary>
        public static bool IsEditor => Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor;

        /// <summary>
        /// Whether or not the game is running a debug configuration.
        /// </summary>
        public static bool IsDebug => UnityEngine.Debug.isDebugBuild;

        /// <summary>
        /// Whether or not the game is running in WebGL.
        /// </summary>
        public static bool IsWebGL => Application.platform == RuntimePlatform.WebGLPlayer;

        /// <summary>
        /// Whether or not the game is running on a standalone platform.
        /// </summary>
        public static bool IsStandalone => Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer;

        /// <summary>
        /// Whether or not the game is running on a mobile platform.
        /// </summary>
        public static bool IsMobile => Application.isMobilePlatform;

        /// <summary>
        /// Whether or not the game is running on a console.
        /// </summary>
        public static bool IsConsole => Application.isConsolePlatform;

        public static bool IsPlaying => Application.isPlaying;
        public static bool IsShuttingDown => Application.isPlaying && _isShuttingDown;
        public static bool StartedInThisSceneFromEditor => CheckInitialized() && IsEditor && _startingScene.IsValid();

        public static event Action AwakeCallback;
        public static event Action StartCallback;
        public static event Action OnEnableCallback;
        public static event Action OnDisableCallback;
        public static event Action UpdateCallback;
        public static event Action LateUpdateCallback;
        public static event Action FixedUpdateCallback;
        public static event Action<bool> OnApplicationFocusCallback;
        public static event Action<bool> OnApplicationPauseCallback;
        public static event Action OnApplicationQuitCallback;
        public static event Action OnGUICallback;
        public static event Action OnPreCullCallback;
        public static event Action OnPreRenderCallback;
        public static event Action OnPostRenderCallback;

        public static event Action BeforeUpdateCallback;
        public static event Action AfterUpdateCallback;
        public static event Action BeforeLateUpdateCallback;
        public static event Action AfterLateUpdateCallback;

        private static Scene _startingScene;
        private static bool _isShuttingDown;
        private static GameObject _pumpObject;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            _isShuttingDown = false;
            _startingScene = SceneManager.GetActiveScene();

            Application.quitting -= OnQuitting;
            Application.quitting += OnQuitting;

            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            _pumpObject = new GameObject("Runtime Update Pump");
            _pumpObject.AddComponent<RuntimeUpdatePump>();
            _pumpObject.AddComponent<RuntimeEarlyUpdatePump>();
            _pumpObject.AddComponent<RuntimeLateUpdatePump>();
        }


        static void OnQuitting()
        {
            _isShuttingDown = true;
        }

        static bool CheckInitialized()
        {
            if (_pumpObject != null) return true;

            Debug.LogError("Runtime is not initialized. Make sure Runtime is only accessed in Play Mode.");

            return false;
        }

        static void OnActiveSceneChanged(Scene fromScene, Scene toScene)
        {
            if (_startingScene.IsValid() && _startingScene != toScene)
            {
                _startingScene = new Scene();
            }
        }

        private class RuntimeUpdatePump : MonoBehaviour
        {

            void Awake()
            {
                AwakeCallback?.Invoke();
                DontDestroyOnLoad(gameObject);
                gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            void Start() => StartCallback?.Invoke();
            void OnEnable() => OnEnableCallback?.Invoke();
            void OnDisable() => OnDisableCallback?.Invoke();

            void Update() => UpdateCallback?.Invoke();
            void FixedUpdate() => FixedUpdateCallback?.Invoke();
            void LateUpdate() => LateUpdateCallback?.Invoke();
            void OnApplicationFocus(bool hasFocus) => OnApplicationFocusCallback?.Invoke(hasFocus);
            void OnApplicationPause(bool pauseStatus) => OnApplicationPauseCallback?.Invoke(pauseStatus);
            void OnApplicationQuit() => OnApplicationQuitCallback?.Invoke();
            void OnGUI() => OnGUICallback?.Invoke();
            void OnPreCull() => OnPreCullCallback?.Invoke();
            void OnPreRender() => OnPreRenderCallback?.Invoke();
            void OnPostRender() => OnPostRenderCallback?.Invoke();
        }

        [DefaultExecutionOrder(int.MinValue)]
        private class RuntimeEarlyUpdatePump : MonoBehaviour
        {

            void Awake()
            {
                AwakeCallback?.Invoke();
                DontDestroyOnLoad(gameObject);
                gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            void Update() => BeforeUpdateCallback?.Invoke();
            void LateUpdate() => BeforeLateUpdateCallback?.Invoke();
        }

        [DefaultExecutionOrder(int.MaxValue)]
        private class RuntimeLateUpdatePump : MonoBehaviour
        {

            void Awake()
            {
                AwakeCallback?.Invoke();
                DontDestroyOnLoad(gameObject);
                gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            void Update() => AfterUpdateCallback?.Invoke();
            void LateUpdate() => AfterLateUpdateCallback?.Invoke();
        }

        /* Adapted from https://github.com/nickgravelyn/UnityToolbag */

        public static string GetGeneralStorageDirectory(bool includeCompanyName = false)
        {
            string path = null;

            if (IsWindows)
            {
                path = GetWindowsPath("", includeCompanyName);
            }
            else if (IsOSX)
            {
                path = GetOSXApplicationSupportPath("", includeCompanyName);
            }
            else if (IsLinux)
            {
                path = GetLinuxSaveDirectory(includeCompanyName);
            }

            if (string.IsNullOrEmpty(path))
            {
                path = Application.persistentDataPath;
            }

            Directory.CreateDirectory(path);
            return path;
        }


        public static string GetSaveDirectory(bool includeCompanyName = false)
        {
            string path = null;

            if (IsWindows)
            {
                path = GetWindowsPath("Saves", includeCompanyName);
            }
            else if (IsOSX)
            {
                path = GetOSXApplicationSupportPath("Saves", includeCompanyName);
            }
            else if (IsLinux)
            {
                path = GetLinuxSaveDirectory(includeCompanyName);
            }

            if (string.IsNullOrEmpty(path))
            {
                path = Application.persistentDataPath;
            }

            Directory.CreateDirectory(path);
            return path;
        }

        public static string GetConfigDirectory(bool includeCompanyName = false)
        {
            string path = null;

            if (IsWindows)
            {
                path = GetWindowsPath("Config", includeCompanyName);
            }
            else if (IsOSX)
            {
                path = GetOSXApplicationSupportPath("Config", includeCompanyName);
            }
            else if (IsLinux)
            {
                path = GetLinuxConfigDirectory(includeCompanyName);
            }
            if (string.IsNullOrEmpty(path))
            {
                path = Application.persistentDataPath;
            }

            Directory.CreateDirectory(path);
            return path;
        }

        public static string GetLogDirectory(bool includeCompanyName = false)
        {
            string path = null;

            if (IsWindows)
            {
                path = GetWindowsPath("Logs", includeCompanyName);
            }
            else if (IsOSX)
            {
                path = GetOSXLogsPath(includeCompanyName);
            }
            else if (IsLinux)
            {
                path = GetLinuxLogDirectory(includeCompanyName);
            }

            if (string.IsNullOrEmpty(path))
            {
                path = Application.persistentDataPath;
            }

            Directory.CreateDirectory(path);
            return path;
        }

        private static string AppendProductPath(string path, bool includeCompanyName)
        {
            if (includeCompanyName)
            {
                path = AppendDirectory(path, Application.companyName);
            }

            return AppendDirectory(path, Application.productName);
        }

        private static string AppendDirectory(string path, string dir)
        {
            if (string.IsNullOrEmpty(dir)) return path;

            char[] invalidCharacters = Path.GetInvalidFileNameChars();
            StringBuilder cleanDir = new StringBuilder();

            for (int i = 0; i < dir.Length; i++)
            {
                char c = dir[i];
                bool clean = true;

                for (int j = 0; j < invalidCharacters.Length; j++)
                {
                    if (c == invalidCharacters[j])
                    {
                        clean = false;
                        break;
                    }
                }

                if (clean)
                {
                    cleanDir.Append(c);
                }
            }

            return Path.Combine(path, cleanDir.ToString());
        }

        public static string GetWindowsPath(string subdirectory, bool includeCompanyName)
        {
            string result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games");
            result = AppendProductPath(result, includeCompanyName);
            return AppendDirectory(result, subdirectory);
        }

        public static string GetOSXApplicationSupportPath(string subdirectory, bool includeCompanyName)
        {
            string result = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library/Application Support");
            result = AppendProductPath(result, includeCompanyName);
            return AppendDirectory(result, subdirectory);
        }

        public static string GetOSXLogsPath(bool includeCompanyName)
        {
            string result = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library/Logs");
            return AppendProductPath(result, includeCompanyName);
        }

        public static string GetLinuxSaveDirectory(bool includeCompanyName)
        {
            string result = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
            if (string.IsNullOrEmpty(result))
            {
                string home = Environment.GetEnvironmentVariable("HOME");
                result = Path.Combine(home, ".local/share");
            }

            return AppendProductPath(result, includeCompanyName);
        }

        public static string GetLinuxConfigDirectory(bool includeCompanyName)
        {
            string result = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            if (string.IsNullOrEmpty(result))
            {
                string home = Environment.GetEnvironmentVariable("HOME");
                result = Path.Combine(home, ".config");
            }

            return AppendProductPath(result, includeCompanyName);
        }

        public static string GetLinuxLogDirectory(bool includeCompanyName)
        {
            return AppendProductPath("/var/log", includeCompanyName);
        }


    }
}

