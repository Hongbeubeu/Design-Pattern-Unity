namespace hcore.IoC.Services
{
    public interface IStateAction : IAction
    {
        void Apply();
    }
}