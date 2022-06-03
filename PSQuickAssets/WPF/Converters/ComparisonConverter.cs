using System.Globalization;

namespace PSQuickAssets.WPF.Converters;

public class ComparisonConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.Equals(parameter)!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? parameter : Binding.DoNothing;
    }
}
