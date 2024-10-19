using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{
    /// <summary>
    /// Helps serialize arrays or lists where an enum will be used as an index.
    /// </summary>
    public class EnumIndexedAttribute : PropertyAttribute
    {
        public Type Type => _type;
        private Type _type;

        public EnumIndexedAttribute(Type type)
        {
            Assert.IsTrue(type.IsEnum);
            _type = type;
        }

    }
}
