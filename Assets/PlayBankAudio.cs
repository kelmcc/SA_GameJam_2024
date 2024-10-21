using System.Collections;
using System.Collections.Generic;
using SoundManager;
using UnityEngine;

public class PlayBankAudio : MonoBehaviour
{
    [SerializeField] private EffectSoundBank bank;

    void OnEnable()
    {
        bank.Play();
    }

}