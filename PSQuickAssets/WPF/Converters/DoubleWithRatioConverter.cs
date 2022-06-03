using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace PSQuickAssets.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class DoubleWithRatioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double input) throw new ArgumentException("Input Value should be of type double");
            if (parameter is null) return input;
            if (parameter is not double ratio) throw new ArgumentException("Converter Parameter should be of type double");

            return input * ratio;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double input) throw new ArgumentException("Input Value should be of type double");
            if (parameter is null) return input;
            if (parameter is not double ratio) throw new ArgumentException("Converter Parameter should be of type double");
            if (ratio == 0) throw new ArgumentException("Cannot convert. Dividing by zero.");

            return input / ratio;
        }
    }
}
