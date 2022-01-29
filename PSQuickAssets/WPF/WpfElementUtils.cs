using System.Windows;
using System.Windows.Media;

namespace PSQuickAssets.WPF
{
    public static class WpfElementUtils
    {
        public static T? GetParentOfType<T>(FrameworkElement element) where T : FrameworkElement
        {
            DependencyObject parent = element;

            do
            {
                if (parent == null)
                    return null;

                if (parent.GetType() == typeof(T))
                    return (T)parent;

                parent = VisualTreeHelper.GetParent(parent);
            }
            while (parent != null);

            return null;
        }

        public static T? GetParentOfTypeByName<T>(FrameworkElement element, string name) where T : FrameworkElement
        {
            DependencyObject parent = element;

            do
            {
                if (parent == null)
                    return null;

                if (parent.GetType() == typeof(T) && parent is FrameworkElement visual && visual.Name == name)
                    return (T)parent;

                parent = VisualTreeHelper.GetParent(parent);
            }
            while (parent != null);

            return null;
        }
    }
}
