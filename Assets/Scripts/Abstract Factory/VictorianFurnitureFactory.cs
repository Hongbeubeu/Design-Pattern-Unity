using hcore.Attributes;
using UnityEngine;

public class VictorianFurnitureFactory : MonoBehaviour, IFurnitureFactory
{
    [SerializeField, RequireType(typeof(IChair))]
    private GameObject victorianChairPrefab;

    [SerializeField, RequireType(typeof(ISofa))]
    private GameObject victorianSofaPrefab;

    [SerializeField, RequireType(typeof(ICoffeeTable))]
    private GameObject victorianCoffeeTablePrefab;

    public IChair CreateChair()
    {
        return victorianChairPrefab.GetComponent<IChair>();
    }

    public ISofa CreateSofa()
    {
        return victorianSofaPrefab.GetComponent<ISofa>();
    }

    public ICoffeeTable CreateCoffeeTable()
    {
        return victorianCoffeeTablePrefab.GetComponent<ICoffeeTable>();
    }
}