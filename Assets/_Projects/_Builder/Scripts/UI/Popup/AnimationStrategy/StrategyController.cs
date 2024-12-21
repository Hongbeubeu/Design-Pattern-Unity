using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class StrategyController
{
    private IPopupAnimationStrategy[] Strategies { get; }

    public StrategyController(AnimationType[] animationTypes)
    {
        Strategies = GetStrategies(animationTypes);
    }

    private static IPopupAnimationStrategy[] GetStrategies(AnimationType[] animationTypes)
    {
        var strategies = new IPopupAnimationStrategy[animationTypes.Length];
        for (var i = 0; i < animationTypes.Length; i++)
        {
            strategies[i] = GetStrategy(animationTypes[i]);
        }

        return strategies;
    }

    private static IPopupAnimationStrategy GetStrategy(AnimationType type)
    {
        return type switch
               {
                   AnimationType.None => throw new ArgumentException(),
                   AnimationType.Move => new MoveAnimationStrategy(),
                   AnimationType.Scale => new ScaleAnimationStrategy(),
                   AnimationType.Fade => new FadeAnimationStrategy(),
                   AnimationType.Rotate => throw new ArgumentOutOfRangeException(),
                   _ => throw new ArgumentOutOfRangeException()
               };
    }

    public void DoAnimations(UIAnimationTarget target, PopupAnimationStrategyConfig[] configs, Action onBegin = null, Action onComplete = null)
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