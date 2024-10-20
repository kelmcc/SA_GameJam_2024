using Unity.Entities;
using Unity.Mathematics;

namespace PhysicsDOTS
{
    public struct PlayerComponent : IComponentData
    {
        public float3 PlayerPosition;
    }
}