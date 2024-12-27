using System;
using DG.Tweening;
using UnityEngine;

namespace Builder.UI
{
    [Serializable]
    public struct UIAnimationStrategyConfig
    {
        [SerializeField]
        private AnimationType _animationType;

        [SerializeField]
        private float _duration;

        [SerializeField]
        private Ease _ease;

        [SerializeField]
        private AnimationStrategyData _animateTo;

        public AnimationType AnimationType => _animationType;
        public float Duration => _duration;
        public Ease Ease => _ease;
        public AnimationStrategyData AnimateTo => _animateTo;

        // Constructor
        public UIAnimationStrategyConfig(AnimationType animationType, float duration, AnimationStrategyData animateTo, Ease ease)
        {
            _animationType = animationType;
            _duration = duration;
            _animateTo = animateTo;
            _ease = ease;
        }
    }

    public enum AnimationType
    {
        None = 1 << 0,
        Scale = 1 << 1,
        Move = 1 << 2,
        Fade = 1 << 3,
        Rotate = 1 << 4,
    }

    [Serializable]
    public struct AnimationStrategyData
    {
        [SerializeField]
        private Vector3 _value;

        [SerializeField]
        private float _alpha;

        public Vector3 Position => _value;
        public Vector3 Rotation => _value;
        public Vector3 Scale => _value;
        public float Alpha => _alpha;

        // Constructor
        public AnimationStrategyData(Vector3 value, float alpha)
        {
            _value = value;
            _alpha = alpha;
        }
    }
}