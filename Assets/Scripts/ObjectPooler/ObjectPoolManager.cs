using System;
using System.Collections.Generic;
using Singleton;
using UnityEngine;

namespace ObjectPooler
{
    /// <summary>
    /// Singleton class that manages object pools
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private class Pool
        {
            private readonly Queue<IPoolable> _objectQueue;
            private readonly Func<IPoolable> _createFunc;
            private int _additionalSize;

            public Pool(Func<IPoolable> createFunc, int initialSize, int additionalSize)
            {
                _createFunc = createFunc;
                _additionalSize = additionalSize;
                _objectQueue = new Queue<IPoolable>();
                for (var i = 0; i < initialSize; i++)
                {
                    var obj = createFunc();
                    obj.OnReturn();
                    _objectQueue.Enqueue(obj);
                }
            }

            private void AddAdditionalObjects()
            {
                // Ensure that the additional size is at least 1
                if (_additionalSize == 0)
                {
                    _additionalSize = 1;
                }

                for (var i = 0; i < _additionalSize; i++)
                {
                    var obj = _createFunc();
                    obj.OnReturn();
                    _objectQueue.Enqueue(obj);
                }
            }

            public IPoolable Get()
            {
                if (_objectQueue.Count == 0)
                {
                    AddAdditionalObjects();
                }

                var obj = _objectQueue.Dequeue();
                return obj;
            }

            public void Return(IPoolable obj)
            {
                obj.OnReturn();
                _objectQueue.Enqueue(obj);
            }
        }

        private Dictionary<Type, Pool> _poolDictionary = new();

        /// <summary>
        /// Create a pool of objects
        /// </summary>
        /// <param name="createFunc"></param>
        /// <param name="targetObject"></param>
        /// <param name="poolSize"></param>
        /// <param name="additionalSize"></param>
        /// <typeparam name="T"></typeparam>
        public void CreatePool<T>(Func<T> createFunc, object targetObject, int poolSize, int additionalSize) where T : IPoolable
        {
            var key = targetObject.GetType();
            _poolDictionary ??= new Dictionary<Type, Pool>();
            if (!_poolDictionary.ContainsKey(key))
            {
                var pool = new Pool(() => createFunc(), poolSize, additionalSize);
                _poolDictionary.Add(key, pool);
            }
        }

        public T GetObject<T>(object targetObject) where T : IPoolable
        {
            var key = targetObject.GetType();

            if (!_poolDictionary.TryGetValue(key, out var pool))
            {
                Debug.LogWarning($"Pool with type {key} doesn't exist.");
                return default;
            }

            var obj = (T)pool.Get();
            obj.OnSpawn();

            return obj;
        }

        public void ReturnObject<T>(T objectToReturn) where T : IPoolable
        {
            var key = typeof(T);

            if (!_poolDictionary.TryGetValue(key, value: out var pool))
            {
                Debug.LogWarning($"Pool with type {key} doesn't exist.");
                return;
            }

            objectToReturn.OnReturn();
            pool.Return(objectToReturn);
        }
    }
}