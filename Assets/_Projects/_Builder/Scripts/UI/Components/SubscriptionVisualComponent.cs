using UnityEngine;

public class SubscriptionVisualComponent : MonoBehaviour
{
    [SerializeField] private Animator _animator1;
    [SerializeField] private Animator _animator2;

    public void ClickFreeButton()
    {
        _animator1.Play("BtnAnimation1_Reverse");
    }

    public void ClickPremiumButton()
    {
        _animator1.Play("BtnAnimation1");
    }

    public void ClickMonthlyButton()
    {
        _animator2.Play("BtnAnimation2_Reverse");
    }

    public void ClickAnnualButton()
    {
        _animator2.Play("BtnAnimation2");
    }
}