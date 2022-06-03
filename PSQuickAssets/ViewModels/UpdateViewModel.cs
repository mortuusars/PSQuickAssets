using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;

namespace PSQuickAssets.ViewModels;

public partial class UpdateViewModel
{
    public Version CurrentVersion { get; set; } = new Version();
    public Version NewVersion { get; set; } = new Version();

    public string DownloadPageUrl { get; } = "https://github.com/mortuusars/PSQuickAssets/releases";
    public string ChangelogUrl { get; } = "https://github.com/mortuusars/PSQuickAssets/blob/master/changelog.md";

    [ICommand]
    private void OpenDownloadPage()
    {
        try
        {
            Process.Start(new ProcessStartInfo(DownloadPageUrl) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Cannot open link: {ex.Message}", App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [ICommand]
    private void OpenChangelogPage()
    {
        try
        {
            Process.Start(new ProcessStartInfo(ChangelogUrl) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Cannot open link: {ex.Message}", App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
