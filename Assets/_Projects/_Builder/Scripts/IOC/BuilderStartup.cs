using System;
using Builder.Actions;
using hcore.IoC;
using hcore.IoC.Services;
using UnityEngine;

namespace Builder.Entity
{
    public class BuilderStartup : MonoStartup
    {
        public Action onInstalled;

        private IActionService _actionService;

        public override void Start()
        {
            base.Start();
            onInstalled?.Invoke();
            _actionService = Resolver.Resolve<IActionService>();
        }

        private void Update()
        {
            if (_actionService == null)
            {
                return;
            }

            var action = _actionService.Get<GameUpdateAction>();
            action.Init(Time.deltaTime);
            _actionService.Dispatch(action);
        }
    }
}