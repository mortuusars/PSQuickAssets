using System.Globalization;
using System.IO;

namespace PSQuickAssets.WPF.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class ShortenStringConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Length < 2)
                throw new ArgumentException("2 values should be provided: string and int length.");

            if (value[0] is not string str)
                throw new ArgumentException("First value should be a string (string to shorten).");

            if (value[1] is not int chars)
                throw new ArgumentException("Second value should be an int (number of chars).");

            return StringFormatter.CutStart(Path.GetFileNameWithoutExtension(str), chars);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
