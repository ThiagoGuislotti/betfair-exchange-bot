using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Enums;

namespace BetfairBirzhaBot.Filters.Models
{
    public class CorrectScoreFilter : FilterBase
    {
        public CorrectScoreFilter(string name, EMarket type) : base(name, type)
        {
            Group = EFilterGroup.CorrectScore;
        }

        public int Home { get; set; }
        public int Away { get; set; }
        public double From { get; set; }
        public double To { get; set; }

        public override Sygnal Check(Game game)
        {
            var market = game.CorrectScoreMarkets.Find(market => market.Home == Home && market.Away == Away);
            if (market is null)
                return new Sygnal();

            return CheckConditions(market.Coefficient, From, To);
        }
    }
}
