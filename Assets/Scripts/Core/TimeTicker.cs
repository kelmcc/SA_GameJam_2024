using System;
using UnityEngine;

namespace Core
{
    // TODO: Expand to keep track of day cycle
    public class TimeTicker : MonoBehaviourSingleton<TimeTicker>
    {
        public static event Action OnTick;
        
        private const float TICK_INTERVAL = 1f;
        
        private float _tickTimer = 0;

        private TimeTicker()
        {
        }

        private void Update()
        {
            _tickTimer += Time.deltaTime;
            
            if (_tickTimer >= TICK_INTERVAL)
            {
                _tickTimer = 0f;
                OnTick?.Invoke();
            }
        }
    }
}