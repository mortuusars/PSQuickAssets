using PureUI.Themes;

namespace PSQuickAssets.Services;

internal class ThemeService
{
    private readonly ThemeManager _themeManager;
    private readonly IConfig _config;

    public ThemeService(ThemeManager themeManager, IConfig config)
    {
        _themeManager = themeManager;
        _config = config;
    }

    public void Initialize()
    {
        _themeManager.AddCompiledTheme("Dark", "../Resources/Themes/Dark.xaml");
        _themeManager.AddCompiledTheme("Light", "../Resources/Themes/Light.xaml");
        DirectoryThemeProvider themeProvider = new DirectoryThemeProvider(Folders.Themes);
        themeProvider.WriteExampleFile(new Uri("pack://application:,,,./Resources/Themes/Dark.xaml"));
        _themeManager.ThemeProviders.Add(themeProvider);
        _themeManager.LoadThemes();

        // Set theme from the config.
        _themeManager.CurrentTheme = _themeManager.Themes.FirstOrDefault(t => t.Name == _config.Theme) ?? _themeManager.CurrentTheme;
    }
}
