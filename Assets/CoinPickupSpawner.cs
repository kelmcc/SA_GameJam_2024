using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class CoinPickupSpawner : MonoBehaviour
{
    public Pickup Prefab;
    public Pickup KinematicPrefab;

    public static CoinPickupSpawner Instance;

    public Player Player;
    private ObjectPool<Pickup> _pool;
    private ObjectPool<Pickup> _kinematicPool;
    private void Awake()
    {
        Instance = this;
        _pool = new ObjectPool<Pickup>(() => Instantiate(Prefab, transform, false),
            g => { g.gameObject.SetActive(true); },
            gr => gr.gameObject.SetActive(false),
            Destroy);
        
        _kinematicPool = new ObjectPool<Pickup>(() => Instantiate(KinematicPrefab, transform, false),
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

    public void SpawnCoin(Vector3 position, bool kinematic = false)
    {
        Pickup p;

        if (kinematic)
        {
            p = _kinematicPool.Get();
            p.Kinematic = true;
        }
        else
        {
           p = _pool.Get();
        }
        p.transform.position = position;
    }

    public void PickedUp(Pickup pickup)
    {
        if (pickup.Kinematic)
        {
            _kinematicPool.Release(pickup);
        }
        else
        {
            _pool.Release(pickup);
        }
       
    }
}
