using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{

    public static class TypeUtils
    {

        private static Dictionary<string, Type> _typeCache;
        private static Assembly[] _assemblies;

        public static Type GetTypeFromName(string typeName)
        {

            if (_typeCache == null)
            {
                _typeCache = new Dictionary<string, Type>();
                _assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }


            if (!_typeCache.TryGetValue(typeName, out Type type))
            {
                for (int i = 0; i < _assemblies.Length; i++)
                {
                    type = _assemblies[i].GetType(typeName);
                    if (type != null)
                    {
                        _typeCache.Add(typeName, type);
                        break;
                    }
                }

                if (type == null)
                {
                    for (int i = 0; i < _assemblies.Length; i++)
                    {
                        Type[] types = _assemblies[i].GetExportedTypes();
                        for (int j = 0; j < types.Length; j++)
                        {
                            if (types[j].Name == typeName)
                            {
                                type = types[j];
                                _typeCache.Add(typeName, types[j]);
                                break;
                            }
                        }

                        if (type != null) break;
                    }
                }
            }


            if (type == null)
            {
                throw new ArgumentException("Type not found: " + typeName);
            }

            return type;
        }

        public static FieldInfo[] GetSerializedFields<T>() where T : Object
        {
            return GetSerializedFields(typeof(T));
        }


        public static FieldInfo[] GetSerializedFields(Type type)
        {
            List<FieldInfo> results = new List<FieldInfo>(type.GetFields(BindingFlags.Instance | BindingFlags.Public));

            FieldInfo[] privateFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < privateFields.Length; i++)
            {
                if (privateFields[i].GetAttribute<SerializeField>(true) != null)
                {
                    results.Add(privateFields[i]);
                }
            }

            return results.ToArray();
        }
    }
}
