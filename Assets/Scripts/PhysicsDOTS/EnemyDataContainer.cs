using System.Collections.Generic;
using Unity.Entities;

namespace PhysicsDOTS
{
    public class EnemyDataContainer : IComponentData
    {
        public List<EnemyData> enemyData;
    }

    public struct EnemyData
    {
        public int Level;
        public int BaseDamage;
        public int DamageMultiplier;
        public float Speed;
        public float Health;
        public Entity Prefab;
        public float Scale;
    }
}