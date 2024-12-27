using System;
using System.Linq;
using Builder.Editor;
using UnityEngine;

namespace Builder.UI
{
    [Serializable]
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class UIElement : MonoBehaviour, IUIElement
    {
        [SerializeField]
        protected GameObject _popupCanvas;

        [SerializeField]
        protected UIAnimationTarget _popupAnimationTarget;

        [Space]
        [Header("Animation Configs")]
        [SerializeField]
        protected UIAnimationStrategyConfig[] _openPopupConfigs;

        [SerializeField]
        protected UIAnimationStrategyConfig[] _closePopupConfigs;


        [Space]
        [Header("Initialize Values")]
        [SerializeField]
        private Vector3 _initialPosition;

        [SerializeField]
        private Vector3 _initialRotation;

        [SerializeField]
        private Vector3 _initialScale;

        [SerializeField]
        private float _initialAlpha;

        private UIAnimationController _openUIAnimation;
        private UIAnimationController _closeUIAnimations;

        public virtual void Start()
        {
            _openUIAnimation = new UIAnimationController(_openPopupConfigs.Select(i => i.AnimationType).ToArray());
            _closeUIAnimations = new UIAnimationController(_closePopupConfigs.Select(i => i.AnimationType).ToArray());

            _popupCanvas.SetActive(false);
            _popupAnimationTarget.Group.interactable = false;
        }

        public virtual void Show()
        {
            _openUIAnimation.DoAnimations(
                _popupAnimationTarget,
                _openPopupConfigs,
                PrepareOpenPopup,
                () => _popupAnimationTarget.Group.interactable = true);
        }

        public virtual void Close()
        {
            _closeUIAnimations.DoAnimations(
                _popupAnimationTarget,
                _closePopupConfigs,
                () => _popupAnimationTarget.Group.interactable = false,
                PostClosePopup);
        }

        public void PrepareOpenPopup()
        {
            ResetPopup();
            _popupCanvas.SetActive(true);
        }

        private void PostClosePopup()
        {
            _popupCanvas.SetActive(false);
            ResetPopup();
        }

        private void ResetPopup()
        {
            var rect = _popupAnimationTarget.Rect;
            rect.anchoredPosition = _initialPosition;
            rect.localEulerAngles = _initialRotation;
            rect.localScale = _initialScale;
            _popupAnimationTarget.Group.alpha = _initialAlpha;
            _popupAnimationTarget.Group.interactable = false;
        }

        [Button("Toggle Popup")]
        public virtual void Toggle()
        {
            if (_popupCanvas.activeSelf)
            {
                Close();
            }
            else
            {
                Show();
            }
        }
    }
}