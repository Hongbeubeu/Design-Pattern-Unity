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
       

        private Dictionary<object, Pool> _poolDictionary = new();

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
            var key = targetObject;
            _poolDictionary ??= new Dictionary<object, Pool>();
            if (_poolDictionary.ContainsKey(key)) return;
            var pool = new Pool(() => createFunc(), poolSize, additionalSize);
            _poolDictionary.Add(key, pool);
        }

        public T GetObject<T>(object targetObject) where T : IPoolable
        {
            if (!_poolDictionary.TryGetValue(targetObject, out var pool))
            {
                Debug.LogWarning($"Pool with type {targetObject} doesn't exist.");
                return default;
            }

            var obj = (T)pool.Get();
            obj.OnSpawn();

            return obj;
        }

        public void ReturnObject(object key, object objectToReturn)
        {
            if (!_poolDictionary.TryGetValue(key, value: out var pool))
            {
                Debug.LogWarning($"Pool with type {key} doesn't exist.");
                return;
            }

            var obj = (IPoolable)objectToReturn;
            obj.OnReturn();
            pool.Return(obj);
        }
    }
}