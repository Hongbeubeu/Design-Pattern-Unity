using UnityEngine;

namespace Builder.UI
{
    public interface IPopup : IUIElement
    {
        RectTransform Rect { get; }
    }
}