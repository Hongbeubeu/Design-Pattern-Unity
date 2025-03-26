using System;

namespace hcore.IoC.Providers
{
    public class CachedProvider : IProvider
    {
        private readonly IProvider _provider;
        private object _object;
        private bool _wasCached;
        public Type ContractType => _provider.ContractType;
        public bool WasCached => _wasCached;

        public CachedProvider(IProvider provider)
        {
            _provider = provider ?? throw new BindingException($"Attempting to create <b>{GetType()}</b> with a null <b> {typeof(IProvider)}</b>.");
        }

        public object GetInstance()
        {
            _wasCached = _object != null;
            if (!_wasCached)
            {
                _object = _provider.GetInstance();
            }

            return _object;
        }

        public override string ToString()
        {
            return $"<b>{GetType()}</b> with {nameof(ContractType)} <b>{ContractType}</b>, {nameof(_object)} <b>{_object}</b> and {nameof(_provider)} <b>{_provider}</b>";
        }
    }
}