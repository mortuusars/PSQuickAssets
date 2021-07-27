using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace PSQuickAssets.Infrastructure.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class PathToFolderNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DirectoryInfo(Path.GetDirectoryName((string)value)).Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
