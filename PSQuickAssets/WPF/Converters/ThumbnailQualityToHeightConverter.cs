using System.Globalization;

namespace PSQuickAssets.WPF.Converters;

[ValueConversion(typeof(ThumbnailQuality), typeof(double))]
public class ThumbnailQualityToHeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch ((ThumbnailQuality)value)
        {
            case ThumbnailQuality.Low:
                return 70;
            case ThumbnailQuality.Medium:
                return 150;
            case ThumbnailQuality.High:
                return 300;
            default:
                return 150;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
