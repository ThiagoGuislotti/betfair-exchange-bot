namespace BetfairBirzhaBot.Common.Entities
{
    public class CorrectScoreMarket
    {
        public string SelectionId { get; set; }
        public string ScoreText { get; set; }
        public int Home { get; set; }
        public int Away { get; set; }
        public double Coefficient { get; set; }
        public string MarketId { get; set; }
    }
}
