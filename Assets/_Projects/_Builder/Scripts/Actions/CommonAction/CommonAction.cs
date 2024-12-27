using System;
using IoC.Services;

namespace Builder.Actions
{
    public class CommonAction : IAction
    {
        public Type[] TypesToDispatchAs => new[] { typeof(CommonAction) };

        public void ResetAction()
        {
        }
    }

    public class GameUpdateAction : IAction
    {
        public float DeltaTime { get; private set; }
        public Type[] TypesToDispatchAs => new[] { typeof(GameUpdateAction) };

        public GameUpdateAction Init(float deltaTime)
        {
            DeltaTime = deltaTime;
            return this;
        }

        public void ResetAction()
        {
            DeltaTime = 0;
        }
    }
}