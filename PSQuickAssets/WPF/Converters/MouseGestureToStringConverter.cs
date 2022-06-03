using System.Globalization;
using System.Windows.Input;

namespace PSQuickAssets.WPF.Converters;

[ValueConversion(typeof(MouseGesture), typeof(string))]
public class MouseGestureToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        MouseGesture gesture = (MouseGesture)value;

        string hotkey = new MGlobalHotkeys.WPF.Hotkey(Key.None, gesture.Modifiers).ToString();

        return $"{hotkey}+{gesture.MouseAction}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}