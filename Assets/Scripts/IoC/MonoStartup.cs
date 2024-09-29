using System;
using ActionService;
using ActionService.TestDrive;
using Logger;
using UnityEngine;

namespace IoC
{
    public class MonoStartup : MonoBehaviour, IStartup
    {
        [SerializeField] private MonoContext context;
        private IContext Context => context;

        public IResolver Resolver => Context.Container;

        private IActionService _actionService;

        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            if (!Input.anyKeyDown) return;
            var action = _actionService?.Get<SampleAction>();
            if (action == null) return;
            _actionService.Dispatch(action);
        }

        public void StartGame()
        {
            Context.Bind();
            _actionService = Resolver.Resolve<IActionService>();
            _actionService.Subscribe<SampleAction>(OnSample);
            _actionService.RegisterAction(new SampleAction());
        }

        public void ResetGame()
        {
            _actionService.Unsubscribe<SampleAction>(OnSample);
            Context.Unbind();
            StartGame();
        }

        private void OnSample(SampleAction obj)
        {
            Logs.Log<MonoStartup>($"On Sample Action: {obj}");
        }
    }
}