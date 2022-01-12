using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PSQuickAssets.Controls;

//[ContentProperty(nameof(Control))]
public partial class SettingItem : UserControl
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(SettingItem), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty InfoProperty =
        DependencyProperty.Register("Info", typeof(string), typeof(SettingItem), new PropertyMetadata(string.Empty));
    
    public static readonly DependencyProperty ControlProperty =
        DependencyProperty.Register("Control", typeof(object), typeof(SettingItem), new PropertyMetadata(null));
    
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public string Info
    {
        get { return (string)GetValue(InfoProperty); }
        set { SetValue(InfoProperty, value); }
    }

    public object Control
    {
        get { return (object)GetValue(ControlProperty); }
        set { SetValue(ControlProperty, value); }
    }

    public SettingItem()
    {
        InitializeComponent();
    }
}