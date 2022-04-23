using System.Globalization;
using WPFLocalizeExtension.Engine;

namespace PSQuickAssets.Resources.Lang;

internal class LocalizationProvider
{
    /// <summary>
    /// Gets the instance of <see cref="LocalizationProvider"/> class.<br></br>
    /// This class is globally imported in GlobalUsings, and this property can be used anywhere without specifying the type.
    /// </summary>
    public static LocalizationProvider Localize { get => _localizer ??= new LocalizationProvider(); }
    private static LocalizationProvider? _localizer;
    private LocalizationProvider() { }

    /// <summary>
    /// Gets or sets the current culture. Lang resources will be automatically changed.
    /// </summary>
    public CultureInfo CurrentCulture
    {
        get => LocalizeDictionary.Instance.Culture; 
        set => LocalizeDictionary.Instance.Culture = value;
    }

    public string this[string key]
    {
        get => GetLocalizedString(key, "Lang", CurrentCulture);
    }
    public string this[string key, string dictionary]
    {
        get => GetLocalizedString(key, dictionary, CurrentCulture);
    }

    public string this[string key, CultureInfo culture]
    {
        get => GetLocalizedString(key, "Lang", culture);
    }

    public string this[string key, string dictionary, CultureInfo culture]
    {
        get => GetLocalizedString(key, dictionary, culture);
    }

    private static string GetLocalizedString(string key, string dictionary, CultureInfo culture)
    {
        return LocalizeDictionary.Instance.GetLocalizedObject(App.AppName, dictionary, key, culture)
            as string ?? "localized_key_not_found";
    }
}
