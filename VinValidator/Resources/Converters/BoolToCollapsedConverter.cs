﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VinValidator.Resources.Converters;

public class BoolToCollapsedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => !bool.TryParse(value.ToString(), out bool result) || result ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
