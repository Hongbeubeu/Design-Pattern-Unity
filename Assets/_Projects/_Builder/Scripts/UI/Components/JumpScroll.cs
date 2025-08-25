using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Builder.UI.Components
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(ScrollRect))]
    public class JumpScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float snapSpeed = 15f;
        [SerializeField] private float dragThreshold = 0.2f; // Ngưỡng kéo để chuyển item
        private ScrollRect scrollRect;
        private RectTransform contentPanel;
        private RectTransform viewport;
        private int currentItem = 0;
        private bool isValidDrag = false;
        private Vector2 startDragPosition;

        void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            contentPanel = scrollRect.content;
            viewport = scrollRect.viewport != null ? scrollRect.viewport : GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Kiểm tra xem drag có bắt đầu trong viewport không
            if (RectTransformUtility.RectangleContainsScreenPoint(viewport, eventData.position, eventData.pressEventCamera))
            {
                isValidDrag = true;
                startDragPosition = contentPanel.anchoredPosition;
                scrollRect.StopMovement();
            }
            else
            {
                isValidDrag = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isValidDrag) return;
            // Cho phép drag bình thường
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isValidDrag) return;

            float dragDistance = (contentPanel.anchoredPosition - startDragPosition).x;
            float itemWidth = contentPanel.GetChild(0).GetComponent<RectTransform>().rect.width;

            // Xác định hướng kéo
            if (Mathf.Abs(dragDistance) > itemWidth * dragThreshold)
            {
                currentItem += -(int)Mathf.Sign(dragDistance);
                currentItem = Mathf.Clamp(currentItem, 0, contentPanel.childCount - 1);
            }

            SnapToItem(currentItem);
            isValidDrag = false;
        }

        void Update()
        {
            if (!isValidDrag && scrollRect.velocity.magnitude < 100)
            {
                SnapToItem(currentItem);
            }
        }

        private void SnapToItem(int itemIndex)
        {
            if (contentPanel.childCount == 0) return;

            float itemWidth = contentPanel.GetChild(0).GetComponent<RectTransform>().rect.width;
            float targetX = -itemIndex * itemWidth;

            contentPanel.anchoredPosition = Vector2.Lerp(
                contentPanel.anchoredPosition,
                new Vector2(targetX, contentPanel.anchoredPosition.y),
                snapSpeed * Time.deltaTime);

            scrollRect.velocity = Vector2.zero;
        }
    }
}