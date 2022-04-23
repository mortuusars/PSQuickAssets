using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.WPF.Converters;

[ValueConversion(typeof(string), typeof(string))]
internal class PathToImageDimentionsStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string path)
            return string.Empty;

        try
        {
            using var imageStream = File.OpenRead(path);
            var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
            Size size = new(decoder.Frames[0].PixelWidth, decoder.Frames[0].PixelHeight);
            return SizeToDimensionsStringConverter.Convert(size);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
