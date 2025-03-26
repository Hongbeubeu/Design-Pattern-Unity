using System;

namespace hcore.IoC.Services
{
    public interface IAction
    {
        Type[] TypesToDispatchAs { get; }
        void ResetAction();
    }
}