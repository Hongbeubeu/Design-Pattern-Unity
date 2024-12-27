namespace Builder.Entity
{
    public abstract class BaseSavableEntity<TConfigData, TSaveData> : BaseEntity<TConfigData>, ISavableEntity<TConfigData, TSaveData>
        where TSaveData : class, ISaveData
        where TConfigData : class, IEntityConfigData
    {
        public TSaveData SaveData { get; private set; }

        protected BaseSavableEntity(string id) : base(id)
        {
        }

        public void ProvideSaveData(ISaveData saveData)
        {
            SaveData = (TSaveData)saveData;
        }
    }
}