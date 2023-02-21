namespace BetfairBirzhaBot.Common.Entities
{
    public class TotalMarket
    {
        public string Id { get; set; }
        public string MarketId { get; set; }
        public string SelectionId { get; set; }
        public double Parameter { get; set; }
        public string Name { get; set; }
        public TotalMarketData Over { get; set; } = new();
        public TotalMarketData Under { get; set; } = new();

    }

    public class TotalMarketData
    {
        public double Coefficient { get; set; }
        public string SelectionId { get; set; }
        public string MarketId { get; set; }

    }
}
