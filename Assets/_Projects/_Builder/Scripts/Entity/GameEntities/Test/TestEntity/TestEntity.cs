using Builder.Actions;
using UnityEngine;

namespace Builder.Entity
{
    public class TestEntity : BaseEntity<ITestEntityConfigData>, ITestEntity
    {
        public TestEntity(string id) : base(id)
        {
        }

        public override void Initialize()
        {
            ActionService.Subscribe<GameUpdateAction>(HandleGameUpdate);
        }

        public override void Cleanup()
        {
            ActionService.Unsubscribe<GameUpdateAction>(HandleGameUpdate);
            base.Cleanup();
        }

        private void HandleGameUpdate(GameUpdateAction obj)
        {
            // Logs.LogInfo<TestEntity>($"DeltaTime: {obj.DeltaTime}");
        }

        public void DoSomething()
        {
            Debug.Log(ConfigData.Number);
        }
    }
}