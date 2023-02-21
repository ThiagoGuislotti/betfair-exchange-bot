using Newtonsoft.Json;
using System.Text.Json;

namespace BetfairBirzhaBot.Common.Entities.ResponseEntities
{
    public class MarketsResponse
    {
        [JsonProperty("attachments")]
        public Attachments Attachments { get; set; }
    }
}
