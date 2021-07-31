using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using PSQuickAssets.Models;

namespace PSQuickAssets.Infrastructure.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class FilenameToShortConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string filename = value[0] as string;
            int width = (int)value[1];

            return StringFormatter.CutStart(Path.GetFileNameWithoutExtension(filename), width);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
