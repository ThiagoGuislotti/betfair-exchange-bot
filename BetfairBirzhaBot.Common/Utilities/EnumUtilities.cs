using BetfairBirzhaBot.Filters.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BetfairBirzhaBot.Utilities
{
    public static class EnumUtility
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static string GetMarketDescription(this Enum enumObj)
        {
            if (enumObj is null) return "not found";
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                MarketFullDescription attrib = attribArray[0] as MarketFullDescription;
                return attrib.Description;
            }
        }

        public static T CastEnumObjToValue<T>(this Enum enumObj)
        {
            T enumVal = (T)Enum.Parse(typeof(T), enumObj.ToString());

            return enumVal;
        }
    }
}
