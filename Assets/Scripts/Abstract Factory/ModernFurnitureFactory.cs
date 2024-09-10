using UnityEngine;

public class ModernFurnitureFactory : MonoBehaviour, IFurnitureFactory
{
    [SerializeField, RequireType(typeof(IChair))]
    private GameObject modernChairPrefab;

    [SerializeField, RequireType(typeof(ISofa))]
    private GameObject modernSofaPrefab;

    [SerializeField, RequireType(typeof(ICoffeeTable))]
    private GameObject modernCoffeeTablePrefab;

    public IChair CreateChair()
    {
        return modernChairPrefab.GetComponent<IChair>();
    }

    public ISofa CreateSofa()
    {
        return modernSofaPrefab.GetComponent<ISofa>();
    }

    public ICoffeeTable CreateCoffeeTable()
    {
        return modernCoffeeTablePrefab.GetComponent<ICoffeeTable>();
    }
}