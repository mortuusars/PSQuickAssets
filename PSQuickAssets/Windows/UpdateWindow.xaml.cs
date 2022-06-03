using PureUI;

namespace PSQuickAssets.Windows;

public partial class UpdateWindow : PureWindow
{
    public UpdateWindow()
    {
        InitializeComponent();
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);

        if (WindowState == System.Windows.WindowState.Minimized)
            this.Close();
        else if (WindowState == System.Windows.WindowState.Maximized)
            WindowState = System.Windows.WindowState.Normal;
    }

    private void OpenDownloadPageButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        this.Close();
        e.Handled = true;
    }
}