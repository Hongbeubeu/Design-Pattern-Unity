using DG.Tweening;

public class MoveAnimationStrategy : IPopupAnimationStrategy
{
    public void DoAnimation(UIAnimationTarget target, PopupAnimationStrategyConfig config)
    {
        var popupRectTransform = target.Rect;
        popupRectTransform.DOLocalMove(config.AnimateTo.Position, config.Duration)
                          .SetEase(config.Ease);
    }
}