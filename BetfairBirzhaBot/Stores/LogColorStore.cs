
using BetfairBirzhaBot.Common.Enums;
using BetfairBirzhaBot.Common.Utilities;
using System.Collections.Generic;
using System.Windows.Media;

namespace BetfairBirzhaBot.Common.Helpers
{
    public static class LogColorStore
    {
        public static Dictionary<ELogType, Brush> Brushes = new()
        {
            { ELogType.INFO, "#494949".ToBrush(0.2) },
            { ELogType.SUCCESS, "#35d929".ToBrush(0.2) },
            { ELogType.WARNING, "#d0d929".ToBrush(0.2) },
            { ELogType.ERROR, "#d93529".ToBrush(0.2) },
            { ELogType.PROCESSING, "#4275f5".ToBrush(0.2) }
        };
    }
}
