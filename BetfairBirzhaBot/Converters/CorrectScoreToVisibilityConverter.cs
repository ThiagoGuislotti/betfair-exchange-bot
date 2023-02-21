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
    public class CorrectScoreToVisibilityConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObj)
        {
            return enumObj.GetMarketDescription();
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EMarket type = ((Enum)value).CastEnumObjToValue<EMarket>();
            return type == EMarket.CorrectScore ? Visibility.Visible : Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
