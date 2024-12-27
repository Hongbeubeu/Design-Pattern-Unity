using System;
using UnityEngine;

namespace Builder.UI
{
    [Serializable]
    public class BasePopup : UIElement, IPopup
    {
        [SerializeField]
        private RectTransform _rect;

        public RectTransform Rect => _rect;
    }
}