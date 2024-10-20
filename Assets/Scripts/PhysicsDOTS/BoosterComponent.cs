using Unity.Entities;
using Unity.Mathematics;

namespace PhysicsDOTS
{
    public struct BoosterComponent : IComponentData
    {
        public float sizeX;
        public float sizeY;
        public float sizeZ;
        public float3 velocityDirection;
    }
}