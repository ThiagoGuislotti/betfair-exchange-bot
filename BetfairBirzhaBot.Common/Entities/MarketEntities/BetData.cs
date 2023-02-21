namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class BetData
    {
        public static BetData NotExists => new BetData();
        public bool IsMarketExists { get; set; }

        public double Stake { get; set; }
        public string SelectionId { get; set; }
        public string MarketId { get; set; }
        public double Coefficient { get; set; }

        public BetData()
        {
            IsMarketExists = false;
        }

        public BetData(string marketId, string selectionId, double coefficient)
        {
            MarketId = marketId;
            SelectionId = selectionId;
            Coefficient = coefficient;

            IsMarketExists = true;
        }

        public void SetStakeSumm(double stake)
        {
            Stake = stake;
        }
    }
}
