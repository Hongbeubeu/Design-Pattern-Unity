using System.Collections.Generic;
using UnityEngine;

public class FactoryMethodTestDrive : MonoBehaviour
{
    [SerializeField] private GameObject[] factories;
    private readonly List<IObjectFactory> _factories = new();
    private int _index;

    private void Start()
    {
        foreach (var factory in factories)
        {
            _factories.Add(factory.GetComponent<IObjectFactory>());
        }
    }

    private void Update()
    {
        if (_factories == null || _factories.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _index++;
            if (_index >= _factories.Count)
            {
                _index = 0;
            }
        }

        if (!Input.GetKeyDown(KeyCode.Space)) return;

        var randomObject = _factories[_index].CreateRandomObject();
        Debug.Log(randomObject.GetType().Name);
        var randomPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        Instantiate((MonoBehaviour)randomObject, randomPosition, Quaternion.identity);
    }
}