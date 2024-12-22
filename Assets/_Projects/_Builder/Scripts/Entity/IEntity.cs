public interface IEntity : IEntityLifeCycle, IProvideConfig
{
    string Id { get; }
}