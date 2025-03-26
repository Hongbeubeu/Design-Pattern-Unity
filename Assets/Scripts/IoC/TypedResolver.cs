using System;
using hcore.IoC.Services;

namespace hcore.IoC
{
    public class ActionResolver : TypedResolver<IAction>, IActionResolver
    {
        // Empty
    }

    public abstract class TypedResolver<T> : ITypedResolver<T>
    {
        protected IResolver resolver;

        public void Inject(IResolver initResolver)
        {
            resolver = initResolver;
        }

        public bool CanResolve(Type type)
        {
            return resolver.CanResolve(type);
        }

        public bool CanResolve(Type type, object id)
        {
            return resolver.CanResolve(type, id);
        }

        public bool CanResolve<TContract>() where TContract : T
        {
            return resolver.CanResolve<TContract>();
        }

        public bool CanResolve<TContract>(object id) where TContract : T
        {
            return resolver.CanResolve<TContract>(id);
        }

        public TContract Resolve<TContract>() where TContract : T
        {
            return resolver.Resolve<TContract>();
        }

        public TContract Resolve<TContract>(object id) where TContract : T
        {
            return resolver.Resolve<TContract>(id);
        }

        public TContract[] ResolveAll<TContract>() where TContract : T
        {
            return resolver.ResolveAll<TContract>();
        }

        public TContract[] ResolveAll<TContract>(object[] ids) where TContract : T
        {
            return resolver.ResolveAll<TContract>(ids);
        }

        public TContract[] ResolveAll<TContract>(Predicate<TContract> predicate) where TContract : T
        {
            return resolver.ResolveAll(predicate);
        }
    }
}