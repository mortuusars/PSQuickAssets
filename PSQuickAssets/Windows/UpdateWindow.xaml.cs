using Microsoft.Extensions.DependencyInjection;
using MortuusUI;
using PSQuickAssets.Services;
using System.Diagnostics;

namespace PSQuickAssets.Windows;

public partial class UpdateWindow : WindowBase
{
    public UpdateWindow()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        e.Handled = true;
        try
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        }
        catch (System.Exception ex)
        {
            DIKernel.ServiceProvider.GetRequiredService<INotificationService>().Notify(ex.Message, NotificationIcon.Error);
        }

        this.Close();
    }
}
