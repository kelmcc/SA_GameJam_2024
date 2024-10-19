using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoundManager
{
    public class SoundBoardWindow : EditorWindow
    {

        private List<SoundBank> _soundBanks = new List<SoundBank>();
        private List<SoundInstance> _soundInstances = new List<SoundInstance>();
        private GUIStyle _buttonStyle;
        private float _newBankWidth = 300f;

        private static EditorSoundPool _soundPool;

        [MenuItem("Window/Sound Board")]
        static void Open()
        {
            GetWindow(typeof(SoundBoardWindow), false, "Sound Board").Show();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _soundPool = null;
        }

        void OnGUI()
        {

            SoundBoardWindow window = (SoundBoardWindow)GetWindow(typeof(SoundBoardWindow), false, "Sound Board");
            window.minSize = new Vector2(412f, 80f);

            if (_soundPool == null)
            {
                _soundPool = new EditorSoundPool();
            }

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
                _buttonStyle.margin = new RectOffset(4, 4, 1, 0);
            }



            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();

            GUILayout.Space(20f);
            GUILayout.Label("Sound Bank", "BoldLabel", GUILayout.MinWidth(200f), GUILayout.MaxWidth(300f));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("PLAY ALL", GUILayout.MinWidth(80f), GUILayout.MaxWidth(100f)))
            {
                for (int i = 0; i < _soundInstances.Count; i++)
                {
                    StopSound(i);
                    PlaySound(i);
                }
            }
            if (GUILayout.Button("STOP", GUILayout.MaxWidth(80f)))
            {
                for (int i = 0; i < _soundInstances.Count; i++)
                {
                    StopSound(i);
                }
            }
            if (GUILayout.Button("X", GUILayout.MaxWidth(25f)))
            {
                _soundPool.Clear();
                _soundInstances.Clear();
                _soundBanks.Clear();
            }

            GUILayout.Space(15f);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("GroupBox");



            for (int i = 0; i < _soundBanks.Count; i++)
            {
                DrawRow(i);
            }

            EditorGUILayout.BeginHorizontal();

            Object newBank = EditorGUILayout.ObjectField("", null, typeof(SoundBank), false, GUILayout.Width(_newBankWidth));

            if (newBank != null)
            {
                _soundBanks.Add((SoundBank)newBank);
                _soundInstances.Add(null);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }

        void DrawRow(int index)
        {
            GUILayout.BeginHorizontal();

            _soundBanks[index] = (SoundBank)EditorGUILayout.ObjectField("", _soundBanks[index], typeof(SoundBank), false, GUILayout.MaxWidth(300f));
            if (_soundBanks[index] == null)
            {
                _soundInstances.RemoveAt(index);
                _soundBanks.RemoveAt(index);

                GUILayout.EndHorizontal();
                return;
            }

            if (GUILayoutUtility.GetLastRect().width > 100f)
            {
                _newBankWidth = GUILayoutUtility.GetLastRect().width;
            }

            GUILayout.FlexibleSpace();

            bool isPaused = _soundInstances[index] != null && _soundInstances[index].IsPaused;
            bool isPlaying = _soundInstances[index] != null && _soundInstances[index].IsPlaying;

            if (GUILayout.Button(isPaused ? "RESUME" : isPlaying ? "PAUSE" : "PLAY", _buttonStyle, GUILayout.MinWidth(80f), GUILayout.MaxWidth(100f)))
            {
                PlaySound(index);
            }
            if (GUILayout.Button("STOP", _buttonStyle, GUILayout.MaxWidth(80f)))
            {
                StopSound(index);
            }
            if (GUILayout.Button("X", _buttonStyle, GUILayout.MaxWidth(25f)))
            {
                if (_soundInstances[index] != null)
                {
                    _soundInstances[index].StopAndDestroy();
                }

                _soundInstances.RemoveAt(index);
                _soundBanks.RemoveAt(index);
            }


            GUILayout.EndHorizontal();
            GUILayout.Space(1f);
        }

        void PlaySound(int index)
        {
            if (_soundInstances[index] != null && _soundInstances[index].IsPaused)
            {
                _soundInstances[index].Resume();
            }
            else
            {
                if (_soundInstances[index] != null && _soundInstances[index].IsPlaying)
                {
                    _soundInstances[index].Pause();
                }
                else
                {
                    _soundInstances[index] = _soundBanks[index].TestInEditor(_soundPool);
                }
            }
        }

        void StopSound(int index)
        {
            if (_soundInstances[index] != null)
            {
                _soundInstances[index].StopAndDestroy();
            }
        }
    }
}
