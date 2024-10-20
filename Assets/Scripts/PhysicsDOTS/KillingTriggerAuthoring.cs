using Unity.Entities;
using UnityEngine;

namespace PhysicsDOTS
{
    public class KillingTriggerAuthoring : MonoBehaviour
    {
        public float sizeX;
        public float sizeY;
        public float sizeZ;

        public class KillingTriggerBaker : Baker<KillingTriggerAuthoring>
        {
            public override void Bake(KillingTriggerAuthoring authoring)
            {
                Entity killingTriggerAuthoring = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(killingTriggerAuthoring,
                    new KillingTriggerComponent()
                    {
                        sizeX = authoring.sizeX,
                        sizeY = authoring.sizeY,
                        sizeZ = authoring.sizeZ
                    });
            }
        }
    }
}