using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WindowTemplates.Converters;

[ValueConversion(typeof(Thickness), typeof(Thickness))]
internal class BorderThicknessToHeaderMarginConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var thickness = (Thickness)value;
        return new Thickness(thickness.Left, thickness.Top, thickness.Right, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
