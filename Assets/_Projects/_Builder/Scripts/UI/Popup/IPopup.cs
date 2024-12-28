using IoC;
using UnityEngine;

namespace Builder.UI
{
    public interface IPopup : IUIElement, IInjectable
    {
        RectTransform Rect { get; }
    }
}