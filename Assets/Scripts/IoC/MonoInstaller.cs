using IoC.Services;
using UnityEngine;

namespace IoC
{
    public class MonoInstaller : MonoBehaviour, IInstaller
    {
        public virtual void Install(IContainer container)
        {
            container.Bind<IActionService>().To<ActionService>().AsSingleton().Conclude();
            container.Bind<IActionResolver>().To<ActionResolver>().AsSingleton().Conclude();
            container.Bind<SampleAction>().AsSingleton().Conclude();
        }

        public virtual void Uninstall(IContainer container)
        {
            container.Unbind<SampleAction>();
            container.Unbind<IActionResolver>();
            container.Unbind<IActionService>();
        }
    }
}