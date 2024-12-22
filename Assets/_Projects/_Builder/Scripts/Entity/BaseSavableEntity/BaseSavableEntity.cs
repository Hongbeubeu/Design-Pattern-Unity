public abstract class BaseSavableEntity<TConfigData, TSaveData> : ISavableEntity<TConfigData, TSaveData>
    where TSaveData : ISaveData
    where TConfigData : IEntityConfigData
{
    public string Id { get; }
    public TConfigData ConfigData { get; protected set; }
    public TSaveData SaveData { get; protected set; }

    protected BaseSavableEntity(string id)
    {
        Id = id;
    }

    public void Setup()
    {
    }

    public void Cleanup()
    {
    }

    public virtual void ProvideConfigData(IEntityConfigData configData)
    {
        ConfigData = (TConfigData)configData;
    }

    public virtual void ProvideSaveData(ISaveData saveData)
    {
        SaveData = (TSaveData)saveData;
    }
}