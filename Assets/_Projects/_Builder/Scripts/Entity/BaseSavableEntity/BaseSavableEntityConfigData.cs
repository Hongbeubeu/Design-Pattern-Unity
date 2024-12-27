using UnityEngine;

namespace Builder.Entity
{
    public abstract class BaseSavableEntityConfigData : ScriptableObject, IBaseSavableEntityConfigData
    {
        [SerializeField]
        protected string _id;

        public string Id => _id;

        public abstract IEntity CreateEntity();
        public abstract ISaveData CreateSaveData();
    }
}