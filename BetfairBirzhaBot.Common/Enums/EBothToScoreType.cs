using BetfairBirzhaBot.Filters.Common;

namespace BetfairBirzhaBot.Common.Entities
{
    public enum EBothToScoreType
    {
        NONE,
        [MarketFullDescription("Да")]
        Yes,
        [MarketFullDescription("Нет")]
        No
    }
}
