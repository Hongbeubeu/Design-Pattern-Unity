using UnityEngine;

namespace Builder
{
    public class DontDestroyOnLoads : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _objects;


        private void Awake()
        {
            DontDestroyOnLoad(this);
            foreach (var obj in _objects)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }
}