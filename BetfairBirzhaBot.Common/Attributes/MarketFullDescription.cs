namespace BetfairBirzhaBot.Filters.Common
{
    [AttributeUsage(AttributeTargets.All)]
    public class MarketFullDescription: Attribute
    {
        public string Description { get; }

        public MarketFullDescription(string name)
        {
            Description = name;
        }
    }
}
