using System.Globalization;
using System.IO;

namespace PSQuickAssets.WPF.Converters;

[ValueConversion(typeof(string), typeof(string))]
public class PathToFileSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string path)
            throw new ArgumentException("Value must be of type string.");

        try
        {
            var file = new FileInfo(path);
            return file.Length.ToSize();
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
