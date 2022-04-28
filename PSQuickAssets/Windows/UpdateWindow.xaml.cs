using PureUI;

namespace PSQuickAssets.Windows;

public partial class UpdateWindow : PureWindow
{
    public UpdateWindow()
    {
        InitializeComponent();
    }

    private void OpenDownloadPageButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        this.Close();
        e.Handled = true;
    }
}