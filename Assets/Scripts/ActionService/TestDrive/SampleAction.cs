using System;

namespace ActionService.TestDrive
{
    public class SampleAction : IAction
    {
        public Type[] TypesToDispatchAs => new[] { typeof(SampleAction) };

        public void ResetAction()
        {
        }
    }
}