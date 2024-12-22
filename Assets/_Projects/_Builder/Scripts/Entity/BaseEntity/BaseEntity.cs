public abstract class BaseEntity<TConfigData> : IBaseEntity<TConfigData> where TConfigData : class, IEntityConfigData
{
    public string Id { get; protected set; }
    public TConfigData ConfigData { get; protected set; }

    protected BaseEntity(string id)
    {
        Id = id;
    }

    public virtual void Setup()
    {
    }

    public virtual void Cleanup()
    {
        ConfigData = null;
    }

    public void ProvideConfigData(IEntityConfigData configData)
    {
        ConfigData = (TConfigData)configData;
    }
}