using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class Runner
    {
        [JsonProperty("selectionId")]
        public int SelectionId { get; set; }

        [JsonProperty("description")]
        public RunnerDescription Description { get; set; }

        [JsonProperty("state")]
        public State State { get; set; }

        [JsonProperty("exchange")]
        public Exchange Exchange { get; set; }

        public override string ToString()
        {
            return $"{Description.RunnerName} | {Exchange.AvailableToBack?.First()?.Price}";
        }
    }
}
