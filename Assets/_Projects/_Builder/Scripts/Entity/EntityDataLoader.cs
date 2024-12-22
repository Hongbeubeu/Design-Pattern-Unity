using System.Collections.Generic;
using UnityEngine;

public class EntityDataLoader : MonoBehaviour
{
    [SerializeField]
    private BaseEntityConfigData[] _configDatas;

    [SerializeField]
    private BaseSavableEntityConfigData[] _savableConfigDatas;

    public List<IEntity> Entities { get; } = new();


    private List<ISaveData> _baseSaveDatas = new();

    private void Awake()
    {
        Entities.Clear();
        _baseSaveDatas.Clear();
        if (_configDatas is { Length: > 0 })
            foreach (var configData in _configDatas)
            {
                var entity = configData.CreateEntity();
                entity.ProvideConfigData(configData);
                Entities.Add(entity);
            }

        if (_savableConfigDatas is { Length: > 0 })
            foreach (var configData in _savableConfigDatas)
            {
                var entity = configData.CreateEntity();
                entity.ProvideConfigData(configData);
                Entities.Add(entity);
                var saveData = configData.CreateSaveData();
                saveData.Deserialize();
                _baseSaveDatas.Add(saveData);
                (entity as IProvideSaveData)?.ProvideSaveData(saveData);
            }
    }

    [Button]
    public void DoSomething()
    {
        foreach (var entity in Entities)
        {
            if (entity is ITestEntity testEntity)
            {
                testEntity.DoSomething();
            }
        }
    }

    [Button]
    public void ModifyTestSaveData()
    {
        foreach (var entity in Entities)
        {
            if (entity is ITestSavableEntity testSavableEntity)
            {
                Debug.Log($"Test value: {testSavableEntity.SaveData.TestValue}");
                testSavableEntity.SaveData.TestValue++;
            }
        }
    }
}