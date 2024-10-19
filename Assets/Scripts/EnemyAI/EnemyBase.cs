using Pathfinding;
using UnityEngine;
using UnityEngine.Pool;

namespace EnemyAI.Spawning
{
    public class EnemyBase : MonoBehaviour
    {
        public float DamagePerHit => _baseDamagePerHit * _damageMultiplier;
        [SerializeField] private float _baseDamagePerHit = 1;
        [SerializeField] private float _damageMultiplier = 1; // alter this to be a curve over timeTicker's day timer
        
        [SerializeField] private AIPath _ai;
        [SerializeField] private AIDestinationSetter _destination;
        public void Deactivate()
        {
           // _ai.destination = null;
        }
        
        public void SetActivate(Transform target)
        {
            _destination.target = target;
        }
        
        public void OnDeath()
        {
            
        }
    }
}