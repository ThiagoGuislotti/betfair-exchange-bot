using BetfairBirzhaBot.Filters.Common;

namespace BetfairBirzhaBot.Filters.Enums
{
    public enum EFilterCondition
    {
        None = 0,
        [MarketFullDescription(">=")]
        BiggerOrEquals = 1,
        [MarketFullDescription("<=")]
        LessOrEquals = 2,
        [MarketFullDescription("=")]
        Equals = 3,
        [MarketFullDescription("!=")]
        NotEquals = 4,
        [MarketFullDescription("<>")]
        BiggerOrLess = 5,
        [MarketFullDescription(">")]
        Bigger = 6,
        [MarketFullDescription("<")]
        Less = 7,
    }
}
