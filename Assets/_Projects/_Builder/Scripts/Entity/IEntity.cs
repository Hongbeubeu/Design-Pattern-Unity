using IoC;

namespace Builder.Entity
{
    public interface IEntity : IEntityLifeCycle, IConfigProvider, IInjectable
    {
        string Id { get; }
    }
}