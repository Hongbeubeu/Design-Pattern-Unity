using System.IO;
using MessagePack;
using UnityEngine;

public interface ITestSavableEntity : ISavableEntity<ITestSavableEntityConfigData, ITestSaveData>
{
}

public interface ITestSavableEntityConfigData : IBaseSavableEntityConfigData
{
}

public interface ITestSaveData : ISaveData
{
    int TestValue { get; set; }
}

public class TestSavableEntity : BaseSavableEntity<ITestSavableEntityConfigData, ITestSaveData>, ITestSavableEntity
{
    public TestSavableEntity(string id) : base(id)
    {
    }
}

public class TestSaveData : BaseSaveData, ITestSaveData
{
    [Key(1)]
    public int TestValue
    {
        get => _testValue;
        set
        {
            _testValue = value;
            Serialize();
        }
    }

    private int _testValue;

    public TestSaveData(string id) : base(id)
    {
    }

    public override void Serialize()
    {
        var serializedData = MessagePackSerializer.Serialize(this);
        File.WriteAllBytes(SavePath, serializedData);
    }

    public override void Deserialize()
    {
        if (!CanDeserialize())
        {
            Debug.Log($"{Id} hasn't serialized. Terminate deserialize");
            return;
        }

        var data = File.ReadAllBytes(SavePath);
        var deserialized = MessagePackSerializer.Deserialize<TestSaveData>(data);

        _testValue = deserialized.TestValue;
    }
}