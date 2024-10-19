using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace PhysicsDOTS
{
    public partial struct BoosterSystem : ISystem
    {
        private void OnUpdate(ref SystemState state)
        {
            EntityManager entityManager = state.EntityManager;

            NativeArray<Entity> entities = entityManager.GetAllEntities(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                if (entityManager.HasComponent<BoosterComponent>(entity))
                {
                    RefRW<LocalToWorld> boosterTransform = SystemAPI.GetComponentRW<LocalToWorld>(entity);
                    RefRO<BoosterComponent> boosterComponent = SystemAPI.GetComponentRO<BoosterComponent>(entity);

                    float size = boosterComponent.ValueRO.size;
                    boosterTransform.ValueRW.Value.c0 = new float4(size, 0, 0, 0);
                    boosterTransform.ValueRW.Value.c1 = new float4(0, size, 0, 0);
                    boosterTransform.ValueRW.Value.c2 = new float4(0, 0, size, 0);

                    PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

                    NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

                    physicsWorldSingleton.BoxCastAll(boosterTransform.ValueRO.Position,
                        boosterTransform.ValueRO.Rotation,
                        boosterComponent.ValueRO.size / 2,
                        float3.zero, 1, ref hits,
                        new CollisionFilter
                            { BelongsTo = (uint)EnemiesLayer.Triggers, CollidesWith = (uint)EnemiesLayer.Enemies });

                    foreach (ColliderCastHit hit in hits)
                    {
                        RefRW<PhysicsVelocity> physicsVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(hit.Entity);
                        physicsVelocity.ValueRW.Linear += new float3(20 * SystemAPI.Time.DeltaTime, 0, 0);
                    }
                }
            }

            entities.Dispose();
        }
    }
}

public enum EnemiesLayer
{
    Enemies = 1 << 6,
    Triggers = 1 << 7
}