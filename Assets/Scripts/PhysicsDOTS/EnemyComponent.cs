using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace PhysicsDOTS
{
    public struct EnemyComponent : IComponentData
    {
        public float MoveSpeed;
    }
}