using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace PhysicsDOTS
{
    public partial class EnemySpawnerSystem : SystemBase
    {
        private EnemySpawnerComponent enemySpawnerComponent;
        private EnemyDataContainer enemyDataContainer;
        private Entity enemySpawnerEntity;
        private float nextSpawnTime;
        private Random random;

        protected override void OnCreate()
        {
            base.OnCreate();
            random = Random.CreateFromIndex((uint)enemySpawnerComponent.GetHashCode());
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingletonEntity<EnemySpawnerComponent>(out enemySpawnerEntity))
            {
                return;
            }

            enemySpawnerComponent = EntityManager.GetComponentData<EnemySpawnerComponent>(enemySpawnerEntity);
            enemyDataContainer = EntityManager.GetComponentData<EnemyDataContainer>(enemySpawnerEntity);

            if (SystemAPI.Time.ElapsedTime > nextSpawnTime)
            {
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            int level = 1;
            List<EnemyData> availableEnemies = new List<EnemyData>();

            foreach (var enemyData in enemyDataContainer.enemyData)
            {
                if (enemyData.Level <= level)
                {
                    availableEnemies.Add(enemyData);
                }
            }

            int index = random.NextInt(0, enemySpawnerComponent.SpawnPositions.Value.SpawnPositions.Length);

            Entity newEnemy = EntityManager.Instantiate((availableEnemies[1].Prefab));
            EntityManager.SetComponentData(newEnemy, new LocalTransform()
            {
                Position = AddJitter(enemySpawnerComponent.SpawnPositions.Value.SpawnPositions[index]),
                Rotation = quaternion.identity,
                Scale = availableEnemies[1].Scale
            });
            
            EntityManager.AddComponent<EnemyTag>(newEnemy);
            
            nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + enemySpawnerComponent.SpawnInterval;
        }

        private float3 AddJitter(float3 position)
        {
            float3 jitter = new float3(random.NextFloat(-2, 2) + position.x, random.NextFloat(-2, 2) + position.y,
                random.NextFloat(-2, 2) + position.z);
            return jitter;
        }

        private void Scale(Entity entity, float scale)
        {
            RefRW<LocalToWorld> transform = SystemAPI.GetComponentRW<LocalToWorld>(entity);
            transform.ValueRW.Value.c0 = new float4(scale, 0, 0, 0);
            transform.ValueRW.Value.c1 = new float4(0, scale, 0, 0);
            transform.ValueRW.Value.c2 = new float4(0, 0, scale, 0);
        }
    }
}