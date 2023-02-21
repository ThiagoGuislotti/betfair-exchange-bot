using BetfairBirzhaBot.Filters.Common;

namespace BetfairBirzhaBot.Filters.Enums
{
    public enum EMarket
    {
        None = 0,
        [MarketFullDescription("П1")]
        WinHome = 1,
        [MarketFullDescription("П2")]
        WinAway = 2,
        [MarketFullDescription("Х")]
        Draw = 3,
        [MarketFullDescription("Тотал")]
        Total = 4,
        [MarketFullDescription("Атаки")]
        Attacks = 5,
        [MarketFullDescription("Угловые")]
        Corners = 7,
        [MarketFullDescription("Красные карточки")]
        RedCards = 8,
        [MarketFullDescription("Желтые карточки")]
        YellowCards = 9,
        [MarketFullDescription("Удары в створ")]
        KickToGateBorder = 10,
        [MarketFullDescription("Удары в сторону ворот")]
        KickToGateDirection = 11,
        [MarketFullDescription("Двойной шанс 1X")]
        DoubleChanceWinHome = 12,
        [MarketFullDescription("Двойной шанс 2X")]
        DoubleChanceWinAway = 13,
        [MarketFullDescription("Двойной шанс 12")]
        DoubleChanceBoth = 14,
        [MarketFullDescription("Точный счёт")]
        CorrectScore = 15,
        [MarketFullDescription("Обе забьют")]
        BothToScore = 16,
        [MarketFullDescription("Опасные атаки")]
        DangerousAttacks = 17,
        [MarketFullDescription("Голы")]
        Goals = 18,
    }
}
