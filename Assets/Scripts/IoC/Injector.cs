namespace hcore.IoC
{
    internal class Injector : IInjector
    {
        public bool TryInject(object @object, IBinding binding, IResolver resolver)
        {
            if (@object is not IInjectable injectable || binding.Provider.WasCached) return false;
            injectable.Inject(resolver);
            return true;
        }
    }
}