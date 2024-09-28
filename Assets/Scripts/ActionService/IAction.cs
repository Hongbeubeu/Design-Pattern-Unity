using System;

namespace ActionService
{
    public interface IAction
    {
        Type[] TypesToDispatchAs { get; }
        void ResetAction();
    }
}