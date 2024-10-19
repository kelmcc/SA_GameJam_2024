using Unity.Entities;
using UnityEngine;

namespace PhysicsDOTS
{
    public class BoosterAuthoring : MonoBehaviour
    {
        public float size;

        public class BoosterBaker : Baker<BoosterAuthoring>
        {
            public override void Bake(BoosterAuthoring authoring)
            {
                Entity boosterAuthoring = GetEntity(TransformUsageFlags.None);

                AddComponent(boosterAuthoring,
                    new BoosterComponent()
                    {
                        size = authoring.size,
                    });
            }
        }
    }
}