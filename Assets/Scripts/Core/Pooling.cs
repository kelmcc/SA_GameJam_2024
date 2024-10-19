using System;
using EnemyAI.Spawning;
using UnityEngine;
using UnityEngine.Pool;

namespace Core
{
    public class EnemyPool : MonoBehaviourSingleton<EnemyPool>
    {
        private IObjectPool<EnemyBase> _pool;
        
        [SerializeField] private Transform _poolParent;
        [SerializeField] private Transform _activeParent;
        
        [SerializeField] private EnemyBase _enemyPrefab;
        [SerializeField] private int _poolSize = 1000;

        private EnemyPool()
        {
        }
        

    }
}