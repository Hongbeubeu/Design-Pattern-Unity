public interface IEntity : IEntityLifeCycle, IConfigProvider
{
    string Id { get; }
}