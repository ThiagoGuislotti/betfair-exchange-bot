using BetfairBirzhaBot.Common.Enums;
using BetfairBirzhaBot.Filters.Common;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace BetfairBirzhaBot.Converters
{
    public class EnumerableToVisibilityConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObj)
        {
            return enumObj.GetMarketDescription();
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList list = value as IList;
            if (list is null)
                return Visibility.Collapsed;
            return list.Count is not 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
