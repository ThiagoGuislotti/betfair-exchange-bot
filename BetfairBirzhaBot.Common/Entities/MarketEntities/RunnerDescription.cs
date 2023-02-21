using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class RunnerDescription
    {
        [JsonProperty("runnerName")]
        public string RunnerName { get; set; }


    }
}
