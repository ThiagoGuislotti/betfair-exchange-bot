using System.Windows.Media;

namespace BetfairBirzhaBot.Common.Utilities
{
    public static class BrushUtility
    {
        public static SolidColorBrush ToBrush(this string HexColorString, double opacity = 1)
        {
            var result = (SolidColorBrush)(new BrushConverter().ConvertFrom(HexColorString));
            result.Opacity = opacity;
            return result;
        }
    }
}
