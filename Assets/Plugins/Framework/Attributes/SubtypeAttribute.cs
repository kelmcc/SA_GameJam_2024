using System;
using UnityEngine;

namespace Framework
{
    public class SubtypeAttribute : PropertyAttribute
    {
        public Type Type => _type;

        private Type _type;

        public SubtypeAttribute(Type type)
        {
            _type = type;
        }
    }
}
