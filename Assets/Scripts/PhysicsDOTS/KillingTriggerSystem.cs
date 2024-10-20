using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace PhysicsDOTS
{
    public partial struct KillingTriggerSystem : ISystem
    {
        private void OnUpdate(ref SystemState state)
        {
            EntityManager entityManager = state.EntityManager;

            NativeArray<Entity> entities = entityManager.GetAllEntities(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                if (entityManager.HasComponent<KillingTriggerComponent>(entity))
                {
                    RefRW<LocalToWorld> killingTriggerTransform = SystemAPI.GetComponentRW<LocalToWorld>(entity);
                    RefRO<KillingTriggerComponent> killingTriggerComponent =
                        SystemAPI.GetComponentRO<KillingTriggerComponent>(entity);

                    var cpm = killingTriggerComponent.ValueRO;
                    killingTriggerTransform.ValueRW.Value.c0 = new float4(cpm.sizeX, 0, 0, 0);
                    killingTriggerTransform.ValueRW.Value.c1 = new float4(0, cpm.sizeY, 0, 0);
                    killingTriggerTransform.ValueRW.Value.c2 = new float4(0, 0, cpm.sizeZ, 0);

                    PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

                    NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

                    physicsWorldSingleton.BoxCastAll(killingTriggerTransform.ValueRO.Position,
                        killingTriggerTransform.ValueRO.Rotation,
                        new float3(cpm.sizeX / 2, cpm.sizeY / 2,
                            cpm.sizeZ / 2),
                        float3.zero, 1, ref hits,
                        new CollisionFilter
                            { BelongsTo = (uint)EnemiesLayer.Triggers, CollidesWith = (uint)EnemiesLayer.Enemies });

                    foreach (ColliderCastHit hit in hits)
                    {
                        entityManager.DestroyEntity(hit.Entity);
                    }
                }
            }

            entities.Dispose();
        }
    }
}