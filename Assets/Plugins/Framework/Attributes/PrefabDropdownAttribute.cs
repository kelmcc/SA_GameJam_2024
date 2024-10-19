using System;
using UnityEngine;

namespace Framework
{
    public class PrefabDropdownAttribute : PropertyAttribute
    {
        public Type Type => _type;
        public string[] Paths => _paths;
        public bool AllowNull => _allowNull;

        private string[] _paths;
        private Type _type;
        private bool _allowNull = true;

        public PrefabDropdownAttribute(string path, bool allowNull = true)
        {
            _allowNull = allowNull;
            _paths = string.IsNullOrEmpty(path) ? null : new[] { path };
        }

        public PrefabDropdownAttribute(bool allowNull = true, params string[] paths)
        {
            _allowNull = allowNull;
            _paths = paths;
        }

        public PrefabDropdownAttribute(Type type, string path, bool allowNull = true)
        {
            _allowNull = allowNull;
            _paths = string.IsNullOrEmpty(path) ? null : new[] { path };

            if (typeof(Component).IsAssignableFrom(type))
            {
                _type = type;
            }
            else
            {
                throw new ArgumentException("Type is not a component type: " + type);
            }
        }

        public PrefabDropdownAttribute(Type type, bool allowNull = true, params string[] paths)
        {
            _allowNull = allowNull;
            _paths = paths;

            if (typeof(Component).IsAssignableFrom(type))
            {
                _type = type;
            }
            else
            {
                throw new ArgumentException("Type is not a component type: " + type);
            }
        }

        public PrefabDropdownAttribute(Type type, bool allowNull = true)
        {
            _allowNull = allowNull;

            if (typeof(Component).IsAssignableFrom(type))
            {
                _type = type;
            }
            else
            {
                throw new ArgumentException("Type is not a component type: " + type);
            }
        }

        public PrefabDropdownAttribute(bool allowNull = true)
        {
            _allowNull = allowNull;
            _paths = new[] { "Assets/Prefabs" };
        }

    }
}
