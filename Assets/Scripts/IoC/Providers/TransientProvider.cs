using System;

namespace hcore.IoC.Providers
{
    public class TransientProvider : IProvider
    {
        private readonly Type _contractType;
        private readonly Type _concreteType;
        public Type ContractType => _contractType;
        public bool WasCached => false;

        public TransientProvider(Type contractType, Type concreteType)
        {
            _contractType = contractType;
            _concreteType = concreteType;
        }

        public object GetInstance()
        {
            return Activator.CreateInstance(_concreteType);
        }

        public override string ToString()
        {
            return $"<b>{GetType()}</b> with {nameof(ContractType)} <b>{ContractType}</b> and {nameof(_contractType)} <b> {_concreteType}</b>";
        }
    }
}