using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MTerminal.WPF;
using System.Diagnostics;
using System.Media;
using System.Windows;
using System.Windows.Controls;
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
    }, () => App.ServiceProvider.GetRequiredService<IConfig>().DebugMode); //TODO: MenuItem not working.

    /// <summary>
    /// Exits the app.
    /// </summary>
    public static ICommand Shutdown { get; } = new RelayCommand(() => App.Current.Shutdown());

    /// <summary>
    /// Copies text to the <see cref="Clipboard"/> from a <see cref="TextBox"/> provided as a <see cref="CommandParameter"/>.<br></br><br></br>
    /// If <see cref="TextBox"/> has a selection - only the selected portion will be copied. Otherwise all text will be copied.
    /// </summary>
    public static ICommand TextBoxCopy { get; } = new RelayCommand<TextBox>((tb) =>
    {
        if (tb is null)
            return;

        if (tb.SelectionLength > 0)
            Clipboard.SetText(tb.SelectedText);
        else
            Clipboard.SetText(tb.Text);
    });

    public static ICommand ShowInExplorer { get; } = new RelayCommand<string>((path) =>
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        string args = $"/e, /select, \"{path}\"";
        ProcessStartInfo info = new("explorer", args);
        Process.Start(info);
    });

    public static ICommand OpenInShell { get; } = new RelayCommand<string>((path) =>
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        try
        {
            ProcessStartInfo info = new(path) { UseShellExecute = true };
            Process.Start(info);
        }
        catch (Exception)
        {
            SystemSounds.Exclamation.Play();
        }
    });
}
