using System;

namespace IoC.Services
{
    public interface IAction
    {
        Type[] TypesToDispatchAs { get; }
        void ResetAction();
    }
}