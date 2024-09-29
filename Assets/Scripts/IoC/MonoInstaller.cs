using ActionService;
using UnityEngine;

namespace IoC
{
    public class MonoInstaller : MonoBehaviour, IInstaller
    {
        public virtual void Install(IContainer container)
        {
            container.Bind<IActionService>().To<ActionService.ActionService>().AsSingleton().Conclude();
        }

        public virtual void Uninstall(IContainer container)
        {
            container.Unbind<IActionService>();
        }
    }
}  