using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LoadingSpinnerControl
{
    public class SpeedDoubleToDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value.ToString(), out double speed))
            {
                var span = TimeSpan.FromSeconds(1);
                return new Duration(span * speed);
            }

            return new Duration(TimeSpan.FromSeconds(1));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
