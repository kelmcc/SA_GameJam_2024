using UnityEngine;

namespace PhysicsDOTS
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "GameData/Enemy Data")]
    public class EnemySO : ScriptableObject
    {
        public int Level;
        public int BaseDamage;
        public int DamageMultiplier;
        public float Speed;
        public float Health;
        public GameObject Prefab;
        public float Scale;
    }
}