using System.Collections.Generic;
using hcore.Attributes;
using UnityEngine;

namespace hcore.ObjectPooler.TestDrive
{
    public class ObjectPoolTestDrive : MonoBehaviour
    {
        [SerializeField, RequireType(typeof(IPoolable))]
        private GameObject[] prefabs;


        private readonly List<IPoolable> _items = new();

        private void Start()
        {
            foreach (var prefab in prefabs)
            {
                ObjectPoolManager.Instance.CreatePool(() => Instantiate(prefab).GetComponent<IPoolable>(), prefab, 5, 2);
            }
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                var obj = ObjectPoolManager.Instance.GetObject<PoolableMonoBehaviourBase>(prefabs[0]);
                if (!obj) return;
                _items.Add(obj);
                var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                obj.transform.position = mousePosition;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                var obj = ObjectPoolManager.Instance.GetObject<PoolableMonoBehaviourBase>(prefabs[1]);
                if (!obj) return;
                _items.Add(obj);
                var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                obj.transform.position = mousePosition;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (_items.Count == 0) return;
                var obj = _items[0];
                _items.RemoveAt(0);
                obj.ReturnToPool();
            }
        }
    }
}