namespace Builder.Entity
{
    public class TestSavableEntity : BaseSavableEntity<ITestSavableEntityConfigData, ITestSaveData>, ITestSavableEntity
    {
        public TestSavableEntity(string id) : base(id)
        {
        }
    }
}