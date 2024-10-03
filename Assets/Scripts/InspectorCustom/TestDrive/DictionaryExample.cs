using System;
using System.Collections.Generic;
using UnityEngine;

namespace InspectorCustom.TestDrive
{
    public class DictionaryExample : MonoBehaviour
    {
        [Dictionary] public SerializableDictionary<ListString, int> testDictionary = new();
        [SerializeField] public List<(string, string)> testList = new();

        [Serializable]
        public class ListString : List<SerializableDictionary<int, int>>
        {
            [SerializeField] private List<SerializableDictionary<int, int>> strings = new();

            public List<SerializableDictionary<int, int>> Strings => strings;
        }
    }
}