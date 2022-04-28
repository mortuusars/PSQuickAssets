using System.Drawing;
using System.Globalization;

namespace PSQuickAssets.WPF.Converters;

[ValueConversion(typeof(Size), typeof(string))]
public class SizeToDimensionsStringConverter : IValueConverter
{
    public static string Convert(Size size)
    {
        return size.IsEmpty ? string.Empty : $"{size.Width}x{size.Height} px";
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert((Size)value);
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