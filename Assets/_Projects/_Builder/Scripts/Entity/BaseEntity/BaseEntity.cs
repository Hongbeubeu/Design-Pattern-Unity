using IoC;
using IoC.Services;

namespace Builder.Entity
{
    public abstract class BaseEntity<TConfigData> : IBaseEntity<TConfigData>
        where TConfigData : class, IEntityConfigData
    {
        public string Id { get; protected set; }
        protected IResolver Resolver { get; private set; }
        protected IActionService ActionService { get; private set; }

        public virtual void Inject(IResolver initResolver)
        {
            Resolver = initResolver;
            ActionService = Resolver.Resolve<IActionService>();
        }

        public virtual void Initialize()
        {
        }

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
}