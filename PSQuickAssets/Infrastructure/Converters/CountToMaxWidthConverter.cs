using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PSQuickAssets.Infrastructure.Converters
{
    [ValueConversion(typeof(int), typeof(double))]
    public class CountToMaxWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;
            return (double)Math.Min(200, Math.Max(70, 200 / (count/10d)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
