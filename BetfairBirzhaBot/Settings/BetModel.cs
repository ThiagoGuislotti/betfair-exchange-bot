using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Enums;

namespace BetfairBirzhaBot.Settings
{
    public class BetModel
    {
        public string Stake { get; set; }
        public EMarket Market { get; set; }
        public ETotalType TotalType { get; set; }
        public EBothToScoreType BothToScoreType { get; set; }
        public ETimePart TotalTimePart { get; set; }
        public string TotalParameter { get; set; }

        public int Home { get; set; }
        public int Away { get; set; }
    }
}
