using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace PSQuickAssets.WPF.Converters;

[ValueConversion(typeof(Size), typeof(string))]
public class SizeToImageDimensionsConverter : IValueConverter
{
    public static SizeToImageDimensionsConverter Instance
    {
        get
        {
            if (_instance is null)
                _instance = new SizeToImageDimensionsConverter();

            return _instance;
        }
    }
    private static SizeToImageDimensionsConverter? _instance;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var size = (Size)value;
        if (size.IsEmpty)
            return "-";

        return $"{size.Width}x{size.Height} px";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string input = (string)value;
        if (!string.IsNullOrEmpty(input))
            return Size.Empty;

        int xIndex = input.IndexOf('x');
        int width = System.Convert.ToInt32(input.Substring(0, xIndex));
        int height = System.Convert.ToInt32(input.Substring(xIndex));
        return new Size(width, height);
    }
}