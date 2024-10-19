using Core;
using UnityEngine;

namespace EnemyAI.Spawning
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private double _spawnInterval = 1f;
        [SerializeField] private int _maxSpawnForDay = 500;
        [SerializeField] private double _spawnStartOffSet = 50;
        [SerializeField] private Transform _target;
        

        private void Start()
        {
            TimeTicker.OnTick += OnTick;
        }

        private void OnTick()
        {
            var enemyBase = EnemyPool.Instance.Pool.Get();
            enemyBase.transform.position = transform.position;
            enemyBase.transform.rotation = transform.rotation;

            enemyBase.SetActivate(_target);
            
            enemyBase.gameObject.SetActive(true);
        }
    }
}
