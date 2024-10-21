using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DamageNumbers : MonoBehaviour
{
    public DamageNumberInstance Prefab;

    public static DamageNumbers Instance;
    
    private ObjectPool<DamageNumberInstance> _pool;

    private void Awake()
    {
        Instance = this;

        _pool = new ObjectPool<DamageNumberInstance>(() => Instantiate(Prefab, transform, false),
            g => { g.gameObject.SetActive(true); },
            gr => gr.gameObject.SetActive(false),
            Destroy);
    }
    
    public void SpawnDamageNumber(Vector3 position, int number, bool showCoinIcon)
    {
        DamageNumberInstance instance = _pool.Get();
        instance.transform.position = position;
        instance.SetDamage(number, showCoinIcon, _pool);
    }
}
