using Builder.Actions;
using IoC;

namespace Builder.Entity
{
    public class BuilderInstaller : MonoInstaller
    {
        public override void Install(IContainer container)
        {
            base.Install(container);
            container.Bind<SerializerService>().AsSingleton().Conclude();
            container.Bind<GameUpdateAction>().AsSingleton().Conclude();
        }

        public override void Uninstall(IContainer container)
        {
            container.Unbind<GameUpdateAction>();
            container.Unbind<SerializerService>();
            base.Uninstall(container);
        }
    }
}