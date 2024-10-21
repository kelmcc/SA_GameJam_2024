using System.Collections;
using System.Collections.Generic;
using SoundManager;
using UnityEngine;

public class PlayBankAudio : MonoBehaviour
{
    [SerializeField] private SoundBank bank;
    [SerializeField] private bool loop = false;

    void OnEnable()
    {
      
        if (bank is EffectSoundBank e)
        {
            var instance = e.Play();
            instance.SetLooping(loop);
        }
        else
        {
            bank.Play();
        }
    }

}