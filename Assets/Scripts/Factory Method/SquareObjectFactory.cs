using UnityEngine;

public class SquareObjectFactory : MonoBehaviour, IObjectFactory
{
    [SerializeField] private GameObject[] squareObjects;

    public IShape CreateRandomObject()
    {
        var randomIndex = Random.Range(0, squareObjects.Length);
        var randomObject = squareObjects[randomIndex];
        return randomObject.GetComponent<IShape>();
    }
}