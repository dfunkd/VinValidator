using System.Globalization;
using System.Windows.Data;

namespace VinValidator.Resources.Converters;

public class ZeroCountToDisabledConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (int.TryParse(values[0].ToString(), out var count))
            return count != 0 || !string.IsNullOrEmpty(values[1].ToString());
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
