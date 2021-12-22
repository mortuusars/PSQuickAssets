using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class ThumbnailHeightToWidthProportionConverter : IMultiValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value is not double desiredHeight)
        //        throw new ArgumentException("Only double is supported.");

        //    if (parameter is not BitmapImage thumbnail)
        //        throw new ArgumentException("Parameter of type BitmapImage is required.");

        //    double thWidth = thumbnail.PixelWidth;
        //    double thHeight = thumbnail.PixelHeight;

        //    double ratio = thWidth / thHeight;

        //    return desiredHeight * ratio;
        //}

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                throw new ArgumentException("values must contain two items: BitmapImage and double");

            if (values[0] is not BitmapImage thumbnail)
                throw new ArgumentException("Parameter of type BitmapImage is required.");

            if (values[1] is not double desiredHeight)
                throw new ArgumentException("Only double is supported.");

            double thWidth = thumbnail.PixelWidth;
            double thHeight = thumbnail.PixelHeight;

            double ratio = thWidth / thHeight;

            return desiredHeight * ratio;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
