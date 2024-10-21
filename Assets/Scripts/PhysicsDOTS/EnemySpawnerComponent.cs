using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace PhysicsDOTS
{
    public struct EnemySpawnerComponent : IComponentData
    {
        public float SpawnInterval;
        public BlobAssetReference<EnemySpawnerBlob> SpawnPositions;
    }

// Define a Blob structure to store an array of positions
    public struct EnemySpawnerBlob
    {
        public BlobArray<float3> SpawnPositions;
    }

}