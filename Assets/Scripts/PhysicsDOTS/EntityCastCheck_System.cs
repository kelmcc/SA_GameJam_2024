using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PhysicsDOTS
{
    public partial struct EntityCastCheck_System : ISystem
    {
        private NativeList<Entity> _targetEntities;
        private EntityManager _entityManager;

        public void OnCreate(ref SystemState state)
        {
            _targetEntities = new NativeList<Entity>(Allocator.Persistent);
            _entityManager = state.EntityManager;

            NativeArray<Entity> entities = _entityManager.GetAllEntities(Allocator.Temp);
            foreach (Entity entity in entities)
            {
                if (_entityManager.HasComponent<EntityCastCheck_Component>(entity))
                {
                    _targetEntities.Add(entity);
                }
            }
        }

        private void OnUpdate(ref SystemState state)
        {
            if (_targetEntities.Length == 0)
            {
                TargetsSetUp();
            }

            foreach (Entity entity in _targetEntities)
            {
                RefRW<LocalToWorld> shadowTransform = SystemAPI.GetComponentRW<LocalToWorld>(entity);
                RefRO<EntityCastCheck_Component> targetingComponent =
                    SystemAPI.GetComponentRO<EntityCastCheck_Component>(entity);

                var cpm = targetingComponent.ValueRO;
                
                shadowTransform.ValueRW.Value = float4x4.TRS(cpm.TargetPosition, shadowTransform.ValueRO.Rotation, new float3(1f, 1f, 1f));

                PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                physicsWorldSingleton.CapsuleCastAll(cpm.CapsuleP1, cpm.CapsuleP2, cpm.TargetRadius, cpm.Direction, 10,
                    ref hits,
                    new CollisionFilter
                    {
                        BelongsTo = (uint)EnemiesLayer.Triggers,
                        CollidesWith = (uint)EnemiesLayer.Enemies
                    });

                foreach (ColliderCastHit hit in hits)
                {
                    state.EntityManager.DestroyEntity(hit.Entity);
                }
            }
        }

        private void TargetsSetUp()
        {
            NativeArray<Entity> entities = _entityManager.GetAllEntities(Allocator.Temp);
            foreach (Entity entity in entities)
            {
                if (_entityManager.HasComponent<EntityCastCheck_Component>(entity))
                {
                    _targetEntities.Add(entity);
                }
            }
        }
    }
}