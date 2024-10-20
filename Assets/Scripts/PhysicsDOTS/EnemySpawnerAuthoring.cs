using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsDOTS
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        public float SpawnInterval = 1f;
        public List<EnemySO> EnemyDataSO;
        public List<float3> SpawnPoints;

        public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                Entity enemySpawnerAuthor = GetEntity(TransformUsageFlags.None);

                AddComponent(enemySpawnerAuthor, new EnemySpawnerComponent
                {
                    SpawnInterval = authoring.SpawnInterval,
                    SpawnPosition_1 = authoring.SpawnPoints[0],
                    SpawnPosition_2 = authoring.SpawnPoints[1],
                    SpawnPosition_3 = authoring.SpawnPoints[2],
                });

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

                AddComponentObject(enemySpawnerAuthor, new EnemyDataContainer { enemyData = enemyData });
            }
        }
    }
}