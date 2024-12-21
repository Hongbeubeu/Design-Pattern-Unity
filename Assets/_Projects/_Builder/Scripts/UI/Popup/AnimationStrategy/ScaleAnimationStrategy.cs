using DG.Tweening;

public class ScaleAnimationStrategy : IPopupAnimationStrategy
{
    public void DoAnimation(UIAnimationTarget target, PopupAnimationStrategyConfig config)
    {
        var popupRectTransform = target.Rect;
        popupRectTransform.DOScale(config.AnimateTo.Scale, config.Duration)
                          .SetEase(config.Ease);
    }
}