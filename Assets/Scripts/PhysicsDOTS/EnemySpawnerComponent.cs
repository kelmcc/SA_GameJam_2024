using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace PhysicsDOTS
{
    public struct EnemySpawnerComponent : IComponentData
    {
        public float SpawnInterval;
        public float3 SpawnPosition_1;
        public float3 SpawnPosition_2;
        public float3 SpawnPosition_3;
    }
}