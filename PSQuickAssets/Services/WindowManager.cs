using MTerminal.WPF;
using PSQuickAssets.Utils;
using PSQuickAssets.ViewModels;
using PSQuickAssets.Windows;
using System;
using System.Linq;
using System.Windows.Interop;

namespace PSQuickAssets.Services;

internal class WindowManager
{
    private readonly IConfig _config;

    internal AssetsWindow? AssetsWindow { get; private set; }

    public WindowManager(IConfig config)
    {
        _config = config;
    }

    public void FocusPhotoshop()
    {
        if (_config.HideWindowWhenAddingAsset)
            AssetsWindow?.Hide();
        WindowControl.FocusWindow("photoshop");
    }

    /// <summary>
    /// Shows main window. Creates it if needed.
    /// </summary>
    public void ShowMainWindow()
    {
        if (AssetsWindow is null)
            AssetsWindow = new AssetsWindow();

        AssetsWindow.Show();
    }

    /// <summary>
    /// Toggles state of the main window. If window was hidden - shows it. And vise versa.
    /// </summary>
    public void ToggleMainWindow()
    {
        AssetsWindow?.ToggleVisibility();

        //if (AssetsWindow?.IsVisible is true)
        //    HideMainWindow();
        //else 
        //    ShowMainWindow();
    }

    /// <summary>
    /// Hides main window.
    /// </summary>
    public void HideMainWindow()
    {
        AssetsWindow?.Hide();
    }

    /// <summary>
    /// Closes main window.
    /// </summary>
    public void CloseMainWindow()
    {
        AssetsWindow?.Close();
        AssetsWindow = null;
    }

    /// <summary>
    /// Creates and shows <see cref="SettingsWindow"/>. If window is already opened - it will be activated and brought to foreground.
    /// </summary>
    internal void ShowSettingsWindow()
    {
        if (App.Current.Windows.OfType<SettingsWindow>().FirstOrDefault() is SettingsWindow settingsWindow)
        {
            settingsWindow.Show();
            settingsWindow.Activate();
            return;
        }

        settingsWindow = new SettingsWindow();
        settingsWindow.Owner = AssetsWindow;
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
            settingsWindow.Show();
            settingsWindow.Owner = AssetsWindow;
            settingsWindow.Left = WpfScreenHelper.MouseHelper.MousePosition.X;
            settingsWindow.Top = WpfScreenHelper.MouseHelper.MousePosition.Y - 60;
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
    /// <param name="changelog"></param>
    public void ShowUpdateWindow(Version currentVersion, Version newVersion, string changelog)
    {
        UpdateWindow updateWindow = new();
        updateWindow.DataContext = new UpdateViewModel(currentVersion, newVersion, changelog);
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
        return new WindowInteropHelper(AssetsWindow).Handle;
    }
}