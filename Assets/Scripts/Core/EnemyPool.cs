using System;
using EnemyAI.Spawning;
using UnityEngine;
using UnityEngine.Pool;

namespace Core
{
    public class EnemyPool : MonoBehaviourSingleton<EnemyPool>
    {
        public IObjectPool<EnemyBase> Pool => _pool;
        private IObjectPool<EnemyBase> _pool;

        [SerializeField] private Transform _poolParent;
        [SerializeField] private Transform _activeParent;

        [SerializeField] private EnemyBase _enemyPrefab;
        [SerializeField] private int _maxPoolSize = 1000;
        [SerializeField] private int _startingPoolSize = 200;

        private EnemyPool()
        {
        }

        private void Start()
        {
            _pool = new ObjectPool<EnemyBase>(CreatEnemyBase, OnGetEnemyFromPool, OnReturnEnemyToPool, OnDestroyEnemy,
                true, _startingPoolSize, _maxPoolSize);
        }

        private EnemyBase CreatEnemyBase()
        {
            EnemyBase enemy = Instantiate(_enemyPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), _poolParent);
            enemy.Deactivate();
            enemy.gameObject.SetActive(false);

            return enemy;
        }

        private void OnGetEnemyFromPool(EnemyBase enemy)
        {
            enemy.transform.SetParent(_activeParent);
        }

        private void OnReturnEnemyToPool(EnemyBase enemy)
        {
            enemy.transform.SetParent(_poolParent);

            enemy.Deactivate();
            enemy.gameObject.SetActive(false);
        }

        private void OnDestroyEnemy(EnemyBase enemy)
        {
            Destroy(enemy.gameObject);
        }
    }
}