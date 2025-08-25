using UnityEngine;
using UnityEngine.UI;

namespace Builder.UI.Components
{
    [RequireComponent(typeof(Image))]
    public class GradientImage : MonoBehaviour
    {
        [SerializeField] private Gradient gradient;
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool loop = true;
    
        private Image image;
        private float currentTime = 0f;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void Update()
        {
            if (gradient == null || gradient.colorKeys.Length == 0) return;
        
            // Cập nhật thời gian
            currentTime += Time.deltaTime * speed;
        
            // Nếu loop thì lặp lại gradient
            if (loop)
            {
                currentTime %= 1f;
            }
            else
            {
                currentTime = Mathf.Clamp01(currentTime);
            }
        
            // Áp dụng màu từ gradient
            image.color = gradient.Evaluate(currentTime);
        }

        // Reset thời gian khi cần
        public void ResetGradient()
        {
            currentTime = 0f;
        }
    }
}