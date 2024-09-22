using UnityEngine;

namespace ObjectPooler
{
    public class Item : MonoBehaviour, IPoolable
    {
        public void OnSpawn()
        {
            gameObject.SetActive(true);
        }

        public void OnReturn()
        {
            gameObject.SetActive(false);
        }
    }
}