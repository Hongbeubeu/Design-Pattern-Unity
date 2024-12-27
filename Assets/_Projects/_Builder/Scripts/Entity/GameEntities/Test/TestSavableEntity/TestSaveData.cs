using MessagePack;

namespace Builder.Entity
{
    public class TestSaveData : BaseSaveData, ITestSaveData
    {
        private int _testValue;

        [Key(1)]
        public int TestValue
        {
            get => _testValue;
            set => _testValue = value;
        }

        // For Serialization
        public TestSaveData()
        {
        }

        public TestSaveData(string id) : base(id)
        {
        }

        public override void Serialize()
        {
            SerializerService.SerializeSaveData(this);
        }

        public override void Deserialize()
        {
            if (SerializerService.DeserializeSaveData(Id, out TestSaveData deserialized))
            {
                _testValue = deserialized.TestValue;
            }
        }
    }
}