using System;
using System.Globalization;
using System.Windows.Data;

namespace PSQuickAssets.WPF.Converters
{
    public class ProportionConverter : IMultiValueConverter
    {
        public static ProportionConverter Instance { get; }

        static ProportionConverter()
        {
            Instance = new ProportionConverter();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                throw new ArgumentException("'values' must contain two items: BitmapImage and double");

            double width = System.Convert.ToDouble(values[0]);
            double height = System.Convert.ToDouble(values[1]);
            double desiredHeight = (double)values[2];

            if (height == 0)
                return desiredHeight;

            double ratio = width / height;

            if (parameter is double maxRatio && ratio > maxRatio)
                ratio = maxRatio;

            var result = desiredHeight * ratio;

            if (result < 130)
            {
                Console.WriteLine(result);
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
