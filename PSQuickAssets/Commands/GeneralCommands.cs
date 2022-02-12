using CommunityToolkit.Mvvm.Input;
using MTerminal.WPF;
using System.Windows.Input;

namespace PSQuickAssets;

internal static class GeneralCommands
{
    public static ICommand ToggleTerminalWindow { get; } = new RelayCommand(() =>
    {
        if (!Terminal.IsOpen || !Terminal.IsVisible)
            Terminal.Show();
        else
            Terminal.Close();
    });

    public static ICommand ShutdownCommand { get; } = new RelayCommand(() => App.Current.Shutdown());
}
