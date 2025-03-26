namespace hcore.IoC
{
    public interface IContext
    {
        IContainer Container { get; }
        IInstaller Installer { get; }
        void Bind();
        void Unbind();
    }
}