using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agents.ShipSystem
{
    public class MiningShip : Damagable
    {
        [SerializeField] private float _miningPower = 1;
        [SerializeField] private float _miningRewardPerTick = 10;

        [SerializeField] private float _health = 1000;

        [SerializeField] private float _shieldHealth = 100;
        [SerializeField] private float _shieldRegenPerTick = 1;
        [SerializeField] private float _shieldRegenTickDelay = 5;
        
        [SerializeField] private List<GameObject> _miningDebrisPrefabs;

        protected override void TakeDamage(float damage)
        {
            // First check if we have shields

            _health -= damage;

            if (_health <= 0)
            {
                OnDeath();
            }
        }

        private void OnDeath()
        {
            // Massive explosion

            // Let main know we have died
            Debug.LogError("Ship has died");
        }
    }
}