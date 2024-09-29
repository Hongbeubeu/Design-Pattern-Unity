namespace IoC
{
    public interface IInstaller
    {
        void Install(IContainer container);
        void Uninstall(IContainer container);
    }
}