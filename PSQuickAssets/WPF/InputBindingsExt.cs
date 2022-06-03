using System.Windows;
using System.Windows.Input;

namespace PSQuickAssets.WPF;

public class InputBindingsExt
{
    public static readonly DependencyProperty DisableInputBindingsProperty =
    DependencyProperty.RegisterAttached("DisableInputBindings", typeof(bool), typeof(InputBindingsExt), new PropertyMetadata(false, OnDisableChanged));
    public static bool GetDisableInputBindings(DependencyObject obj) => (bool)obj.GetValue(DisableInputBindingsProperty);
    public static void SetDisableInputBindings(DependencyObject obj, bool value) => obj.SetValue(DisableInputBindingsProperty, value);

    private static Dictionary<FrameworkElement, InputBinding[]> _bindings = new();

    private static void OnDisableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        FrameworkElement element = d.CastTo<FrameworkElement>();

        if (e.NewValue is true)
        {
            if (!element.IsInitialized)
            {
                element.Initialized += Element_Initialized;
                return;
            }

            _bindings.Add(element, element.InputBindings.Cast<InputBinding>().ToArray());
            element.InputBindings.Clear();
        }
        else
        {
            if (_bindings.TryGetValue(element, out InputBinding[]? bindings))
            {
                element.InputBindings.AddRange(bindings);
                _bindings.Remove(element);
            }
        }
    }

    private static void Element_Initialized(object? sender, EventArgs e)
    {
        FrameworkElement element = sender!.CastTo<FrameworkElement>();
        _bindings.Add(element, element.InputBindings.Cast<InputBinding>().ToArray());
        element.InputBindings.Clear();
        element.Initialized -= Element_Initialized;
    }
}
