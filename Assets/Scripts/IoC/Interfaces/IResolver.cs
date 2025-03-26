using System;
using System.Collections.Generic;

namespace hcore.IoC
{
    public interface IResolver
    {
        bool CanResolve(Type type);
        bool CanResolve(Type type, object id);
        bool CanResolve<TContract>();
        bool CanResolve<TContract>(object id);
        bool TryResolve(Type type, out object value);
        bool TryResolve(Type type, object id, out object value);
        bool TryResolve<TContract>(out TContract value);
        bool TryResolve<TContract>(object id, out TContract value);
        object Resolve(Type type);
        object Resolve(Type type, object id);
        TContract Resolve<TContract>();
        TContract Resolve<TContract>(object id);
        object[] ResolveAll(Type type);
        object[] ResolveAll(Type type, IEnumerable<object> ids);
        TContract[] ResolveAll<TContract>();
        TContract[] ResolveAll<TContract>(IEnumerable<object> ids);
        TContract[] ResolveAll<TContract>(Predicate<TContract> predicate);
    }
}