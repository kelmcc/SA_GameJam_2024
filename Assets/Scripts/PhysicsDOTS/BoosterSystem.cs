using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PhysicsDOTS
{
    [BurstCompile]
    public partial struct BoosterSystem : ISystem
    {
        private const bool CenterLineUsesY = false;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // No need to do anything here since we'll use queries in OnUpdate
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var deltaTime = SystemAPI.Time.DeltaTime;

            // Create ComponentLookups for accessing components on other entities
            var physicsVelocityLookup = state.GetComponentLookup<PhysicsVelocity>();
            var localToWorldLookup = state.GetComponentLookup<LocalToWorld>(true);

            // Update the lookups to ensure they are current
            physicsVelocityLookup.Update(ref state);
            localToWorldLookup.Update(ref state);

            // Schedule a job that processes all entities with BoosterComponent and LocalToWorld
            var boosterJob = new BoosterJob
            {
                PhysicsWorld = physicsWorldSingleton,
                DeltaTime = deltaTime,
                CenterLineUsesY = CenterLineUsesY,
                PhysicsVelocityLookup = physicsVelocityLookup,
                LocalToWorldLookup = localToWorldLookup
            };
            boosterJob.Schedule();
        }

        [BurstCompile]
        private partial struct BoosterJob : IJobEntity
        {
            public PhysicsWorldSingleton PhysicsWorld;
            public float DeltaTime;
            public bool CenterLineUsesY;

            // Component lookups to access components on other entities
            public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;
            [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldLookup;

            public void Execute(ref BoosterComponent boosterComponent, in LocalToWorld boosterTransform)
            {
                BoosterComponent cpm = boosterComponent;

                // Prepare collision filter
                CollisionFilter collisionFilter = new CollisionFilter
                {
                    BelongsTo = (uint)EnemiesLayer.Triggers,
                    CollidesWith = (uint)EnemiesLayer.Enemies,
                    GroupIndex = 0
                };

                // Perform the box cast
                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                PhysicsWorld.BoxCastAll(
                    boosterTransform.Position,
                    boosterTransform.Rotation,
                    new float3(cpm.sizeX / 2, cpm.sizeY / 2, cpm.sizeZ / 2),
                    float3.zero,
                    1,
                    ref hits,
                    collisionFilter);

                float3 normCenterLine = math.normalize(cpm.velocityDirection);

                // Process each hit
                foreach (ColliderCastHit hit in hits)
                {
                    Entity enemy = hit.Entity;

                    // Check if the enemy has the required components
                    if (!PhysicsVelocityLookup.HasComponent(enemy) || !LocalToWorldLookup.HasComponent(enemy))
                        continue;

                    PhysicsVelocity physicsVelocity = PhysicsVelocityLookup[enemy];
                    LocalToWorld enemyTrans = LocalToWorldLookup[enemy];

                    float3 localEntityPosRelativeToBoxCenter = enemyTrans.Position - boosterTransform.Position;
                    float projMag = math.dot(localEntityPosRelativeToBoxCenter, normCenterLine);
                    float3 projectedPointOnCenterLine = normCenterLine * math.abs(projMag);
                    float3 vecTowardsCenterLine = projectedPointOnCenterLine - localEntityPosRelativeToBoxCenter;

                    if (!CenterLineUsesY)
                    {
                        vecTowardsCenterLine = new float3(vecTowardsCenterLine.x, 0, vecTowardsCenterLine.z);
                    }

                    float3 velocityChange = new float3(
                        (cpm.velocityDirection.x * cpm.BoosterStrength + vecTowardsCenterLine.x * cpm.CorrallingMultiplier) * DeltaTime,
                        (cpm.velocityDirection.y * cpm.BoosterStrength + vecTowardsCenterLine.y * cpm.CorrallingMultiplier) * DeltaTime,
                        (cpm.velocityDirection.z * cpm.BoosterStrength + vecTowardsCenterLine.z * cpm.CorrallingMultiplier) * DeltaTime
                    );

                    // Update the physics velocity
                    physicsVelocity.Linear += velocityChange;

                    // Write back the updated velocity
                    PhysicsVelocityLookup[enemy] = physicsVelocity;
                }

                hits.Dispose();
            }
        }
    }
}

[Flags]
public enum EnemiesLayer
{
    Player = 1 << 4,
    Enemies = 1 << 6,
    Triggers = 1 << 7,
    Buildings = 1 << 10
}

