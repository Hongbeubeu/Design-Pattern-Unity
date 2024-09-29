using System;
using System.Collections.Generic;
using System.Linq;
using Logger;

namespace IoC
{
    public delegate void BindResolver(IBinding binding);

    public class Container : IContainer
    {
        private readonly IInjector _injector = new Injector();
        private readonly Dictionary<BindIndex, IBinding> _bindings = new();

        public Container()
        {
            Bind<IResolver>().ToInstance(this).AsSingleton().Conclude();
            Bind<IBinder>().ToInstance(this).AsSingleton().Conclude();
        }

        public IBinding Bind(Type type)
        {
            IBinding binding = new Binding(type, BindResolver);
            return binding;
        }

        public IBinding Bind<TContract>()
        {
            return Bind(typeof(TContract));
        }

        public bool Unbind(Type type)
        {
            return Unbind(type, null);
        }

        public bool Unbind<TContract>()
        {
            return Unbind(typeof(TContract), null);
        }

        public bool Unbind(Type type, object id)
        {
            return _bindings.Remove(new BindIndex(type, id));
        }

        public bool Unbind<TContract>(object id)
        {
            return Unbind(typeof(TContract), id);
        }

        public bool CanResolve(Type type)
        {
            return CanResolve(type, null);
        }

        public bool CanResolve(Type type, object id)
        {
            var bindIndex = new BindIndex(type, id);
            return _bindings.ContainsKey(bindIndex);
        }

        public bool CanResolve<TContract>()
        {
            return CanResolve<TContract>(null);
        }

        public bool CanResolve<TContract>(object id)
        {
            return CanResolve(typeof(TContract), id);
        }

        public bool TryResolve(Type type, out object value)
        {
            return TryResolve(type, null, out value);
        }

        public bool TryResolve(Type type, object id, out object value)
        {
            var bindIndex = new BindIndex(type, id);
            if (_bindings.TryGetValue(bindIndex, out var binding))
            {
                value = binding.Provider.GetInstance();
                _injector.TryInject(value, binding, this);
                return true;
            }

            value = null;
            return false;
        }

        public bool TryResolve<TContract>(out TContract value)
        {
            return TryResolve(null, out value);
        }

        public bool TryResolve<TContract>(object id, out TContract value)
        {
            var result = TryResolve(typeof(TContract), id, out var valueObject);
            value = (TContract)valueObject;
            return result;
        }

        public object Resolve(Type type)
        {
            return Resolve(type, null);
        }

        public object Resolve(Type type, object id)
        {
            if (TryResolve(type, id, out var value))
            {
                return value;
            }

            Logs.HandleException(
                new BindingException(
                    type,
                    id,
                    $"Object of type <b>{type.Name}</b> with id <b>{id}</b> was not found in the Container. A null value was returned."
                )
            );
            return value;
        }

        public TContract Resolve<TContract>()
        {
            return Resolve<TContract>(null);
        }

        public TContract Resolve<TContract>(object id)
        {
            return (TContract)Resolve(typeof(TContract), id);
        }

        public object[] ResolveAll(Type type)
        {
            return ResolveAll(type, _ => true);
        }

        private object[] ResolveAll(Type type, Predicate<BindIndex> predicate)
        {
            var results = new List<object>();
            foreach (var key in _bindings.Keys.Where(key => key.Type == type && predicate(key)))
            {
                try
                {
                    results.Add(Resolve(type, key.Id));
                }
                catch
                {
                    Logs.LogError<Container>($"Couldn't resolve an {nameof(type)} with id {key.Id}.");
                }
            }

            return results.ToArray();
        }

        public object[] ResolveAll(Type type, IEnumerable<object> ids)
        {
            var idArray = ids.ToArray();
            var len = idArray.Length;
            var results = new object[len];
            for (var i = 0; i < len; i++)
            {
                results[i] = Resolve(type, idArray[i]);
            }

            return results;
        }

        public TContract[] ResolveAll<TContract>()
        {
            return ResolveAll<TContract>((BindIndex _) => true);
        }

