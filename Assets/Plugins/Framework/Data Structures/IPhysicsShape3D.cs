using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public interface IPhysicsShape3D
    {
        Collider[] GetOverlappingColliders(LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal);
        int GetOverlappingCollidersNonAlloc(Collider[] results, LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal);
    }
}
