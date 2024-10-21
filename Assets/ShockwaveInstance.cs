using Ross.EditorRuntimeCombatibility;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShockwaveInstance : MonoBehaviour
{
    public float Lifetime = 0.5f;
    public float MaxScale = 10;

    private float _currentLifetime;

    public List<Renderer> Renderers;
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    public Action OnDestroy { get; set; }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        _currentLifetime = 0;
        foreach (Renderer r in Renderers)
        {
            var m =  r.material;
            m.color = m.color.WithAlpha(1);
        }
    }

    void Update()
    {
        _currentLifetime += Time.deltaTime;
        float t = Mathf.Clamp01(_currentLifetime / Lifetime);
        transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(MaxScale,MaxScale,MaxScale),t );

        foreach (Renderer r in Renderers)
        {
           var m =  r.material;
           Color c = m.GetColor(Color1);
           m.SetColor(Color1, c.WithAlpha(t));
        }
        
        if (_currentLifetime >= Lifetime)
        {
            OnDestroy?.Invoke();
        }
    }
}
