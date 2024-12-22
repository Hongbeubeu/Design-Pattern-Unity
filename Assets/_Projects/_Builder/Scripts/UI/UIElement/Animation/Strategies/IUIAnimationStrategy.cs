using System;
using UnityEngine;


/// <summary>
/// UIAnimationTarget is a struct that holds a RectTransform and a CanvasGroup.
/// </summary>
[Serializable]
public struct UIAnimationTarget
{
    [SerializeField]
    private RectTransform _rectTransform;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    public RectTransform Rect => _rectTransform;
    public CanvasGroup Group => _canvasGroup;

    public UIAnimationTarget(RectTransform rectTransform, CanvasGroup canvasGroup)
    {
        _rectTransform = rectTransform;
        _canvasGroup = canvasGroup;
    }
}

public interface IUIAnimationStrategy
{
    void DoAnimation(UIAnimationTarget target, UIAnimationStrategyConfig config);
}