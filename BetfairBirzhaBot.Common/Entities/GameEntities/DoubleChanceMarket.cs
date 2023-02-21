using BetfairBirzhaBot.Common.Entities.MarketEntities;

namespace BetfairBirzhaBot.Common.Entities
{
    public class DoubleChanceMarket
    {
        public string MarketId { get; set; }
        public string SelectionId { get; set; }
        public double Coefficient { get; set; }

        public EDoubleChanceType Type { get; set; }
    }
}
