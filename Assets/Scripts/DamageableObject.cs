using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using EnemyAI.Spawning;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Collider = UnityEngine.Collider;

namespace Agents
{
    [RequireComponent(typeof(Collider))]
    public abstract class Damagable : MonoBehaviour
    {
        public float TakeDamageRadius = 3;
        List<EnemyBase> _enemiesInRange = new List<EnemyBase>();

        protected void OnEnable()
        {
            TimeTicker.OnTick += OnTick;
        }

        protected void OnDisable()
        {
            TimeTicker.OnTick -= OnTick;
        }
        
        (bool, Vector3, Action) PerformSphereQuery(Vector3 pos, float radius)
    {
        // Get the default World
        World defaultWorld = World.DefaultGameObjectInjectionWorld;

        // Get the EntityManager
        var entityManager = defaultWorld.EntityManager;

        // Get the PhysicsWorldSingleton component
        /*if (!entityManager.HasComponent<PhysicsWorldSingleton>(entityManager.UniversalQuery))
        {
            Debug.LogWarning("PhysicsWorldSingleton not found. Ensure that Unity Physics package is properly set up.");
            return;
        }*/

        var physicsWorldSingleton = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton)).GetSingleton<PhysicsWorldSingleton>();

        // Get the PhysicsWorld from the singleton
        var physicsWorld = physicsWorldSingleton.PhysicsWorld;

        // Prepare the overlap sphere input
        var pointDistanceInput = new PointDistanceInput
        {
            Position = pos,
            MaxDistance = radius,
            Filter =  new CollisionFilter {CollidesWith = (uint)EnemiesLayer.Enemies, BelongsTo  = (uint)(EnemiesLayer.Enemies | EnemiesLayer.Triggers | EnemiesLayer.Player | EnemiesLayer.Buildings)}
        };
        
        // Prepare the collector to gather all hits

       // var collector = new AllHitsCollector<DistanceHit>();

        Action destroyEntity = () => { };

        // Perform the query
        if (physicsWorld.CalculateDistance(pointDistanceInput, out DistanceHit closestHit))
        {
            float3 closestPosition = new float3();
           
            // Get the entity from the hit
            Entity entity = physicsWorld.Bodies[closestHit.RigidBodyIndex].Entity;
    
            // Exclude the static ground or invalid entities
            if (entity != Entity.Null)
            {
                // Calculate the distance
                float distance = closestHit.Distance;
                Entity closestEntity = entity;
                closestPosition = closestHit.Position;
            

                // Destroy the closest entity
                if (closestEntity != Entity.Null)
                {
                    destroyEntity = () =>
                    {
                        entityManager.DestroyEntity(closestEntity);
                    };
                }
            }
            
            return (true, closestPosition, destroyEntity);
        }
        
        return (false, Vector3.zero, destroyEntity);
        // Dispose of the collector to prevent memory leaks
        //collector.Dispose();
    }

        protected void OnTriggerEnter(Collider other)
        {
            var enemy = other.gameObject.GetComponent<EnemyBase>();
            if (enemy != null && !_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Add(enemy);
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            var enemy = other.gameObject.GetComponent<EnemyBase>();
            if (enemy != null && !_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Remove(enemy);
            }
        }

        private void OnTick()
        {
            //float totalDamage = _enemiesInRange.Sum(enemy => enemy.DamagePerHit);

            (bool hit, Vector3 pos, Action destroy) = PerformSphereQuery(transform.position, TakeDamageRadius);
            if (hit)
            {
                Debug.Log($"<color=red>{gameObject.name} takes damage!</color>");
                TakeDamage(10);
                destroy?.Invoke();
                Player.Instance.PlayEnemyDie(transform.position);
            }
       
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, TakeDamageRadius);
        }

        protected abstract void TakeDamage(float damage);
    }
}