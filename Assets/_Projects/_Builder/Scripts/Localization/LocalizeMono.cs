using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Builder.Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizeMono : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private LocalizedString _localizedString;

        [SerializeField]
        private List<string> _locales = new();

        private int _currentIndex;

        private void Start()
        {
            _locales.Clear();
            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                _locales.Add(locale.Identifier.Code);
            }

            _localizedString.StringChanged += UpdateText;
            UpdateText(_localizedString.GetLocalizedString());
        }

        private void UpdateText(string text)
        {
            _text.text = text;
        }
    }
}