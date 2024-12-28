using System;
using IoC;
using UnityEngine;

namespace Builder.UI
{
    [Serializable]
    public class BaseCompositePopup : UICompositeElement, IPopup
    {
        [SerializeField]
        private RectTransform _rect;

        public RectTransform Rect => _rect;

        protected IResolver Resolver { get; private set; }

        public virtual void Inject(IResolver initResolver)
        {
            Resolver = initResolver;
        }
    }
}