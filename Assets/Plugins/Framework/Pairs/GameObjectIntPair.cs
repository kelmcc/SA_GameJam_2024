using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public struct GameObjectIntPair
    {
        public GameObject GameObject
        {
            get => _gameObject;
            set => _gameObject = value;
        }
        public int Number
        {
            get => _number;
            set => _number = value;
        }

        [SerializeField]
        private GameObject _gameObject;
        [SerializeField]
        private int _number;

        public GameObjectIntPair(GameObject gameObject, int number)
        {
            _gameObject = gameObject;
            _number = number;
        }
    }
}

