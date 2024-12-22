using System;
using System.Linq;
using DG.Tweening;

public class UIAnimationController
{
    private IUIAnimationStrategy[] Strategies { get; }

    public UIAnimationController(AnimationType[] animationTypes)
    {
        Strategies = GetStrategies(animationTypes);
    }

    private static IUIAnimationStrategy[] GetStrategies(AnimationType[] animationTypes)
    {
        var strategies = new IUIAnimationStrategy[animationTypes.Length];
        for (var i = 0; i < animationTypes.Length; i++)
        {
            strategies[i] = GetStrategy(animationTypes[i]);
        }

        return strategies;
    }

    private static IUIAnimationStrategy GetStrategy(AnimationType type)
    {
        return type switch
               {
                   AnimationType.None => throw new ArgumentException(),
                   AnimationType.Move => new UIMoveAnimationStrategy(),
                   AnimationType.Scale => new UIScaleAnimationStrategy(),
                   AnimationType.Fade => new UIFadeAnimationStrategy(),
                   AnimationType.Rotate => throw new ArgumentOutOfRangeException(),
                   _ => throw new ArgumentOutOfRangeException()
               };
    }

    public void DoAnimations(UIAnimationTarget target, UIAnimationStrategyConfig[] configs, Action onBegin = null, Action onComplete = null)
    {
        onBegin?.Invoke();
        target.Rect.DOKill();
        var longestDuration = configs.Max(c => c.Duration);
        for (var i = 0; i < Strategies.Length; i++)
        {
            Strategies[i].DoAnimation(target, configs[i]);
        }

        DOVirtual.DelayedCall(longestDuration, () => onComplete?.Invoke());
    }
}