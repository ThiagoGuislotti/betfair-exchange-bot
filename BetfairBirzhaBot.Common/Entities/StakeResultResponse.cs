using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.Common.Entities
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Instruction
    {
        [JsonProperty("selectionId")]
        public int SelectionId { get; set; }

        [JsonProperty("handicap")]
        public double Handicap { get; set; }

        [JsonProperty("limitOrder")]
        public LimitOrder LimitOrder { get; set; }

        [JsonProperty("orderType")]
        public string OrderType { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }
    }

    public class InstructionReport
    {
        [JsonProperty("instruction")]
        public Instruction Instruction { get; set; }

        [JsonProperty("betId")]
        public string BetId { get; set; }

        [JsonProperty("placedDate")]
        public DateTime PlacedDate { get; set; }

        [JsonProperty("averagePriceMatched")]
        public double AveragePriceMatched { get; set; }

        [JsonProperty("sizeMatched")]
        public double SizeMatched { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("orderStatus")]
        public string OrderStatus { get; set; }
    }

    public class LimitOrder
    {
        [JsonProperty("size")]
        public double Size { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("persistenceType")]
        public string PersistenceType { get; set; }
    }

    public class Data
    {
        [JsonProperty("exceptionname")]
        public string Exceptionname { get; set; }

        [JsonProperty("ExchangeTradingException")]
        public ExchangeTradingException ExchangeTradingException { get; set; }
    }

    public class ExchangeTradingException
    {
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }
    }

    public class Error
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Result
    {
        [JsonProperty("instructionReports")]
        public List<InstructionReport> InstructionReports { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class StakeResultResponse
    {

        [JsonProperty("result")]
        public Result Result { get; set; }

        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }


}
