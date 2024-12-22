using System;
using UnityEngine;

[Serializable]
public class BaseCompositePopup : UICompositeElement, IPopup
{
    [SerializeField]
    private RectTransform _rect;

    public RectTransform Rect => _rect;
}