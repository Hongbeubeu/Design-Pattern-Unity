namespace IoC
{
    public abstract class Context : IContext
    {
        public IContainer Container { get; }
        public IInstaller Installer { get; }

        public void Bind()
        {
            Installer.Install(Container);
        }

        public void Unbind()
        {
            Installer.Uninstall(Container);
        }
    }
}