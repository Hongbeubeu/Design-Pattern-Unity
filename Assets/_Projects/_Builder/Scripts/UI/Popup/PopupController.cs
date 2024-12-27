using System.Collections.Generic;
using Builder.Editor;
using UnityEngine;

namespace Builder.UI
{
    public class PopupController : MonoBehaviour
    {
        [SerializeField]
        private BasePopup[] _basePopups;

        [SerializeField]
        private BaseCompositePopup[] _baseCompositePopups;

        private readonly List<IPopup> _popups = new();

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _popups.Clear();
            if (_basePopups is { Length: > 0 })
                _popups.AddRange(_basePopups);
            if (_baseCompositePopups is { Length: > 0 })
                _popups.AddRange(_baseCompositePopups);
        }

        public void Show<T>()
        {
            foreach (var popup in _popups)
            {
                if (!popup.Rect.TryGetComponent(typeof(T), out var targetPopup)) continue;
                targetPopup.transform.SetAsLastSibling();
                (targetPopup as IPopup)?.Show();
            }
        }

        [Button("Show Testing Popup", ButtonHeight.Medium)]
        public void ShowTestingPopup()
        {
            Show<TestingPopup>();
        }

        [Button("Show Testing Composite Popup", ButtonHeight.Medium)]
        public void ShowTestingCompositePopup()
        {
            Show<TestingCompositePopup>();
        }
    }
}