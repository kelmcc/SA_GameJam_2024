using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public struct GameObjectFloatPair
    {
        public GameObject GameObject
        {
            get => _gameObject;
            set => _gameObject = value;
        }
        public float Number
        {
            get => _number;
            set => _number = value;
        }

        [SerializeField]
        private GameObject _gameObject;
        [SerializeField]
        private float _number;

        public GameObjectFloatPair(GameObject gameObject, float number)
        {
            _gameObject = gameObject;
            _number = number;
        }
    }
}

