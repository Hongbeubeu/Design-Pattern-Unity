using System;

namespace hcore.IoC.Services
{
    public class SampleAction : IAction
    {
        public Type[] TypesToDispatchAs => new[] { typeof(SampleAction) };

        public void ResetAction()
        {
        }
    }
}