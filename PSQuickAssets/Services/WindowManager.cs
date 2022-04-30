using CommunityToolkit.Mvvm.Input;
using MTerminal.WPF;
using PSQuickAssets.Utils;
using PSQuickAssets.ViewModels;
using PSQuickAssets.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace PSQuickAssets.Services;

internal partial class WindowManager
{
    private AssetsWindow? _assetsWindow;
    private readonly IConfig _config;

    public WindowManager(IConfig config)
    {
        _config = config;

        // Set default tooltip delay:
        ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(350, FrameworkPropertyMetadataOptions.Inherits));
    }

    public void FocusPhotoshop()
    {
        WindowUtils.FocusWindow("photoshop");
    }

    /// <summary>
    /// Shows main window. Creates it if needed.
    /// </summary>
    public void ShowMainWindow()
    {
        if (_assetsWindow is null)
            _assetsWindow = new AssetsWindow();

        _assetsWindow.Show();
    }

    /// <summary>
    /// Toggles state of the main window. If window was hidden - shows it. And vise versa.
    /// </summary>
    [ICommand]
    public void ShowHideMainWindow()
    {
        _assetsWindow?.ToggleVisibility();
    }

    /// <summary>
    /// Hides main window.
    /// </summary>
    public void HideMainWindow()
    {
        _assetsWindow?.Hide();
    }

    /// <summary>
    /// Closes main window.
    /// </summary>
    public void CloseMainWindow()
    {
        _assetsWindow?.Close();
        _assetsWindow = null;
    }

    /// <summary>
    /// Creates and shows <see cref="SettingsWindow"/>. If window is already opened - it will be activated and brought to foreground.
    /// </summary>
    [ICommand]
    internal void ShowSettings()
    {
        if (App.Current.Windows.OfType<SettingsWindow>().FirstOrDefault() is SettingsWindow settingsWindow)
        {
            settingsWindow.Show();
            settingsWindow.Activate();
            return;
        }

        settingsWindow = new SettingsWindow();
        settingsWindow.Owner = _assetsWindow;
        settingsWindow.Show();
    }

    /// <summary>
    /// Closes <see cref="SettingsWindow"/> if it was opened.
    /// </summary>
    /// <returns><see langword="true"/> if closed successfully.</returns>
    internal bool CloseSettingsWindow()
    {
        if (App.Current.Windows.OfType<SettingsWindow>().FirstOrDefault() is SettingsWindow settingsWindow)
        {
            settingsWindow.Close();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Toggle state of the settings window. If settings window is closed - opens it. If opened - closes.
    /// </summary>
    public void ToggleSettingsWindow()
    {
        var settingsWindow = App.Current.Windows.OfType<SettingsWindow>().FirstOrDefault();

        if (settingsWindow is null)
        {
            settingsWindow = new SettingsWindow();
            settingsWindow.Owner = _assetsWindow;
            settingsWindow.Show();
        }
        else
        {
            settingsWindow.Close();
        }
    }

    /// <summary>
    /// Creates and shows update window with provided info about new version.
    /// </summary>
    /// <param name="currentVersion"></param>
    /// <param name="newVersion"></param>
    public void ShowUpdateWindow(Version currentVersion, Version newVersion)
    {
        UpdateWindow updateWindow = new();
        updateWindow.Owner = _assetsWindow;
        updateWindow.DataContext = new UpdateViewModel
        {
            CurrentVersion = currentVersion,
            NewVersion = newVersion,
        };
        updateWindow.Show();
    }

    internal void ToggleTerminalWindow()
    {
        if (!Terminal.IsOpen || !Terminal.IsVisible)
            Terminal.Show();
        else
            Terminal.Close();
    }

    /// <summary>
    /// Gets handle to the main window.
    /// </summary>
    /// <returns>Window handle.</returns>
    /// <exception cref="Exception">Thrown if getting failed.</exception>
    public IntPtr GetMainWindowHandle()
    {
        return new WindowInteropHelper(_assetsWindow).Handle;
    }
}