using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PSQuickAssets.Controls;

public partial class ToggleSettingItem : UserControl
{
    public string Header
    {
        get { return (string)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(ToggleSettingItem), new PropertyMetadata(string.Empty));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(ToggleSettingItem), new PropertyMetadata(string.Empty));

    public bool IsChecked
    {
        get { return (bool)GetValue(IsCheckedProperty); }
        set { SetValue(IsCheckedProperty, value); }
    }

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(ToggleSettingItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public ToggleSettingItem()
    {
        InitializeComponent();
    }

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        IsChecked = !IsChecked;
    }
}
