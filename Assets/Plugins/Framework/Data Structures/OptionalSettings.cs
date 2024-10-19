using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    [Serializable]
    public abstract class OptionalSettings
    {
        public bool Enabled;


        public static implicit operator bool(OptionalSettings settings)
        {
            return !object.ReferenceEquals(settings, null) && settings.Enabled;
        }

    }
}
