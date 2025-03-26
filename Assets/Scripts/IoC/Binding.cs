using System;
using hcore.IoC.Providers;
using hcore.Logger;

namespace hcore.IoC
{
    public class Binding : IBinding
    {
        private readonly BindResolver _bindResolver;
        public Type ContractType { get; }
        public object Id { get; private set; }
        public Lifecycle Lifecycle { get; private set; } = Lifecycle.Transient;
        public IProvider Provider { get; private set; }

        public Binding(Type contractType, BindResolver bindResolver)
        {
            ContractType = contractType;
            _bindResolver = bindResolver;

            if (!contractType.IsInterface && !contractType.IsAbstract)
            {
                Provider = new TransientProvider(contractType, contractType);
            }
        }

        public IBinding To<TConcrete>() where TConcrete : class
        {
            CheckAssignable(typeof(TConcrete));

            Provider = new TransientProvider(ContractType, typeof(TConcrete));
            return this;
        }

        public IBinding FromMethod<TConcrete>(Func<TConcrete> method) where TConcrete : class
        {
            CheckAssignable(typeof(TConcrete));

            Provider = new MethodProvider(ContractType, method);
            return this;
        }

        public IBinding ToInstance(object instance)
        {
            CheckAssignable(instance);

            Provider = new InstanceProvider(ContractType, instance);
            return this;
        }

        public IBinding FromProvider(IProvider newProvider)
        {
            CheckAssignable(newProvider);

            Provider = newProvider;
            return this;
        }

        public IBinding ToId(object id)
        {
            if (Id != null)
            {
                Logs.HandleException(new BindingException($"Calling {nameof(ToId)} to override the current non-null {nameof(Id)}: <b>{Id}</b> with a new id: <b>{id}</b>"));
            }

            if (id == null)
            {
                Logs.HandleException(new BindingException($"Calling {nameof(ToId)} with a null id!"));
            }

            Id = id;
            return this;
        }

        public IBinding AsTransient()
        {
            Lifecycle = Lifecycle.Transient;
            return this;
        }

        public IBinding AsCached()
        {
            Lifecycle = Lifecycle.Cached;
            Provider = new CachedProvider(Provider);
            return this;
        }

        public IBinding AsSingleton()
        {
            if (Id != null)
            {
                Logs.LogWarning<Binding>($"WARNING: Calling {nameof(AsSingleton)}, <b>{nameof(Id)}</b> was assigned, which is not applicable for {nameof(AsSingleton)} binding. <b>{nameof(Id)}</b> has been set to null.");
                Id = null;
            }

            Lifecycle = Lifecycle.Singleton;
            Provider = new CachedProvider(Provider);
            return this;
        }

        public IBinding Conclude()
        {
            _bindResolver(this);
            return this;
        }

        private bool CheckAssignable(Type type)
        {
            if (!ContractType.IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
            {
                Logs.HandleException(new BindingException($"Is not assignalbe. Contract type <b>{ContractType.Name}</b> cannot to be assigned from {typeof(Type)} <b>{type.Name}</b>"));
            }

            return true;
        }

        private bool CheckAssignable<TConcrete>(Func<TConcrete> method) where TConcrete : class
        {
            if (!ContractType.IsAssignableFrom(typeof(TConcrete)))
            {
                Logs.HandleException(new BindingException(
                    $"Is not assignable. Contract type <b>{ContractType.Name}</b> cannot be assigned from {typeof(Func<TConcrete>)} <b>{method}</b>"));
            }

            return true;
        }

        private bool CheckAssignable(object instance)
        {
            if (!ContractType.IsInstanceOfType(instance))
            {
                Logs.HandleException(new BindingException(
                    $"Is not assignable. Contract type <b>{ContractType.Name}</b> cannot be assigned from {typeof(object)} <b>{instance}</b>"));
            }

            return true;
        }

        private bool CheckAssignable(IProvider newProvider)
        {
            if (newProvider == null || !ContractType.IsAssignableFrom(newProvider.ContractType))
            {
                Logs.HandleException(new BindingException(
                    $"Is not assignable. Contract type <b>{ContractType.Name}</b> cannot be assigned from {typeof(IBinding)} <b>{newProvider}</b>"));
            }

            return true;
        }

        public override string ToString()
        {
            return $"<b>{GetType().Name}</b> with contract type <b>{ContractType.Name}</b>, provider {Provider}, and id <b>{Id}</b>";
        }
    }
}