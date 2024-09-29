using System;

namespace IoC.Providers
{
    public class MethodProvider : IProvider
    {
        private readonly Type _contractType;
        private readonly Func<object> _funcProvider;
        public Type ContractType => _contractType;
        public bool WasCached => false;

        public MethodProvider(Type contractType, Func<object> funcProvider)
        {
            _contractType = contractType;
            _funcProvider = funcProvider;
        }

        public object GetInstance()
        {
            return _funcProvider.Invoke();
        }

        public override string ToString()
        {
            return $"<b>{GetType()}</b> with {nameof(ContractType)} <b>{ContractType}</b> and {nameof(_funcProvider)} <b>{_funcProvider}</b>";
        }
    }
}