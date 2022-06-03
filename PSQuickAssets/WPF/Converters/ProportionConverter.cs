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
            //TODO: Fix:
            if (values.Length < 2)
                throw new ArgumentException("'values' must contain two items: BitmapImage and double");

            double desiredHeight = (double)values[2];
            
            double ratio = 0;

            try
            {
                double width = System.Convert.ToDouble(values[0]);
                double height = System.Convert.ToDouble(values[1]);

                if (height == 0)
                    return desiredHeight;
                
                ratio = width / height;
            }
            catch (Exception)
            {
                return 30.0;
            }

            if (parameter is double maxRatio && ratio > maxRatio)
                ratio = maxRatio;
            else if (parameter is int maxRatioInt && ratio > maxRatioInt)
                ratio = maxRatioInt;

            return desiredHeight * ratio;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
