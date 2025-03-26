namespace hcore.IoC
{
    public interface IInjectable
    {
        void Inject(IResolver initResolver);
    }
}