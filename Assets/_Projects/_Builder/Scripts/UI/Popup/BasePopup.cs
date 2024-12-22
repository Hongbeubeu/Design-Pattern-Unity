using System;
using UnityEngine;

[Serializable]
public class BasePopup : UIElement, IPopup
{
    [SerializeField]
    private RectTransform _rect;

    public RectTransform Rect => _rect;
}