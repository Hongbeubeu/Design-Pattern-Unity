namespace ActionService
{
    public interface IStateAction : IAction
    {
        void Apply();
    }
}