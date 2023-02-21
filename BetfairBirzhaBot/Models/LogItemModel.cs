
using BetfairBirzhaBot.Common.Enums;
using BetfairBirzhaBot.Common.Helpers;
using System.Windows.Media;

namespace BetfairBirzhaBot.Models
{
    public class LogItemModel
    {
        public string Log { get; set; }
        public Brush Color { get; set; }

        public LogItemModel(string log, ELogType type)
        {
            Log = log;
            Color = LogColorStore.Brushes[type];
        }
    }
}
