using BetfairBirzhaBot.Common.Enums;
using BetfairBirzhaBot.Filters.Common;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Utilities;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace BetfairBirzhaBot.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return  (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }

    public class BoolToVisibilityConverterInverted : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
