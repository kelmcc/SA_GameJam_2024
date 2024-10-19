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
        }

        private void OnTriggerExit(Collider other)
        {
            
        }
        
        private void OnTriggerStay(Collider other)
        {
            
        }
    }
}
