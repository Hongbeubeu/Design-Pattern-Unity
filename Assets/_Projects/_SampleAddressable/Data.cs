using UnityEngine;

namespace _Projects._SampleAddressable
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data", order = 0)]
    public class Data : ScriptableObject
    {
        public string prefabKey;
    }
}