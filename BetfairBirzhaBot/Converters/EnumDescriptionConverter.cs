using BetfairBirzhaBot.Filters.Common;
using BetfairBirzhaBot.Utilities;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace BetfairBirzhaBot.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObj)
        {
            return enumObj.GetMarketDescription();
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return "";
            if (string.IsNullOrEmpty(value.ToString()))
                return "Data convert failed";

            Enum myEnum = (Enum)value;
            string description = GetEnumDescription(myEnum);
            return description;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
