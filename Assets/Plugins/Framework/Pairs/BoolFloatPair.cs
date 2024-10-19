using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    [Serializable]
    public struct BoolFloatPair
    {
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }


        public float Value
        {
            get => _value;
            set => _value = value;
        }

        [SerializeField]
        private bool _enabled;

        [SerializeField]
        private float _value;

        public BoolFloatPair(bool enabled, float floatValue)
        {
            _enabled = enabled;
            _value = floatValue;
        }
    }
}
