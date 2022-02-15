using CommunityToolkit.Mvvm.Input;
using MTerminal.WPF;
using System.Windows.Input;

namespace PSQuickAssets.Commands;

internal static class GeneralCommands
{
    /// <summary>
    /// Shows or closes (if open) <see cref="Terminal"/> window.
    /// </summary>
    public static ICommand ToggleTerminalWindow { get; } = new RelayCommand(() =>
    {
        if (!Terminal.IsOpen || !Terminal.IsVisible)
            Terminal.Show();
        else
            Terminal.Close();
    });

    /// <summary>
    /// Exits the app.
    /// </summary>
    public static ICommand ShutdownCommand { get; } = new RelayCommand(() => App.Current.Shutdown());
}
