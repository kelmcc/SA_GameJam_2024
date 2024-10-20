using Unity.Entities;

namespace PhysicsDOTS
{
    public struct KillingTriggerComponent : IComponentData
    {
        public float sizeX;
        public float sizeY;
        public float sizeZ;
    }
}