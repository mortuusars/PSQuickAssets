using System.Globalization;

namespace PSQuickAssets.WPF.Converters;

[ValueConversion(typeof(long), typeof(string))]
public class BytesToSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((long)value).ToSize();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}