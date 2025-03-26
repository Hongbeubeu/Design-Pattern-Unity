using System.Collections.Generic;
using hcore.IoC;
using UnityEngine;
using hcore.Tool;

namespace Builder.UI
{
    public class PopupController : MonoBehaviour, IInjectable
    {
        [SerializeField]
        private BasePopup[] _basePopups;

        [SerializeField]
        private BaseCompositePopup[] _baseCompositePopups;

        private readonly List<IPopup> _popups = new();
        private IResolver _resolver;

        private void Start()
        {
            Initialize();
        }

        public void Inject(IResolver initResolver)
        {
        }

        private void Initialize()
        {
            _popups.Clear();
            if (_basePopups is { Length: > 0 })
                _popups.AddRange(_basePopups);
            if (_baseCompositePopups is { Length: > 0 })
                _popups.AddRange(_baseCompositePopups);
        }

        private void Show<T>()
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

        public void ShowChangeLanguagePopup()
        {
            Show<ChangeLanguagePopup>();
        }
    }
}