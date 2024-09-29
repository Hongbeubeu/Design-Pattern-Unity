namespace IoC
{
    public interface IInjectable
    {
        void Inject(IResolver resolver);
    }
}