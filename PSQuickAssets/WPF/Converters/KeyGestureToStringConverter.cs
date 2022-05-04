using System.Globalization;
using System.Windows.Input;

namespace PSQuickAssets.WPF.Converters;

[ValueConversion(typeof(KeyGesture), typeof(string))]
public class KeyGestureToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((KeyGesture)value).DisplayString;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}