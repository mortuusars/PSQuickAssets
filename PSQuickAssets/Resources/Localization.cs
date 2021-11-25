using System.Globalization;
using WPFLocalizeExtension.Engine;

namespace PSQuickAssets.Resources
{
    public class Localization
    {
        public static Localization Instance { get; } = new Localization();

        public static CultureInfo Culture = LocalizeDictionary.Instance.Culture;

        public void SetCulture(string cultureCode)
        {
            var culture = new CultureInfo(cultureCode);
            LocalizeDictionary.Instance.Culture = culture;
        }

        public string this[string key]
        {
            get
            {
                return LocalizeDictionary.Instance.GetLocalizedObject(App.AppName, "Strings", key, LocalizeDictionary.Instance.Culture) as string ?? "localized_key_not_found";
            }
        }
        public string this[string key, string dictionary]
        {
            get
            {
                return LocalizeDictionary.Instance.GetLocalizedObject(App.AppName, dictionary, key, LocalizeDictionary.Instance.Culture) as string ?? "localized_key_not_found";
            }
        }

        public string this[string key, CultureInfo culture]
        {
            get
            {
                return LocalizeDictionary.Instance.GetLocalizedObject(App.AppName, "Strings", key, culture) as string ?? "localized_key_not_found";
            }
        }

        public string this[string key, string dictionary, CultureInfo culture]
        {
            get
            {
                return LocalizeDictionary.Instance.GetLocalizedObject(App.AppName, dictionary, key, culture) as string ?? "localized_key_not_found";
            }
        }
    }
}
