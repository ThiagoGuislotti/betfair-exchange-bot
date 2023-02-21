using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Enums;

namespace BetfairBirzhaBot.Filters.Models
{

    public class BothToScoreFilter : FilterBase
    {
        public BothToScoreFilter(string name, EMarket type, EBothToScoreType bothType, EFilterCondition condition, bool prematch, double from = 1, double to = 1) : base(name, type)
        {
            Condition = condition;
            Group = EFilterGroup.BothToScore;
            From = from;
            To = to;
            IsPrematch = prematch;
            BothScoreType = bothType;
        }

        public double From { get; set; }
        public double To { get; set; }
        public EBothToScoreType BothScoreType { get; set; }
        public override Sygnal Check(Game game)
        {
            var markets = IsPrematch ? game.BothToScoreMarketsStart : game.BothToScoreMarketsCurrent;

            var market = markets.Find(x => x.Type == BothScoreType);
            if (market == null)
                return Sygnal.Fail;

            return CheckConditions(market.Coefficient, From, To);
        }

    }

    
}
