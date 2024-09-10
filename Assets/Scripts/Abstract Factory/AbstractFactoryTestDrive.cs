using System.Collections.Generic;
using UnityEngine;

public class AbstractFactoryTestDrive : MonoBehaviour
{
    [SerializeField] private GameObject[] furniturePrefabs;
    private readonly List<IFurnitureFactory> _factory = new();
    private int _index;

    private void Start()
    {
        foreach (var prefab in furniturePrefabs)
        {
            _factory.Add(prefab.GetComponent<IFurnitureFactory>());
        }
    }

    private void Update()
    {
        if (_factory == null) return;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _index++;
            if (_index >= _factory.Count)
            {
                _index = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            var chair = _factory[_index].CreateChair();
            Instantiate((MonoBehaviour)chair, GetRandomPosition(), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            var sofa = _factory[_index].CreateSofa();
            Instantiate((MonoBehaviour)sofa, GetRandomPosition(), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            var coffeeTable = _factory[_index].CreateCoffeeTable();
            Instantiate((MonoBehaviour)coffeeTable, GetRandomPosition(), Quaternion.identity);
        }
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
    }
}