using System.IO;
using IoC;
using MessagePack;
using UnityEngine;

namespace Builder.Entity
{
    [MessagePackObject]
    public abstract class BaseSaveData : ISaveData
    {
        [Key(0)]
        public string Id { get; }

        protected string SavePath { get; }

        protected IResolver Resolver { get; private set; }
        protected SerializerService SerializerService { get; private set; }

        protected BaseSaveData()
        {
        }

        protected BaseSaveData(string id)
        {
            Id = id;
            SavePath = Path.Combine(Application.persistentDataPath, $"{Id}.dat");
        }

        public virtual void Inject(IResolver initResolver)
        {
            Resolver = initResolver;
            SerializerService = Resolver.Resolve<SerializerService>();
        }

        public abstract void Serialize();

        public abstract void Deserialize();

        protected bool CanDeserialize()
        {
            return File.Exists(SavePath);
        }
    }
}