using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PSQuickAssets.WPF;

public class MouseOverHotkeysBehavior
{
    public static readonly DependencyProperty ListenProperty =
    DependencyProperty.RegisterAttached("Listen", typeof(bool), typeof(MouseOverHotkeysBehavior), new PropertyMetadata(false));

    public static bool GetListen(DependencyObject obj) => (bool)obj.GetValue(ListenProperty);
    public static void SetListen(DependencyObject obj, bool value) => obj.SetValue(ListenProperty, value);

}
