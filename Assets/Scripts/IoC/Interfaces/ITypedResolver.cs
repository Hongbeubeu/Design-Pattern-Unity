using System;

namespace IoC
{
    public interface ITypedResolver<in T> : IInjectable
    {
        bool CanResolve(Type type);
        bool CanResolve(Type type, object id);
        bool CanResolve<TContract>() where TContract : T;
        bool CanResolve<TContract>(object id) where TContract : T;
        TContract Resolve<TContract>() where TContract : T;
        TContract Resolve<TContract>(object id) where TContract : T;
        TContract[] ResolveAll<TContract>() where TContract : T;
        TContract[] ResolveAll<TContract>(object[] ids) where TContract : T;
        TContract[] ResolveAll<TContract>(Predicate<TContract> predicate) where TContract : T;
    }
}