using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Unity.VisualScripting;
using UnityEngine;

namespace EnemyAI.Spawning
{
    public class Death : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            EnemyBase enemyBase = other.gameObject.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.OnDeath();
                EnemyPool.Instance.Pool.Release(enemyBase);
            }
            
            Player player = other.gameObject.GetComponent<Player>();
            if (player!= null)
            {
                player.OnFall();
            }
            
            Pickup pickup = other.gameObject.GetComponent<Pickup>();
            if (pickup)
            {
                pickup.PickedUp();
            }
        }

    }
}
