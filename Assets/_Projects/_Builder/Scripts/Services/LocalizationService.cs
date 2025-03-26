using hcore.IoC.Services;
using UnityEngine.Localization.Settings;

namespace Builder.Services
{
    public class LocalizationService : BaseService
    {
        protected override void InitializeService()
        {
        }

        protected override void CleanupService()
        {
        }

        public void SetLocale(string locale)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(locale);
        }
    }
}