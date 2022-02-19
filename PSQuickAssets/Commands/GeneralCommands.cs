using CommunityToolkit.Mvvm.Input;
using MTerminal.WPF;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

    /// <summary>
    /// Copies text to the <see cref="Clipboard"/> from a <see cref="TextBox"/> provided as a <see cref="CommandParameter"/>.<br></br><br></br>
    /// If <see cref="TextBox"/> has a selection - only the selected portion will be copied. Otherwise all text will be copied.
    /// </summary>
    public static ICommand TextBoxCopyCommand { get; } = new RelayCommand<TextBox>((tb) =>
    {
        if (tb is null)
            return;

        if (tb.SelectionLength > 0)
            Clipboard.SetText(tb.SelectedText);
        else
            Clipboard.SetText(tb.Text);
    });
}
