using System.ComponentModel;
using System.Windows;

namespace PSQuickAssets.WPF;

internal static class WPFExtensionMethods
{
    public static bool IsInDesignMode(this Application app)
    {
        return (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
    }
}
