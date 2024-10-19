using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{
    public class EnumMaskAttribute : PropertyAttribute
    {
        public Type Type => _type;
        private Type _type;

        public EnumMaskAttribute(Type type)
        {
            Assert.IsTrue(type.IsEnum);
            _type = type;
        }
    }
}
