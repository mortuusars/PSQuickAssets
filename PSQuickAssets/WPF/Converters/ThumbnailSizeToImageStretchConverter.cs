using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.WPF.Converters
{
    [ValueConversion(typeof(ImageSource), typeof(Stretch))]
    public class ThumbnailSizeToImageStretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapSource image = (BitmapSource)value;

            if (image is not null)
            {
                double ratio = image.PixelWidth / image.PixelHeight;

                if (ratio < 0.5 || ratio > 5)
                    return Stretch.Uniform;
            }

            return Stretch.UniformToFill;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
