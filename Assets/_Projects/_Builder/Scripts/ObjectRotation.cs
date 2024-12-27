using UnityEngine;
using Lean.Touch;

namespace Builder
{
    public class ObjectRotation : MonoBehaviour
    {
        [SerializeField]
        private float _maxRotationSpeed = 10f; // Maximum rotation speed

        [SerializeField]
        private float _rotationSpeed = 10f; // Rotation speed

        [SerializeField]
        private float _scrollSpeed = 2f; // Scroll speed

        [SerializeField]
        private float _dampingSpeed = 5f; // Speed of damping

        private float _rotationVelocityY; // Rotation velocity on Y axis

        private void Update()
        {
            // Handle drag from mouse or touch
            HandleDragRotation();

            // Handle scroll from mouse
            HandleScrollRotation();

            // Apply smooth rotation
            ApplySmoothRotation();
        }

        private void HandleDragRotation()
        {
            // Check if there is any finger/touch on the screen
            if (LeanTouch.Fingers.Count <= 0 || !Input.GetMouseButton(0)) return;
            var finger = LeanTouch.Fingers[0];
            var delta = finger.ScreenDelta;

            // Update rotation velocity based on mouse/touch movement
            _rotationVelocityY = -Mathf.Clamp(delta.x * _rotationSpeed, -_maxRotationSpeed, _maxRotationSpeed);
        }

        private void HandleScrollRotation()
        {
            // Get mouse scroll wheel value
            var scrollDelta = Input.mouseScrollDelta.y;

            // Only rotate if there is scroll
            if (!(Mathf.Abs(scrollDelta) > 0.01f)) return;
            _rotationVelocityY = Mathf.Clamp(scrollDelta * _scrollSpeed, -_maxRotationSpeed, _maxRotationSpeed);
        }

        private void ApplySmoothRotation()
        {
            // Apply rotation on Y axis
            transform.Rotate(Vector3.up, _rotationVelocityY * Time.deltaTime, Space.World);

            // Damp rotation velocity
            _rotationVelocityY = Mathf.Lerp(_rotationVelocityY, 0, _dampingSpeed * Time.deltaTime);
        }
    }
}