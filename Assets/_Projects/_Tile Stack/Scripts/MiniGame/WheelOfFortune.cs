using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TileStack.MiniGame
{
    public class WheelOfFortune : MonoBehaviour
    {
        [SerializeField] private int _numberOfItems = 8;
        [SerializeField] private float _spinDuration = 5f;
        [SerializeField] private int _fakeSpins = 2;
        [SerializeField] private AnimationCurve _spinCurve;
        [Header("Wheel Item")] public TextMeshProUGUI textPrefab;
        [SerializeField] private float _textRadius = 200f;
        [SerializeField] private float _textAngleOffset = -22.5f;
        private bool _isSpinning;
        private float _currentAngle;
        private float _targetAngle;
        private float _currentSpinTime;
        private int _winningSegment;
        private const float FULL_CIRCLE_ANGLE = 360f;

        private void Start()
        {
            CreateWheelTexts();
        }

        private void CreateWheelTexts()
        {
            var angleStep = FULL_CIRCLE_ANGLE / _numberOfItems;

            for (var i = 0; i < _numberOfItems; i++)
            {
                var angle = i * angleStep + _textAngleOffset;
                var position = new Vector3(
                                   Mathf.Sin(angle * Mathf.Deg2Rad),
                                   Mathf.Cos(angle * Mathf.Deg2Rad),
                                   0) * _textRadius;

                var textObj = Instantiate(textPrefab, transform);
                textObj.transform.localPosition = position;
                textObj.transform.localRotation = Quaternion.Euler(0, 0, -angle);
                textObj.text = (i + 1).ToString();
            }
        }

        public void Spin()
        {
            if (_isSpinning) return;
            _winningSegment = Random.Range(0, _numberOfItems);
            var segmentSize = FULL_CIRCLE_ANGLE / _numberOfItems;

            var angleToZero = (FULL_CIRCLE_ANGLE - _currentAngle % FULL_CIRCLE_ANGLE) % FULL_CIRCLE_ANGLE;

            _targetAngle = angleToZero + _fakeSpins * FULL_CIRCLE_ANGLE + segmentSize * _winningSegment;

            _currentSpinTime = 0f;
            _isSpinning = true;
        }

        private void Update()
        {
            if (!_isSpinning) return;
            _currentSpinTime += Time.deltaTime;

            if (_currentSpinTime < _spinDuration)
            {
                DoRotate();
            }
            else
            {
                OnSpinFinished();
            }
        }

        private void DoRotate()
        {
            var curveValue = _spinCurve.Evaluate(_currentSpinTime / _spinDuration);
            var angle = Mathf.Lerp(0, _targetAngle, curveValue);
            transform.eulerAngles = new Vector3(0, 0, _currentAngle + angle);
        }

        private void OnSpinFinished()
        {
            _currentAngle = (_currentAngle + _targetAngle) % FULL_CIRCLE_ANGLE;
            transform.eulerAngles = new Vector3(0, 0, _currentAngle);
            _isSpinning = false;
            Debug.Log($"Winning number: {_winningSegment + 1}");
        }
    }
}