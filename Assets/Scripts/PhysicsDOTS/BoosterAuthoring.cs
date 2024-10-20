using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsDOTS
{
    public class BoosterAuthoring : MonoBehaviour
    {
        public float sizeX;
        public float sizeY;
        public float sizeZ;
        
        public float3 velocityDirection;

        public class BoosterBaker : Baker<BoosterAuthoring>
        {
            public override void Bake(BoosterAuthoring authoring)
            {
                Entity boosterAuthoring = GetEntity(TransformUsageFlags.None);

                AddComponent(boosterAuthoring,
                    new BoosterComponent()
                    {
                        sizeX = authoring.sizeX,
                        sizeY = authoring.sizeY,
                        sizeZ = authoring.sizeZ,
                        velocityDirection = authoring.velocityDirection
                    });
            }
        }
    }
}