using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace _Project._Scripts.Nick
{
    public class LanguageSelector : MonoBehaviour
    {
        private const string PlayerPrefsLocaleKey = "selected_locale";

        public void ChangeLanguage(string languageCode)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales
                .FirstOrDefault(l => l.Identifier.Code == languageCode);

            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
                PlayerPrefs.SetString(PlayerPrefsLocaleKey, languageCode);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.LogWarning($"Locale with code '{languageCode}' not found.");
            }
        }
    }
}