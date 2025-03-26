using System.Collections.Generic;
using UnityEngine;
using hcore.Tool;

namespace Builder.Entity
{
    public class EntityLoader : MonoBehaviour
    {
        [SerializeField]
        private BuilderStartup _builderStartup;

        [SerializeField]
        private BaseEntityConfigData[] _configDatas;

        [SerializeField]
        private BaseSavableEntityConfigData[] _savableConfigDatas;

        public List<IEntity> Entities { get; } = new();

        public List<ISaveData> BaseSaveDatas { get; } = new();

        private void Awake()
        {
            _builderStartup.onInstalled += Load;
        }

        private void Load()
        {
            Entities.Clear();
            BaseSaveDatas.Clear();
            if (_configDatas is { Length: > 0 })
                foreach (var configData in _configDatas)
                {
                    var entity = configData.CreateEntity();
                    entity.ProvideConfigData(configData);
                    Entities.Add(entity);
                    entity.Inject(_builderStartup.Resolver);
                    entity.Initialize();
                }

            if (_savableConfigDatas is { Length: > 0 })
                foreach (var configData in _savableConfigDatas)
                {
                    var entity = configData.CreateEntity();
                    entity.ProvideConfigData(configData);
                    Entities.Add(entity);
                    var saveData = configData.CreateSaveData();
                    saveData.Inject(_builderStartup.Resolver);
                    saveData.Deserialize();
                    BaseSaveDatas.Add(saveData);
                    (entity as ISaveDataProvider)?.ProvideSaveData(saveData);
                    entity.Inject(_builderStartup.Resolver);
                    entity.Initialize();
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
                    testSavableEntity.SaveData.Serialize();
                }
            }
        }
    }
}