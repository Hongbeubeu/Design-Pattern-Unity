using DG.Tweening;

public class UIMoveAnimationStrategy : IUIAnimationStrategy
{
    public void DoAnimation(UIAnimationTarget target, UIAnimationStrategyConfig config)
    {
        var popupRectTransform = target.Rect;
        popupRectTransform.DOLocalMove(config.AnimateTo.Position, config.Duration)
                          .SetEase(config.Ease);
    }
}