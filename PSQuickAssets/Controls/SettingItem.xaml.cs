using System.Windows;
using System.Windows.Controls;

namespace PSQuickAssets.Controls;

public partial class SettingItem : UserControl
{
    public string Header
    {
        get { return (string)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(SettingItem), new PropertyMetadata(string.Empty));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(SettingItem), new PropertyMetadata(string.Empty));

    public SettingItem()
    {
        InitializeComponent();
    }
}
