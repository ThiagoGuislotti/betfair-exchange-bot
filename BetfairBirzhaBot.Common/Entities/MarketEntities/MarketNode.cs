using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class MarketNode
    {
        [JsonProperty("marketId")]
        public string MarketId { get; set; }

        [JsonProperty("state")]
        public State State { get; set; }

        [JsonProperty("description")]
        public Description Description { get; set; }

        [JsonProperty("runners")]
        public List<Runner> Runners { get; set; }

        public override string ToString()
        {
            return $"{MarketId} | {Description.MarketName} | {Description.MarketType}";
        }
    }
}
