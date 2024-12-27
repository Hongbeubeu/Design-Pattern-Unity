namespace Builder.Entity
{
    public interface IBaseEntity<out TEntityConfigData> : IEntity where TEntityConfigData : IEntityConfigData
    {
        TEntityConfigData ConfigData { get; }
    }
}