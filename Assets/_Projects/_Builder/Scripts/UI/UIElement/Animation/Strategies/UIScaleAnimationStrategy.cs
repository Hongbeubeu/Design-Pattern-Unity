using DG.Tweening;

namespace Builder.UI
{
    public class UIScaleAnimationStrategy : IUIAnimationStrategy
    {
        public void DoAnimation(UIAnimationTarget target, UIAnimationStrategyConfig config)
        {
            var popupRectTransform = target.Rect;
            popupRectTransform.DOScale(config.AnimateTo.Scale, config.Duration)
                              .SetEase(config.Ease);
        }
    }
}