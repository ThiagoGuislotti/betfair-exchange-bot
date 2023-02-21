using BetfairBirzhaBot.Filters.Common;

namespace BetfairBirzhaBot.Filters.Enums
{
    public enum ETotalType
    {
        None = 0,
        [MarketFullDescription("ТБ")]
        Over = 1,
        [MarketFullDescription("ТМ")]
        Under = 2
    }
}
