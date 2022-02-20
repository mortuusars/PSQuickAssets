using System.Windows;

namespace PSQuickAssets.WPF;

/// <summary>
/// Helper class that allows referencing DataContext from a StaticResource.
/// </summary>
public class BindingProxy : Freezable //Class should be public for Designer to not show an error.
{
    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }

    public object Data
    {
        get { return (object)GetValue(DataProperty); }
        set { SetValue(DataProperty, value); }
    }

    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
}
