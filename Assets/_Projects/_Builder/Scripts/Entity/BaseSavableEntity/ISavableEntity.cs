public interface ISavableEntity<out TConfigData, out TSaveData> : IBaseEntity<TConfigData>, ISaveDataProvider
    where TSaveData : ISaveData
    where TConfigData : IEntityConfigData
{
    TSaveData SaveData { get; }
}