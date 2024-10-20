using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsDOTS
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public float3 PlayerPosition;
        
        
        public class PlayerBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity playerAuthor = GetEntity(TransformUsageFlags.None);

                AddComponent(playerAuthor, new PlayerComponent
                {
                    PlayerPosition = authoring.PlayerPosition,
                });
            }
        }

    }
}