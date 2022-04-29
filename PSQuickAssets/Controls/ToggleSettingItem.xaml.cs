using System.Windows;
using System.Windows.Input;

namespace PSQuickAssets.Controls;

public partial class ToggleSettingItem : SettingItem
{
    public bool IsChecked
    {
        get { return (bool)GetValue(IsCheckedProperty); }
        set { SetValue(IsCheckedProperty, value); }
    }

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(ToggleSettingItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    private FrameworkElement? _panel;

    public ToggleSettingItem()
    {
        InitializeComponent();
    }

    public override void OnApplyTemplate()
    {
        if (_panel is not null)
            _panel.MouseDown -= Panel_MouseDown;

        _panel = this.Template.FindName("PART_RootPanel", this) as FrameworkElement;
        if (_panel is null)
            throw new InvalidOperationException("Template should have a root panel with name 'PART_RootPanel'.");

        _panel.MouseDown += Panel_MouseDown;
    }

    private void Panel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        IsChecked = !IsChecked;
    }
}
