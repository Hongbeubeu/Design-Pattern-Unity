using UnityEngine;

namespace IoC.Services
{
    public class ActionServiceTestDrive : MonoBehaviour
    {
        private IActionService _actionService;
        [SerializeField] private MonoContext context;
        private IResolver Resolver => context.Container;

        private void OnDestroy()
        {
            _actionService?.Unsubscribe<SampleAction>(OnSampleAction, this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _actionService ??= Resolver.Resolve<IActionService>();
            }

            if (_actionService == null) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                var action = _actionService.Get<SampleAction>();
                _actionService.Dispatch(action, this);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _actionService.Unsubscribe<SampleAction>(OnSampleAction, this);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _actionService.Subscribe<SampleAction>(OnSampleAction, this);
            }
        }

        private void OnSampleAction(SampleAction obj)
        {
            Debug.Log("SampleAction received");
        }
    }
}