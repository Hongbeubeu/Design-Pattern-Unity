namespace Builder.Entity
{
    public interface IEntityLifeCycle
    {
        void Initialize();
        void Cleanup();
    }
}