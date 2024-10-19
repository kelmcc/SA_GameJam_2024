using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class SerializableDateTime
    {
        public DateTime DateTime => DateTime.FromBinary(_binary);

        [SerializeField]
        private long _binary;
    }
}
