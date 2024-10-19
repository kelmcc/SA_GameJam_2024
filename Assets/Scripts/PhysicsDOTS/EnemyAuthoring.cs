using Unity.Entities;
using UnityEngine;

namespace PhysicsDOTS
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public float MoveSpeed = 10;

        public class BallAuthoringBaker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                Entity enemyAuth = GetEntity(TransformUsageFlags.None);

                AddComponent(enemyAuth, new EnemyComponent { MoveSpeed = authoring.MoveSpeed });
            }
        }
    }
}