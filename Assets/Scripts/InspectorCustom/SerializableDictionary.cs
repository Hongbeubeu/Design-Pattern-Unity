using System;
using System.Collections.Generic;
using UnityEngine;

namespace hcore.InspectorCustom
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        [SerializeField] private List<TKey> keys = new();
        [SerializeField] private List<TValue> values = new();

        public Dictionary<TKey, TValue> ToDictionary()
        {
            var dict = new Dictionary<TKey, TValue>();
            for (var i = 0; i < keys.Count; i++)
            {
                if (!dict.ContainsKey(keys[i]))
                {
                    dict.Add(keys[i], values[i]);
                }
            }

            return dict;
        }

        public void FromDictionary(Dictionary<TKey, TValue> dict)
        {
            keys.Clear();
            values.Clear();
            foreach (var kvp in dict)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                var index = keys.IndexOf(key);
                return index >= 0 ? values[index] : default;
            }

            set
            {
                var index = keys.IndexOf(key);
                if (index >= 0)
                {
                    values[index] = value;
                }
                else
                {
                    keys.Add(key);
                    values.Add(value);
                }
            }
        }

        public int Count => keys.Count;
    }
}