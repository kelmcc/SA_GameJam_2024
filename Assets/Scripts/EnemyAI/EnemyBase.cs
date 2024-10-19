using Pathfinding;
using UnityEngine;
using UnityEngine.Pool;

namespace EnemyAI.Spawning
{
    public class EnemyBase : MonoBehaviour
    {
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