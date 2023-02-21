using BetfairBirzhaBot.Filters.Common;

namespace BetfairBirzhaBot.Filters.Enums
{
    public enum ETimePart
    {
        None = 0,
        [MarketFullDescription("Первый тайм")]
        FirstHalf = 1,
        [MarketFullDescription("Вся игра")]
        FullGame = 2
    }
}