        private TContract[] ResolveAll<TContract>(Predicate<BindIndex> predicate)
        {
            var contractType = typeof(TContract);
            var results = new List<TContract>();
            foreach (var key in _bindings.Keys.Where(key => key.Type == contractType && predicate(key)))
            {
                try
                {
                    results.Add(Resolve<TContract>(key.Id));
                }
                catch
                {
                    Logs.LogError<Container>($"Couldn't resolve an {nameof(TContract)} with id {key.Id}.");
                }
            }

            return results.ToArray();
        }

        public TContract[] ResolveAll<TContract>(IEnumerable<object> ids)
        {
            var idArray = ids.ToArray();
            var len = idArray.Length;

            var results = new List<TContract>();
            for (var i = 0; i < len; i++)
            {
                try
                {
                    results.Add(Resolve<TContract>(idArray[i]));
                }
                catch
                {
                    if (len > 1)
                    {
                        Logs.LogError<Container>($"Couldn't resolve an {nameof(TContract)} with id {idArray[i]}.");
                    }
                    else
                    {
                        Logs.HandleException(
                            new BindingException($"Couldn't resolve an {nameof(TContract)} with id {idArray[i]}."));
                    }
                }
            }

            return results.ToArray();
        }

        public TContract[] ResolveAll<TContract>(Predicate<TContract> predicate)
        {
            return ResolveAll<TContract>(bindIndex => predicate(Resolve<TContract>(bindIndex.Id)));
        }

        private void BindResolver(IBinding binding)
        {
            // Every binding must have a provider by the time it is being resolved
            if (binding.Provider == null)
            {
                Logs.HandleException(new BindingException(binding.ContractType, $"Attempt to bind {binding} with name {nameof(IBinding)} with a null provider. Check that the <b>{nameof(IBinding)}</b> is a concrete class first."));
            }

            var bindIndex = new BindIndex(binding);
            // Remove all previous bindings of the same type if the new binding is a singleton
            if (binding.Lifecycle == Lifecycle.Singleton)
            {
                var bindIndices = new List<BindIndex>(_bindings.Keys);
                var len = bindIndices.Count;
                for (var i = 0; i < len; i++)
                {
                    var index = bindIndices[i];
                    if (index.Type == binding.ContractType)
                    {
                        if (_bindings.TryGetValue(index, out var currentBinding) && currentBinding.Lifecycle == Lifecycle.Singleton)
                        {
                            Logs.HandleException(
                                new BindingException(
                                    binding.ContractType,
                                    binding.Id,
                                    $"This object is already in the container as singleton. <b>{nameof(IBinding)}</b>"
                                )
                            );
                        }

                        Logs.LogInfo<Container>($"Removing this object from the container. {binding} is <b>{Lifecycle.Singleton}</b> - the <b>{nameof(IBinding)}s</b> of type <b>{_bindings[index].ContractType}</b> with id <b>{_bindings[index].Id}</b>");
                        _bindings.Remove(index);
                    }
                }
            }

            // Check if a singleton binding already exists, if so, throw an exception
            if (_bindings.TryGetValue(new BindIndex(binding), out var typeBinding) && typeBinding.Lifecycle == Lifecycle.Singleton)
            {
                Logs.HandleException(
                    new BindingException(
                        binding.ContractType,
                        binding.Id,
                        $"This object is already in the container as a Singleton. <b>{nameof(IBinding)}</b> of type <b> {binding.ContractType}</b> in the container <b>{GetType().Name}</b>"
                    )
                );
            }

            // Add to bindings lookup, only binding with new bind index will be added
            if (!_bindings.TryGetValue(bindIndex, out _))
            {
                _bindings.Add(bindIndex, binding);
            }
            else
            {
                Logs.HandleException(
                    new BindingException(
                        binding.ContractType,
                        binding.Id,
                        $"This object is already in the container as a Singleton. <b>{nameof(IBinding)}</b> of type <b>{binding.ContractType}</b> with id <b>{binding.Id}</b> in the container <b>{GetType().Name}</b>. " +
                        $"Please call Unbind with the <b>{binding.ContractType}</b> and <b>{binding.Id}</b> if you wish to replace the binding."
                    )
                );
            }
        }
    }
}