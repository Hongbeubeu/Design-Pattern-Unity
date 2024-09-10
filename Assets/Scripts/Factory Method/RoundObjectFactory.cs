using UnityEngine;
using Random = UnityEngine.Random;

public class RoundObjectFactory : MonoBehaviour, IObjectFactory
{
    [SerializeField] private GameObject[] roundObjects;

    public IShape CreateRandomObject()
    {
        var randomIndex = Random.Range(0, roundObjects.Length);
        var randomObject = roundObjects[randomIndex];
        return randomObject.GetComponent<IShape>();
    }
}