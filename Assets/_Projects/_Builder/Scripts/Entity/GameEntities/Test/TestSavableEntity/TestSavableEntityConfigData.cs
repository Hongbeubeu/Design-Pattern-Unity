using UnityEngine;

namespace Builder.Entity
{
    [CreateAssetMenu(menuName = "Entity/Test Savable", fileName = "TestSavableEntityConfigData", order = 0)]
    public class TestSavableEntityConfigData : BaseSavableEntityConfigData, ITestSavableEntityConfigData
    {
        public override IEntity CreateEntity()
        {
            return new TestSavableEntity(Id);
        }

        public override ISaveData CreateSaveData()
        {
            return new TestSaveData(Id);
        }
    }
}