using System;

namespace IoC
{
    public interface IProvider
    {
        Type ContractType { get; }
        bool WasCached { get; }
        object GetInstance();
    }
}