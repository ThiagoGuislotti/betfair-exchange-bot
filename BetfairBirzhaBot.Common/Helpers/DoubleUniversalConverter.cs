using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
namespace BetfairBirzhaBot.Converters
{
    public static class DoubleUniversalConverter
    {
        public static double ConvertToDouble(this string text)
        {
            try
            {
                var commaCulture = new CultureInfo("en")
                {
                    NumberFormat =
        {
            NumberDecimalSeparator = ","
        }
                };

                var pointCulture = new CultureInfo("en")
                {
                    NumberFormat =
        {
            NumberDecimalSeparator = "."
        }
                };

                text = text.Trim();

                if (text == "0")
                {
                    return 0;
                }

                if (text.Contains(",") && text.Split(',').Length == 2)
                {
                    return Convert.ToDouble(text, commaCulture);
                }

                if (text.Contains(".") && text.Split('.').Length == 2)
                {
                    return Convert.ToDouble(text, pointCulture);
                }

                return Convert.ToDouble(text);
            }
            catch
            {

            }

            return 0;
        }
    }
}
