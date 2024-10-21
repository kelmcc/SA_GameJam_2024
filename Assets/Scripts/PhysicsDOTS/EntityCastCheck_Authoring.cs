using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsDOTS
{
    public class EntityCastCheck_Authoring : MonoBehaviour
    {
        public Transform TargetTransform;
        public Transform CapsuleP1;
        public Transform CapsuleP2;
        public float TargetRadius;
        public float TargetHeight;
        
        
        public class PlayerBaker : Baker<EntityCastCheck_Authoring>
        {
            public override void Bake(EntityCastCheck_Authoring authoring)
            {
                Entity playerAuthor = GetEntity(TransformUsageFlags.None);

                AddComponent(playerAuthor, new EntityCastCheck_Component
                {
                    TargetPosition = authoring.TargetTransform.position,
                    TargetRadius = authoring.TargetRadius,
                    TargetHeight = authoring.TargetHeight,
                    CapsuleP1 = authoring.CapsuleP1.position,
                    CapsuleP2 = authoring.CapsuleP2.position,
                    Direction = authoring.TargetTransform.forward,
                });
            }
        }

    }
}