public interface ISavableEntity<out TConfigData, out TSaveData> : IBaseEntity<TConfigData>, IProvideSaveData
    where TSaveData : ISaveData
    where TConfigData : IEntityConfigData
{
    TSaveData SaveData { get; }
}