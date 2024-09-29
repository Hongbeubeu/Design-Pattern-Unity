using System;

namespace IoC.Providers
{
    public class InstanceProvider : IProvider
    {
        private readonly Type _contractType;
        private readonly object _object;
        public Type ContractType => _contractType;
        public bool WasCached => false;


        public InstanceProvider(Type contractType, object providerObject)
        {
            _contractType = contractType;
            _object = providerObject;
        }

        public object GetInstance()
        {
            return _object;
        }

        public override string ToString()
        {
            return $"<b>{GetType()}</b> with {nameof(ContractType)} <b>{ContractType}</b> and {nameof(_object)} <b>{_object}</b>";
        }
    }
}