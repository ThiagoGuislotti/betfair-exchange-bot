using Newtonsoft.Json;
using System.Text.Json;

namespace BetfairBirzhaBot.Common.Entities.ResponseEntities
{
    public class Market
    {
        [JsonProperty("marketId")]
        public string MarketId { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("eventTypeId")]
        public int EventTypeId { get; set; }

        [JsonProperty("eventId")]
        public int EventId { get; set; }

        [JsonProperty("upperLevelEventId")]
        public int UpperLevelEventId { get; set; }

        [JsonProperty("topLevelEventId")]
        public int TopLevelEventId { get; set; }

        [JsonProperty("numberOfUpperLevels")]
        public int NumberOfUpperLevels { get; set; }

        [JsonProperty("competitionId")]
        public int CompetitionId { get; set; }

        [JsonProperty("marketName")]
        public string MarketName { get; set; }

        [JsonProperty("marketTime")]
        public DateTime MarketTime { get; set; }

        [JsonProperty("marketType")]
        public string MarketType { get; set; }

        [JsonProperty("inplay")]
        public bool Inplay { get; set; }

        [JsonProperty("totalMatched")]
        public double TotalMatched { get; set; }

        [JsonProperty("totalAvailable")]
        public double TotalAvailable { get; set; }

        [JsonProperty("numberOfRunners")]
        public int NumberOfRunners { get; set; }

        [JsonProperty("numberOfActiveRunners")]
        public int NumberOfActiveRunners { get; set; }

        [JsonProperty("numberOfWinners")]
        public int NumberOfWinners { get; set; }

        [JsonProperty("associatedMarkets")]
        public List<object> AssociatedMarkets { get; set; }

        [JsonProperty("canTurnInPlay")]
        public bool CanTurnInPlay { get; set; }

        [JsonProperty("marketLevels")]
        public List<string> MarketLevels { get; set; }

        [JsonProperty("marketStatus")]
        public string MarketStatus { get; set; }

        [JsonProperty("productType")]
        public string ProductType { get; set; }

        public override string ToString()
        {
            return $"{MarketId} | {MarketName}";
        }
    }
}
