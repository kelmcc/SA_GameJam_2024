using Unity.Entities;
using Unity.Mathematics;

namespace PhysicsDOTS
{
    public struct EntityCastCheck_Component : IComponentData
    {
        public float3 TargetPosition;
        public float3 CapsuleP1;
        public float3 CapsuleP2;
        public float3 Direction;
        public float TargetRadius;
        public float TargetHeight;

    }
}