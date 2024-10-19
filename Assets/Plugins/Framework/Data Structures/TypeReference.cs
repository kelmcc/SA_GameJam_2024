using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Framework
{

    [Serializable]
    public class TypeReference
    {

        public Type Type
        {
            get
            {
                if (_type == null)
                {
                    if (string.IsNullOrEmpty(_typeName)) return null;

                    _type = TypeUtils.GetTypeFromName(_typeName);
                }

                return _type;
            }
        }

        public TypeReference(Type type)
        {
            _typeName = type.Name;
            _type = type;
        }

        [SerializeField]
        private string _typeName;

        private Type _type;


    }
}
