using UnityEngine;

namespace Core
{
    public class MonoBehaviourSingleton<TSingletonType> : MonoBehaviour where TSingletonType : MonoBehaviour
    {
        private static TSingletonType _instance = null;

        public static TSingletonType Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TSingletonType>();
                    if (_instance == null)
                    {
                        _instance = new GameObject(typeof(TSingletonType).Name).AddComponent<TSingletonType>();
                    }

                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }
    }
}