namespace Builder.Entity
{
    public interface ITestEntity : IBaseEntity<ITestEntityConfigData>
    {
        void DoSomething();
    }
}