using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoundManager
{
    /// <summary>
    /// Robbie should write a comment here.
    /// </summary>
    public static class SoundManagerEditorUtils
    {



        [MenuItem("Assets/Create/Sound Bank/From Selected Clips")]
        private static void CreateSoundBankFromClips()
        {
            List<AudioClip> clips = new List<AudioClip>();
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                AudioClip clip = Selection.objects[i] as AudioClip;
                if (clip != null)
                {
                    clips.Add(clip);
                }
            }

            clips.Sort((x, y) => x.name.CompareTo(y.name));

            if (clips.Count > 0)
            {
                EffectSoundBank bank = ScriptableObject.CreateInstance<EffectSoundBank>();
                bank.SetClips(clips.ToArray());

                string path = AssetDatabase.GetAssetPath(clips[0]);
                path = path.Substring(0, path.LastIndexOf("/"));
                path += "/" + clips[0].name + ".asset";

                AssetDatabase.CreateAsset(bank, AssetDatabase.GenerateUniqueAssetPath(path));
                AssetDatabase.SaveAssets();

                Selection.activeObject = bank;
            }

        }

        public static SoundBank CreateNewSoundBank(Type type, string name)
        {
            SoundBank bank = ScriptableObject.CreateInstance(type) as SoundBank;
            AssetDatabase.CreateAsset(bank, AssetDatabase.GenerateUniqueAssetPath("Assets/" + name + ".asset"));
            AssetDatabase.SaveAssets();

            return bank;
        }

    }
}
