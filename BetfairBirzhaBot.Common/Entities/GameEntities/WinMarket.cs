namespace BetfairBirzhaBot.Common.Entities
{
    public class WinMarket
    {
        public string MarketId { get; set; }
        public ETeamType Type { get; set; }

        public double Coefficient { get; set; } = 0;
        public string SelectionId { get; set; }
        public override string ToString()
        {
            return $"{Type} | {Coefficient}";
        }
    }
}
