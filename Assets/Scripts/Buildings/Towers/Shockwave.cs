using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Shockwave : MonoBehaviour
{
    public GameObject ShockwavePrefab;


    private ObjectPool<GameObject> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<GameObject>(() => Instantiate(ShockwavePrefab, transform, false),
            g => { g.SetActive(true); },
            gr => gr.SetActive(false),
            Destroy);
    }

    public void Trigger()
    {
        GameObject obj = _pool.Get();
        ShockwaveInstance i = obj.GetComponent<ShockwaveInstance>();
        i.OnDestroy = () =>
        {
            _pool.Release(obj);
            i.OnDestroy = null;
        };
    }
}
