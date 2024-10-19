using Unity.Entities;
using Unity.Physics;
using Unity.Collections;
using Unity.Mathematics;

namespace PhysicsDOTS
{
    public partial struct EnemySystem : ISystem
    {
        InputComponent inputComponent;

        private void OnUpdate(ref SystemState state)
        {
            EntityManager entityManager = state.EntityManager;

            if (!SystemAPI.TryGetSingleton(out inputComponent))
            {
                return;
            }

            NativeArray<Entity> entities = entityManager.GetAllEntities(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                if (entityManager.HasComponent<EnemyComponent>(entity))
                {
                    EnemyComponent enemyComponent = entityManager.GetComponentData<EnemyComponent>(entity);

                    RefRW<PhysicsVelocity> physicsVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(entity);
                    
                    physicsVelocity.ValueRW.Linear += new float3(inputComponent.movement.x * enemyComponent.MoveSpeed * SystemAPI.Time.DeltaTime, 0,
                        inputComponent.movement.y * enemyComponent.MoveSpeed * SystemAPI.Time.DeltaTime);
                }
            }

            entities.Dispose();
        }
    }
}