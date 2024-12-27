using DG.Tweening;

namespace Builder.UI
{
    public class UIFadeAnimationStrategy : IUIAnimationStrategy
    {
        public void DoAnimation(UIAnimationTarget target, UIAnimationStrategyConfig config)
        {
            DOVirtual.Float(target.Group.alpha, config.AnimateTo.Alpha, config.Duration, value => target.Group.alpha = value)
                     .SetEase(config.Ease);
        }
    }
}