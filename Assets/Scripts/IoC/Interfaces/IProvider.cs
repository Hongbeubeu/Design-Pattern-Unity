using System;

namespace hcore.IoC
{
    public interface IProvider
    {
        Type ContractType { get; }
        bool WasCached { get; }
        object GetInstance();
    }
}