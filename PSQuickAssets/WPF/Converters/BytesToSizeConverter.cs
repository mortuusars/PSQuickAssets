using System;
using System.Globalization;
using System.Windows.Data;

namespace PSQuickAssets.WPF.Converters;

[ValueConversion(typeof(long), typeof(string))]
public class BytesToSizeConverter : IValueConverter
{
    public static BytesToSizeConverter Instance
    {
        get
        {
            if (_instance is null)
                _instance = new BytesToSizeConverter();

            return _instance;
        }
    }
    private static BytesToSizeConverter? _instance;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double bytes = (long)value;

        if (bytes < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Number of bytes cannot be negative.");

        int numberOfDivisions = 0;
        string[] chars = new string[] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        while (bytes > 1024)
        {
            bytes /= 1024d;
            numberOfDivisions++;
        }

        // No decimal part if KB.
        double number = numberOfDivisions == 1 ? Math.Round(bytes, 0) : Math.Round(bytes, 1);

        return $"{number} {chars[numberOfDivisions]}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}