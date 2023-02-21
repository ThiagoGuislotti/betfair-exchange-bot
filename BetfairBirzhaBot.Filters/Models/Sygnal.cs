using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BetfairBirzhaBot.Filters.Models
{
    public class Sygnal
    {
        public EMarket Type { get; set; }
        public EFilterCondition Condition { get; set; }
        public double FilterParameter { get; set; }
        public double SecondParameter { get; set; }
        public double TotalParameter { get; set; }
        public ETotalType TotalType { get; set; }
        public ETimePart TimePart { get; set; }
        public string Name { get; set; }
        public double Coefficient { get; set; }
        public bool IsValid { get; set; }
        public string Description { get; set; }
        public Game Game { get; set; }
        public string MarketId { get; set; }
        public string SelectionId { get; set; }


        public Sygnal(EMarket type, string name, double coefficient, EFilterCondition condition, double filterParameter, double secondParameter = 0)
        {
            Type = type;
            Name = name;
            Coefficient = coefficient;
            Condition = condition;
            FilterParameter = filterParameter;
            SecondParameter = secondParameter;

            IsValid = true;
        }

        public static Sygnal Fail => new Sygnal();
        public Sygnal()
        {
            IsValid = false;
        }
    }

    public class StrategySygnalResult
    {
        public List<Sygnal> SuccessedFilterSygnals { get; set; } = new();

        public static StrategySygnalResult Failed => new StrategySygnalResult();
        public string StrategyName { get; set; }
        public int Index { get; set; }
        public DateTime Created { get; }
        public StrategySygnalResult()
        {
            IsActive = false;
        }

        public StrategySygnalResult(List<Sygnal> succesedSygnals, string strategyId, Game game)
        {
            SuccessedFilterSygnals = succesedSygnals;
            StrategyId = strategyId;
            Game = JsonConvert.DeserializeObject<Game>(JsonConvert.SerializeObject(game));
            IsActive = true;
            Id = Guid.NewGuid().ToString();
            Created = DateTime.Now;
        }
        public string Id { get; set; }
        public bool IsActive { get; set; }
        public Game Game { get; set; }
        public string MarketId { get; set; }
        public string SelectionId { get; set; }
        public double Coefficient { get; set; }
        public string StrategyId { get; set; }
    }
}
