using System;
using UnityEngine;

namespace Builder.UI
{
    [Serializable]
    public class BaseCompositePopup : UICompositeElement, IPopup
    {
        [SerializeField]
        private RectTransform _rect;

        public RectTransform Rect => _rect;
    }
}