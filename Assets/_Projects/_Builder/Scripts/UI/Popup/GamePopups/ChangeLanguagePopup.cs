using Builder.Services;
using IoC;

namespace Builder.UI
{
    public class ChangeLanguagePopup : BasePopup
    {
        private LocalizationService _localizationService;

        public override void Inject(IResolver initResolver)
        {
            base.Inject(initResolver);
            _localizationService = Resolver.Resolve<LocalizationService>();
        }

        public void ChangeLanguage(string locale)
        {
            _localizationService.SetLocale(locale);
        }
    }
}