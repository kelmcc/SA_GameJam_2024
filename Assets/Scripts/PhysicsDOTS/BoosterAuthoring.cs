using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsDOTS
{
    public class BoosterAuthoring : MonoBehaviour
    {
        public float BoosterStrength = 1f;
        public float CorrallingMultiplier = 1f;
        public Transform velocityDirection;

        public class BoosterBaker : Baker<BoosterAuthoring>
        {
            public override void Bake(BoosterAuthoring authoring)
            {
                Entity boosterAuthoring = GetEntity(TransformUsageFlags.Dynamic);
                Vector3 direction = authoring.velocityDirection.transform.position - authoring.transform.position;
                direction.Normalize();
                
                var localScale = authoring.transform.localScale;
                AddComponent(boosterAuthoring,
                    new BoosterComponent()
                    {
                        sizeX = localScale.x,
                        sizeY = localScale.y,
                        sizeZ = localScale.z,
                        velocityDirection = direction,
                        BoosterStrength = authoring.BoosterStrength,
                        CorrallingMultiplier = authoring.CorrallingMultiplier,
                    });
            }
        }
    }
}