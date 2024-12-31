using Builder.Editor;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Builder.UI.Components
{
    public class ListViewManager : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _content; // Content of Scroll View

        [SerializeField]
        private ScrollRect _scrollRect; // Scroll Rect

        [SerializeField]
        private RectTransform[] _items; // Items in Content

        [SerializeField]
        private float _speed = 1;

        [SerializeField]
        private float _maxDuration = 1;

        [Button]
        public void ScrollToTop()
        {
            ScrollToItem(0);
        }

        [Button]
        public void ScrollToBottom()
        {
            ScrollToItem(_items.Length - 1);
        }

        /// <summary>
        /// Scroll to item with index, using smooth animation with duration
        /// </summary>
        public void ScrollToItem(int index)
        {
            if (index < 0 || index >= _items.Length)
            {
                Debug.LogError("Index out of range!");
                return;
            }

            // Total height of items before the target item
            var totalHeight = 0f;
            for (var i = 0; i < index; i++)
            {
                totalHeight += _items[i].rect.height;
            }

            // Get height of content and viewport
            var contentHeight = _content.rect.height;
            var viewportHeight = _scrollRect.viewport.rect.height;

            // Calculate target scroll value
            var targetScroll = Mathf.Clamp01(totalHeight / (contentHeight - viewportHeight));
            targetScroll = 1 - targetScroll; // Reverse value

            // Calculate duration
            var currentScroll = _scrollRect.verticalNormalizedPosition;
            var distance = Mathf.Abs(targetScroll - currentScroll) * (contentHeight - viewportHeight);
            var duration = distance / _speed;
            duration = Mathf.Clamp(duration, 0, _maxDuration);

            // Scroll to target value with smooth animation
            DOTween.To(
                () => _scrollRect.verticalNormalizedPosition, // Getter: get current value
                value => _scrollRect.verticalNormalizedPosition = value, // Setter: set new value
                targetScroll, // Target value
                duration
            ).SetEase(Ease.OutQuad);
        }
    }
}