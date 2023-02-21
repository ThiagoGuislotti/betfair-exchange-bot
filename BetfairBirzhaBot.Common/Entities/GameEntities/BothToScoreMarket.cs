namespace BetfairBirzhaBot.Common.Entities
{
    public class BothToScoreMarket
    {
        public EBothToScoreType Type { get; set; }
        public string MarketId { get; set; }
        public string SelectionId { get; set; }
        public double Coefficient { get; set; }
    }
}
