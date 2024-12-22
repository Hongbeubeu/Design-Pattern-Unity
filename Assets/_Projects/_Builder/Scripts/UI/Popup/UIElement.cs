using System.Linq;
using UnityEngine;

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
    protected PopupAnimationStrategyConfig[] _openPopupConfigs;

    [SerializeField]
    protected PopupAnimationStrategyConfig[] _closePopupConfigs;


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

    private StrategyController _openStrategy;
    private StrategyController _closeStrategies;

    public virtual void Start()
    {
        _openStrategy = new StrategyController(_openPopupConfigs.Select(i => i.AnimationType).ToArray());
        _closeStrategies = new StrategyController(_closePopupConfigs.Select(i => i.AnimationType).ToArray());

        _popupCanvas.SetActive(false);
        _popupAnimationTarget.Group.interactable = false;
    }

    public virtual void Show()
    {
        _openStrategy.DoAnimations(
            _popupAnimationTarget,
            _openPopupConfigs,
            PrepareOpenPopup,
            () => _popupAnimationTarget.Group.interactable = true);
    }

    public virtual void Close()
    {
        _closeStrategies.DoAnimations(
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