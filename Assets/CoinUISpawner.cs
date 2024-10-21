using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class CoinUISpawner : MonoBehaviour
{
    public Transform SpawnPoint;
    public GameObject Prefab;
    private ObjectPool<GameObject> _pool;

    public static CoinUISpawner Instance;

    private void Awake()
    {
        Instance = this;
        _pool = new ObjectPool<GameObject>(() => Instantiate(Prefab, transform, false),
            g => { g.SetActive(true); },
            gr => gr.SetActive(false),
            Destroy);
    }
    

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        CoinUIController.Instance.PopObjective();
        GameObject g = _pool.Get();
        g.transform.position = SpawnPoint.position;
        var rb = g.GetComponent<Rigidbody>();
        rb.velocity = Random.onUnitSphere*0.1f;
        rb.angularVelocity = new Vector3(Random.value * 60, Random.value * 60, Random.value *60);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CoinPurseInstance>())
        {
            _pool.Release(other.gameObject);
            other.gameObject.transform.position = SpawnPoint.position;
        }
    }
}
