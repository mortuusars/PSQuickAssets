using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LoadingSpinnerControl
{
    public class ToStrokeDashArrayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 ||
                !double.TryParse(values[0].ToString(), out double diameter) ||
                !double.TryParse(values[1].ToString(), out double thickness) ||
                !double.TryParse(values[2].ToString(), out double lineRatio))
            {
                return 0.0;
            }

            double circumference = Math.PI * diameter;
            double lineLength = circumference * lineRatio;
            double gapLength = circumference - lineLength;

            return new DoubleCollection(new[] { lineLength / thickness, gapLength / thickness });
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
