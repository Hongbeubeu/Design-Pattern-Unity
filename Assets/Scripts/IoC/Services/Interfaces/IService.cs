using System;

namespace IoC.Services
{
    public interface IService : IInjectable
    {
        event Action<IService> OnInitialized;

        bool IsInitialized { get; }
        void Initialize();
        void Cleanup();
    }
}