using UnityEngine;

namespace Builder.Entity
{
    [CreateAssetMenu(menuName = "Entity/Test", fileName = "TestEntityConfigData", order = 0)]
    public class TestEntityConfigData : BaseEntityConfigData, ITestEntityConfigData
    {
        [SerializeField]
        private int _number;

        public int Number => _number;

        public override IEntity CreateEntity()
        {
            return new TestEntity(Id);
        }
    }
}