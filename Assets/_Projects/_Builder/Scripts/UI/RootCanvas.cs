using Builder.Entity;
using UnityEngine;

namespace Builder.UI
{
    public class RootCanvas : MonoBehaviour
    {
        [SerializeField]
        private BuilderStartup _startup;

        [SerializeField]
        private BasePopup[] _basePopups;

        [SerializeField]
        private BaseCompositePopup[] _baseCompositePopups;

        private IPopup[] _popups;

        private void Awake()
        {
            _startup.onInstalled += Load;
        }

        private void Load()
        {
            _popups = new IPopup[_basePopups.Length + _baseCompositePopups.Length];
            _basePopups.CopyTo(_popups, 0);
            _baseCompositePopups.CopyTo(_popups, _basePopups.Length);

            foreach (var popup in _popups)
            {
                popup.Inject(_startup.Resolver);
            }
        }
    }
}