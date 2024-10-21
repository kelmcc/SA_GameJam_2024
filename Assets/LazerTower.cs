using Framework;
using PhysicsDOTS;
using SoundManager;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class LazerTower : MonoBehaviour
{
    public float MoveToDestinationTime = 0.2f;
    public float MaxRange = 60;

    public bool RandomTest = false;

    public GameObject LazerRoot;

    public Transform Pivot;
    public float PivotSpeed = 4;

    public ParticleSystem HitPoint;
    public LineRenderer LazerLine;

    public ParticleSystem afterFiring;
    
    public EffectSoundBank ZapSFX;
    public EffectSoundBank SpiderDieSFX;

    private Coroutine test = null;
    private void Update()
    {
        if (RandomTest && test == null)
        {
            test = StartCoroutine(Test());
            IEnumerator Test()
            {
                while (RandomTest)
                {
                    yield return new WaitForSeconds(1);
                    Vector3 r = Random.onUnitSphere;
                    
                }

                test = null;
            }
        }
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
            Filter =  new CollisionFilter {CollidesWith = (uint)EnemiesLayer.Enemies, BelongsTo  = (uint)EnemiesLayer.Triggers}
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

    public void Trigger()
    {
        (bool hit, Vector3 pos, Action destroy) = PerformSphereQuery(Pivot.transform.position, MaxRange);

        if (hit)
        {
            Vector3 diff = (pos - Pivot.transform.position);
            Vector3 dir = diff.normalized;
            Trigger(dir, diff.magnitude, destroy);
        }
        else
        {
            Debug.Log("No enemies to hit");
        }
    }
    

    public void Trigger(Vector3 direction, float distance, Action destroy)
    {
        CoroutineUtils.StartCoroutine(Effect());
        IEnumerator Effect()
        {
            Quaternion destination = Quaternion.LookRotation(direction, Vector3.up);
            float t = 0;

            while (t < MoveToDestinationTime)
            {
                t += Time.deltaTime * PivotSpeed;
                Pivot.rotation = Quaternion.Slerp(Pivot.rotation, destination, t);

                yield return null;
            }
            
            Pivot.rotation = destination;

            HitPoint.transform.position = Pivot.transform.position;
            ZapSFX.Play(HitPoint.transform);
            yield return new WaitForSeconds(0.1f);
            
            LazerRoot.SetActive(true);
            destroy?.Invoke();
         
            Vector3 hitPos = Pivot.position + direction * Mathf.Abs(distance);
            SpiderDieSFX.Play(hitPos);
            
            float diff = Vector3.Distance(Pivot.transform.position, LazerLine.transform.position);
            LazerLine.transform.localScale = new Vector3(1, 1, (distance-diff) / MaxRange);
            HitPoint.transform.position = hitPos;
            HitPoint.Play();
            yield return new WaitForSeconds(0.2f);
            CoinPickupSpawner.Instance.SpawnCoin(hitPos);
            LazerRoot.SetActive(false);
            
            afterFiring.Play();
        }
    }
}
