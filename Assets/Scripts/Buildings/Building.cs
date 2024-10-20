using Agents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Damagable
{
    public abstract void SetInteractable();

    public abstract void SetPassive();
    protected abstract override void TakeDamage(float damage);
}
