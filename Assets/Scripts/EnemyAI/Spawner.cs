using Core;
using UnityEngine;

namespace EnemyAI.Spawning
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private double _spawnInterval = 1f;
        [SerializeField] private int _maxSpawnForDay = 500;
        [SerializeField] private double _spawnStartOffSet = 50;

        private void Start()
        {
            TimeTicker.OnTick += OnTick;
        }

        private void OnTick()
        {
            // Spawn enemies based on the normalized time of day
        }
    }
}
