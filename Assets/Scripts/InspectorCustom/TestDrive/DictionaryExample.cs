using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InspectorCustom.TestDrive
{
    public class DictionaryExample : MonoBehaviour
    {
        [DictionarySerializeField] public SerializableDictionary<string, ListString> testSerializableDictionary = new();

        [Serializable]
        public class ListString : List<string>
        {
            [SerializeField] private List<string> strings = new();

            public List<string> Strings => strings;
        }

        public enum MyEnum
        {
            TypeA,
            TypeB,
            TypeC,
            TypeD,
            TypeE
        }
    }
}