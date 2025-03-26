using System;

namespace hcore.IoC.Services
{
    public abstract class BaseService : IService
    {
        public bool IsInitialized { get; protected set; }
        public event Action<IService> OnInitialized;

        protected IResolver resolver;

        public virtual void Inject(IResolver initResolver)
        {
            resolver = initResolver;
        }

        public virtual void Initialize()
        {
            InitializeService();

            IsInitialized = true;
            OnInitialized?.Invoke(this);
        }

        public virtual void Cleanup()
        {
            CleanupService();

            IsInitialized = false;
            resolver = null;
        }

        protected abstract void InitializeService();
        protected abstract void CleanupService();
    }
}