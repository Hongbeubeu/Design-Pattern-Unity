using UnityEngine;

public class TestEntity : BaseEntity<ITestEntityConfigData>, ITestEntity
{
    public TestEntity(string id) : base(id)
    {
    }

    public void DoSomething()
    {
        Debug.Log(ConfigData.Number);
    }
}