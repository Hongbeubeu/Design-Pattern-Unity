using System;

namespace IoC.Services
{
    public class SampleAction : IAction
    {
        public Type[] TypesToDispatchAs => new[] { typeof(SampleAction) };

        public void ResetAction()
        {
        }
    }
}