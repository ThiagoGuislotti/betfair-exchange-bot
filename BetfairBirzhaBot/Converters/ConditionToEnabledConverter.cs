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
    public class ConditionToEnabledConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObj)
        {
            return enumObj.GetMarketDescription();
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EFilterCondition condition = Enum.Parse<EFilterCondition>(value.ToString());

            return condition == EFilterCondition.BiggerOrLess;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
