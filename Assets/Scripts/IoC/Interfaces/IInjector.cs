namespace IoC
{
    public interface IInjector
    {
        bool TryInject(object @object, IBinding binding, IResolver resolver);
    }
}