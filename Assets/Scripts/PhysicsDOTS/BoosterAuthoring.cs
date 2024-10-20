using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsDOTS
{
    public class BoosterAuthoring : MonoBehaviour
    {
        public float velocityStrength = 0f;
        public Transform velocityDirection;

        public class BoosterBaker : Baker<BoosterAuthoring>
        {
            public override void Bake(BoosterAuthoring authoring)
            {
                Entity boosterAuthoring = GetEntity(TransformUsageFlags.None);
                Vector3 direction = authoring.velocityDirection.transform.position - authoring.transform.position;
                direction.Normalize();
                
                //float3 velocity = direction * authoring.velocityStrength;
                
                var localScale = authoring.transform.localScale;
                AddComponent(boosterAuthoring,
                    new BoosterComponent()
                    {
                        sizeX = localScale.x,
                        sizeY = localScale.y,
                        sizeZ = localScale.z,
                        velocityDirection = direction
                    });
            }
        }
    }
}