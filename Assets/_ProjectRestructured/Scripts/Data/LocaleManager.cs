using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace _ProjectRestructured.Scripts.Data
{
    public class LocaleManager : MonoBehaviour
    {
        private const string PlayerPrefsLocaleKey = "selected_locale";

        private IEnumerator Start()
        {
            yield return LocalizationSettings.InitializationOperation;

            var savedCode = PlayerPrefs.GetString(PlayerPrefsLocaleKey, string.Empty);

            if (string.IsNullOrEmpty(savedCode)) yield break;
            var locale = LocalizationSettings.AvailableLocales.Locales
                .FirstOrDefault(l => l.Identifier.Code == savedCode);

            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
            }
        }
    }
}
