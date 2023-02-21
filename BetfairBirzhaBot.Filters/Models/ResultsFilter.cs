using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Common.Entities.MarketEntities;
using BetfairBirzhaBot.Common.Helpers;
using BetfairBirzhaBot.Filters.Enums;

namespace BetfairBirzhaBot.Filters.Models
{

    public class ResultsFilter : FilterBase
    {
        public ResultsFilter(string name, EMarket type, EFilterCondition condition, bool isPrematch, double from = 1, double to = 1) : base(name, type)
        {
            Condition = condition;
            Group = EFilterGroup.Results;
            From = from;
            To = to;
            IsPrematch = isPrematch;
        }

        public double From { get; set; }
        public double To { get; set; }

        public override Sygnal Check(Game game)
        {
            if (Type == EMarket.WinHome || Type == EMarket.WinAway || Type == EMarket.Draw)
            {
                if (game.WinMarketsCurrent.Count == 0)
                    return Sygnal.Fail;

                WinMarket market = null;
                var winMarkers = IsPrematch ? game.WinMarketsStartGame : game.WinMarketsCurrent;

                if (winMarkers is null || winMarkers.Count is 0)
                    return Sygnal.Fail;


                if (Type == EMarket.WinHome)
                    market = winMarkers.FindByTeamType(ETeamType.Home);
                if (Type == EMarket.WinAway)
                    market = winMarkers.FindByTeamType(ETeamType.Away);
                if (Type == EMarket.Draw)
                    market = winMarkers.FindByTeamType(ETeamType.Home);


                if (market is null)
                    return Sygnal.Fail;

                var result = CheckConditions(market.Coefficient, From, To);

                if (result.IsValid)
                {
                    result.MarketId = market.MarketId;
                    result.SelectionId = market.SelectionId;
                }

                return result;
            }

            if (Type == EMarket.DoubleChanceBoth || Type == EMarket.DoubleChanceWinHome || Type == EMarket.DoubleChanceWinAway)
            {
                DoubleChanceMarket market = null;

                EDoubleChanceType type = EDoubleChanceType.None;

                if (Type == EMarket.DoubleChanceBoth)
                    type = EDoubleChanceType.WinHomeOrAway;
                if (Type == EMarket.DoubleChanceWinHome)
                    type = EDoubleChanceType.WinHomeOrDraw;
                if (Type == EMarket.DoubleChanceWinAway)
                    type = EDoubleChanceType.WinAwayOrDraw;

                market = game.DoubleChanceMarketsCurrent.Find(x => x.Type == type);

                if (market is null)
                    return Sygnal.Fail;

                var result = CheckConditions(market.Coefficient, From, To);

                if (result.IsValid)
                {
                    result.MarketId = market.MarketId;
                    result.SelectionId = market.SelectionId;
                }

                return result;
            }

            return new Sygnal();
        }

    }

    
}
