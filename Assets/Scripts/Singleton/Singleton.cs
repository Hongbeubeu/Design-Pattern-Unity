using UnityEngine;

namespace hcore.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new();
        
        // ReSharper disable once StaticMemberInGenericType
        private static bool _isSetDontDestroyOnLoad;

        public static T Instance
        {
            get
            {
                lock (Lock)
                {
                    if (!_instance)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));
                    }

                    if (!_instance)
                    {
                        var singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = $"(singleton) {typeof(T)}";
                        if (_isSetDontDestroyOnLoad) return _instance;
                        DontDestroyOnLoad(singleton);
                    }

                    else
                    {
                        if (_isSetDontDestroyOnLoad) return _instance;
                        DontDestroyOnLoad(_instance.gameObject);
                    }

                    _isSetDontDestroyOnLoad = true;
                }

                return _instance;
            }
        }

        /// <summary>
        /// Prevent any call from other class call Instance after the application is quitting
        /// </summary>
        private void OnDestroy()
        {
            _isSetDontDestroyOnLoad = false;
        }

#if UNITY_EDITOR
        public static void ResetInstanceForTest()
        {
            _instance = null;
            _isSetDontDestroyOnLoad = false;
        }
#endif
    }
}