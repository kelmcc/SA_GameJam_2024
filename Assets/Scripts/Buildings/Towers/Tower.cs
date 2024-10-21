using SoundManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[ExecuteAlways]
public class Tower : Building
{
    public float Interval = 5;
    private float _currentT;

    public UnityEvent OnTrigger;

    private bool _active;

    public bool isCrib;

    public static Tower Crib;
    public int Health = 200;
    [FormerlySerializedAs("_currentHealth")] public int CurrentHealth;
    public bool IsBought => _active;

    public UnityEvent OnDestroyed;

    private void Awake()
    {
        if (isCrib)
        {
            Crib = this;
        }
    }

    private void Start()
    {
        if (!Application.isPlaying)
        {
            _active = true;
        }
    }
    
    public override void SetInteractable()
    {
        CurrentHealth = Health;
        _active = true;

        //throw new System.NotImplementedException();
    }

    public override void SetPassive()
    {
        _active = false;

        //throw new System.NotImplementedException();
    }

    protected override void TakeDamage(float damage)
    {
        CurrentHealth -= (int)damage;
        CurrentHealth = Mathf.Max(0, CurrentHealth);

        // throw new System.NotImplementedException();
    }

    private void Update()
    {
        if (CurrentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} Has been destroyed");
            OnDestroyed?.Invoke();
        }
        
        if (!_active)
        {
            _currentT = 0;
            return;
        }
        
        _currentT += Time.deltaTime;

        if (_currentT > Interval)
        {
            _currentT = 0;
            OnTrigger?.Invoke();
        }
    }
}
