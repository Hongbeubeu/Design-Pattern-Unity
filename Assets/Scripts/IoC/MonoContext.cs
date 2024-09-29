using UnityEngine;

namespace IoC
{
    public class MonoContext : MonoBehaviour, IContext
    {
        [SerializeField] private MonoInstaller installer;
        private IContainer _container;
        public IContainer Container => _container ??= new Container();
        public IInstaller Installer => installer;

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