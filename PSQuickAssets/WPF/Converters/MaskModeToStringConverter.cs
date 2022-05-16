using PSQA.Core;
using System.Globalization;

namespace PSQuickAssets.WPF.Converters;

[ValueConversion(typeof(MaskMode), typeof(string))]
public class MaskModeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (App.Current.IsInDesignMode())
            return value?.ToString()!;

        MaskMode maskMode = (MaskMode)value;
        return Localize[$"{nameof(MaskMode)}_{maskMode}"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
