using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace PhysicsDOTS
{
    public partial class EnemySpawnerSystem : SystemBase
    {
        private Entity enemySpawnerEntity;
        private float nextSpawnTime;
        private Random random;

        protected override void OnCreate()
        {
            base.OnCreate();
            random = new Random(1); // Initialize with a fixed seed
        }

        protected override void OnUpdate()
        {
            // Check if the singleton entity exists
            if (!SystemAPI.TryGetSingletonEntity<EnemySpawnerComponent>(out enemySpawnerEntity))
            {
                return;
            }

            // Initialize random with a unique seed if not already initialized
            if (random.state == 0)
            {
                random = new Random((uint)(enemySpawnerEntity.Index + 1));
            }

            // Check if it's time to spawn enemies
            if (SystemAPI.Time.ElapsedTime < nextSpawnTime)
            {
                return;
            }

            // Retrieve components
            EnemySpawnerComponent enemySpawnerComponent = EntityManager.GetComponentData<EnemySpawnerComponent>(enemySpawnerEntity);
            EnemyDataContainer enemyDataContainer = EntityManager.GetComponentData<EnemyDataContainer>(enemySpawnerEntity);

            List<EnemyData> availableEnemies = enemyDataContainer.enemyData;
            int enemyCount = availableEnemies.Count;

            // Calculate cumulative weights
            int totalWeight = 0;
            NativeArray<int> cumulativeWeights = new NativeArray<int>(enemyCount, Allocator.Temp);
            for (int i = 0; i < enemyCount; i++)
            {
                totalWeight += availableEnemies[i].Level;
                cumulativeWeights[i] = totalWeight;
            }

            // Select an enemy based on weighted probability
            int randomValue = random.NextInt(0, totalWeight);
            EnemyData selectedEnemy = availableEnemies[0];
            for (int i = 0; i < enemyCount; i++)
            {
                if (randomValue < cumulativeWeights[i])
                {
                    selectedEnemy = availableEnemies[i];
                    break;
                }
            }

            cumulativeWeights.Dispose();

            // Spawn enemies
            ref BlobArray<float3> spawnPositions = ref enemySpawnerComponent.SpawnPositions.Value.SpawnPositions;
            int spawnPositionsCount = spawnPositions.Length;
            for (int i = 0; i < spawnPositionsCount; i++)
            {
                for (int j = 0; j < enemySpawnerComponent.SpawnCountPerTick; j++)
                {
                    SpawnEnemy(spawnPositions[i], selectedEnemy);
                }
            }

            // Update next spawn time
            nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + enemySpawnerComponent.SpawnInterval;
        }

        private void SpawnEnemy(float3 position, EnemyData enemyData)
        {
            Entity newEnemy = EntityManager.Instantiate(enemyData.Prefab);
            EntityManager.SetComponentData(newEnemy, new LocalTransform
            {
                Position = AddJitter(position),
                Rotation = quaternion.identity,
                Scale = enemyData.Scale
            });
            EntityManager.AddComponent<EnemyTag>(newEnemy);
        }

        private float3 AddJitter(float3 position)
        {
            return position + random.NextFloat3(new float3(-2f), new float3(2f));
        }
    }
}