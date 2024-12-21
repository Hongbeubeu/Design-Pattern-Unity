using DG.Tweening;

public class FadeAnimationStrategy : IPopupAnimationStrategy
{
    public void DoAnimation(UIAnimationTarget target, PopupAnimationStrategyConfig config)
    {
        DOVirtual.Float(target.Group.alpha, config.AnimateTo.Alpha, config.Duration, value => target.Group.alpha = value)
                 .SetEase(config.Ease);
    }
}