using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class CoinPickupSpawner : MonoBehaviour
{
    public Pickup Prefab;
    private ObjectPool<Pickup> _pool;

    public static CoinPickupSpawner Instance;

    public Player Player;

    private void Awake()
    {
        Instance = this;
        _pool = new ObjectPool<Pickup>(() => Instantiate(Prefab, transform, false),
            g => { g.gameObject.SetActive(true); },
            gr => gr.gameObject.SetActive(false),
            Destroy);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnCoin(Player.transform.position + Random.onUnitSphere * 20);
        }
    }

    public void SpawnCoin(Vector3 position)
    {
        Pickup p = _pool.Get();
        p.transform.position = position;
    }

    public void PickedUp(Pickup pickup)
    {
        _pool.Release(pickup);
    }
}
