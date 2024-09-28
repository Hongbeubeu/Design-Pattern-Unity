using System;
using UnityEngine;

namespace ActionService.TestDrive
{
    public class ActionServiceTestDrive : MonoBehaviour
    {
        private IActionService _actionService;

        private void Start()
        {
            _actionService = new ActionService();
            _actionService.RegisterAction(new SampleAction());
            _actionService.Subscribe<SampleAction>(OnSampleAction, this);
        }

        private void OnDestroy()
        {
            _actionService?.Unsubscribe<SampleAction>(OnSampleAction, this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var action = _actionService.Get<SampleAction>();
                _actionService.Dispatch(action, this);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _actionService.Unsubscribe<SampleAction>(OnSampleAction, this);
            }
        }

        private void OnSampleAction(SampleAction obj)
        {
            Debug.Log("SampleAction received");
        }
    }
}