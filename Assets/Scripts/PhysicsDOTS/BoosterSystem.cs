using System.Collections.Generic;
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
        private NativeList<Entity> _boosterEntities;
        private EntityManager _entityManager;

        private const bool CenterLineUsesY = false;

        public void OnCreate(ref SystemState state)
        {
            _boosterEntities = new NativeList<Entity>(Allocator.Persistent);
            _entityManager = state.EntityManager;

            NativeArray<Entity> entities = _entityManager.GetAllEntities(Allocator.Temp);
            foreach (Entity entity in entities)
            {
                if (_entityManager.HasComponent<BoosterComponent>(entity))
                {
                    _boosterEntities.Add(entity);
                }
            }
        }

        private void OnUpdate(ref SystemState state)
        {
            if (_boosterEntities.Length == 0)
            {
                BoostersSetUp();
            }

            foreach (Entity entity in _boosterEntities)
            {
                RefRW<LocalToWorld> boosterTransform = SystemAPI.GetComponentRW<LocalToWorld>(entity);
                RefRO<BoosterComponent> boosterComponent = SystemAPI.GetComponentRO<BoosterComponent>(entity);

                var cpm = boosterComponent.ValueRO;

                PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

                physicsWorldSingleton.BoxCastAll(boosterTransform.ValueRO.Position,
                    boosterTransform.ValueRO.Rotation,
                    new float3(cpm.sizeX / 2, cpm.sizeY / 2,
                        cpm.sizeZ / 2),
                    float3.zero, 1, ref hits,
                    new CollisionFilter
                        { BelongsTo = (uint)EnemiesLayer.Triggers, CollidesWith = (uint)EnemiesLayer.Enemies });

                foreach (ColliderCastHit hit in hits)
                {
                    var enemy = hit.Entity;

                    RefRW<PhysicsVelocity> physicsVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(enemy);

                    RefRW<LocalToWorld> enemyTrans = SystemAPI.GetComponentRW<LocalToWorld>(enemy);

                    var localEntityPosRelativeToBoxCenter =
                        enemyTrans.ValueRO.Position - boosterTransform.ValueRO.Position;

                    var normCenterLine = math.normalize(cpm.velocityDirection);
                    var projMag = math.dot(localEntityPosRelativeToBoxCenter, normCenterLine);

                    var projectedPointOnCenterLine = normCenterLine * math.abs(projMag);
                    var vecTowardsCenterLine = projectedPointOnCenterLine - localEntityPosRelativeToBoxCenter;

                    if (!CenterLineUsesY) //Sorry I had to. Const value at top of file. x
                    {
                        vecTowardsCenterLine = new float3(vecTowardsCenterLine.x, 0, vecTowardsCenterLine.z);
                    }

                    physicsVelocity.ValueRW.Linear += new float3(
                        (cpm.velocityDirection.x * cpm.BoosterStrength +
                         vecTowardsCenterLine.x * cpm.CorrallingMultiplier) *
                        SystemAPI.Time.DeltaTime,
                        (cpm.velocityDirection.y * cpm.BoosterStrength +
                         vecTowardsCenterLine.y * cpm.CorrallingMultiplier) * cpm.BoosterStrength *
                        SystemAPI.Time.DeltaTime,
                        (cpm.velocityDirection.z * cpm.BoosterStrength +
                         vecTowardsCenterLine.z * cpm.CorrallingMultiplier) * cpm.BoosterStrength *
                        SystemAPI.Time.DeltaTime);
                }
            }
        }

        private void BoostersSetUp()
        {
            NativeArray<Entity> entities = _entityManager.GetAllEntities(Allocator.Temp);
            foreach (Entity entity in entities)
            {
                if (_entityManager.HasComponent<BoosterComponent>(entity))
                {
                    _boosterEntities.Add(entity);
                }
            }
        }
    }
}

public enum EnemiesLayer
{
    Enemies = 1 << 6,
    Triggers = 1 << 7
}