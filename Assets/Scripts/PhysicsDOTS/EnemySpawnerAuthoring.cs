using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PhysicsDOTS
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        public float SpawnInterval = 1f;
        public List<EnemySO> EnemyDataSO;
        public List<Transform> SpawnPoints;
        public int SpawnCountPerTick = 1;

        public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                Entity enemySpawnerAuthor = GetEntity(TransformUsageFlags.None);

                // Convert SpawnPoints into a BlobAssetReference<BlobArray<float3>>
                using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
                {
                    ref var blobAsset = ref builder.ConstructRoot<EnemySpawnerBlob>();
                    var spawnPositionsArray =
                        builder.Allocate(ref blobAsset.SpawnPositions, authoring.SpawnPoints.Count);

                    for (int i = 0; i < authoring.SpawnPoints.Count; i++)
                    {
                        spawnPositionsArray[i] = authoring.SpawnPoints[i].position;
                    }

                    BlobAssetReference<EnemySpawnerBlob> blobReference =
                        builder.CreateBlobAssetReference<EnemySpawnerBlob>(Allocator.Persistent);

                    // Add the EnemySpawnerComponent with the BlobAssetReference to the entity
                    AddComponent(enemySpawnerAuthor, new EnemySpawnerComponent
                    {
                        SpawnInterval = authoring.SpawnInterval,
                        SpawnCountPerTick = authoring.SpawnCountPerTick,
                        SpawnPositions = blobReference
                    });
                }

                // Create EnemyData list
                List<EnemyData> enemyData = new List<EnemyData>();

                foreach (var enemySO in authoring.EnemyDataSO)
                {
                    enemyData.Add(new EnemyData
                    {
                        Prefab = GetEntity(enemySO.Prefab, TransformUsageFlags.None),
                        Health = enemySO.Health,
                        Speed = enemySO.Speed,
                        Level = enemySO.Level,
                        BaseDamage = enemySO.BaseDamage,
                        DamageMultiplier = enemySO.DamageMultiplier,
                        Scale = enemySO.Scale
                    });
                }

                // Add EnemyDataContainer to the entity as a managed component
                AddComponentObject(enemySpawnerAuthor, new EnemyDataContainer { enemyData = enemyData });
            }
        }
    }
}