using SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class Tower : Building
{
    public float Interval = 5;
    private float _currentT;

    public UnityEvent OnTrigger;

    private bool _active;

    private void Start()
    {
        if (!Application.isPlaying)
        {
            _active = true;
        }
    }
    
    public override void SetInteractable()
    {
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
       // throw new System.NotImplementedException();
    }

    private void Update()
    {
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
