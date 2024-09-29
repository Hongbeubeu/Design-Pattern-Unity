using System;

namespace IoC
{
    public interface IBinding
    {
        Type ContractType { get; }
        object Id { get; }
        Lifecycle Lifecycle { get; }
        IProvider Provider { get; }
        IBinding To<TConcrete>() where TConcrete : class;
        IBinding FromMethod<TConcrete>(Func<TConcrete> method) where TConcrete : class;
        IBinding ToInstance(object instance);
        IBinding FromProvider(IProvider newProvider);
        IBinding ToId(object id);
        IBinding AsTransient();
        IBinding AsCached();
        IBinding AsSingleton();
        IBinding Conclude();
    }
}