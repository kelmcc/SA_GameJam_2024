using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using EnemyAI.Spawning;
using UnityEngine;

namespace Agents
{
    [RequireComponent(typeof(Collider))]
    public abstract class Damagable : MonoBehaviour
    {
        List<EnemyBase> _enemiesInRange = new List<EnemyBase>();

        private void Start()
        {
            TimeTicker.OnTick += OnTick;
        }

        private void OnTriggerEnter(Collider other)
        {
            var enemy = other.gameObject.GetComponent<EnemyBase>();
            if (enemy != null && !_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Add(enemy);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var enemy = other.gameObject.GetComponent<EnemyBase>();
            if (enemy != null && !_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Remove(enemy);
            }
        }

        private void OnTick()
        {
            float totalDamage = _enemiesInRange.Sum(enemy => enemy.DamagePerHit);

            TakeDamage(totalDamage);
        }

        protected abstract void TakeDamage(float damage);
    }
}