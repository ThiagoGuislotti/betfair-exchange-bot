using BetfairBirzhaBot.Common.Entities.ResponseEntities;
using Newtonsoft.Json;
using System.ComponentModel;

namespace BetfairBirzhaBot.Common.Entities
{

    public class Freeze
    {
        public bool IsFreezed { get; set; } = false;
        public DateTime FromMinutes { get; set; }
        public int FreezedForMinutes { get; set; }

    }
    public class Game
    {
        public Game()
        {

        }

        public Game(string eventId)
        {
            Url = "https://www.betfair.com/exchange/plus/ru/%D1%84%D1%83%D1%82%D0%B1%D0%BE%D0%BB/uefa-women-s-euro/%D0%B0%D0%BD%D0%B3%D0%BB%D0%B8%D1%8F-%D0%B6-%D0%B3%D0%B5%D1%80%D0%BC%D0%B0%D0%BD%D0%B8%D1%8F-%D0%B6-%D1%81%D1%82%D0%B0%D0%B2%D0%B8%D1%82%D1%8C-" + eventId;
            //EventId = url.Split('-').ToList().Last();
            IsUpdating = false;
            Id = Guid.NewGuid().ToString();
            EventId = eventId;
        }

        public Game(string eventId, string name)
        {
            EventId = eventId;
            IsUpdating = false;
            Id = Guid.NewGuid().ToString();
            Title = name;
        }

        public string InPlayMatchStatus { get; set; }

        public string Status { get; set; }
        public string Id { get; set; }
        public string EventId { get; set; }
        public string Url { get; set; }
        [JsonIgnore]
        public string Title { get; set; }

        public string GetTitle()
        {
            return Teams[0].Name + " - " + Teams[1].Name;
        }
        public string League { get; set; } = null;
        public List<Team> Teams { get; set; } = new();

        [JsonIgnore]
        public int Total => Teams.Count == 2 ? Teams[0].Score + Teams[1].Score : 0;
        public int ElapsedMinutes { get; set; } = -1;
        public int ConcreteElapsedMinutes { get; set; } = 0;
        public int LastHomeScore { get; set; } = -1;
        public int LastAwayScore { get; set; } = -1;
        public int FirstHalfHomeScore { get; set; } = -1;
        public int FirstHalfAwayScore { get; set; } = -1;
        public int LastScoreUpdatedMinute { get; set; } = 0;

        public List<DoubleChanceMarket> DoubleChanceMarketsStart { get; set; } = new();
        public List<DoubleChanceMarket> DoubleChanceMarketsCurrent { get; set; } = new();

        public List<WinMarket> WinMarketsStartGame { get; set; } = new();
        public List<WinMarket> WinMarketsCurrent { get; set; } = new();
    
        public List<TotalMarket> TotalMarketsFirstHalfStart { get; set; } = new();
        public List<TotalMarket> TotalMarketsFirstHalfCurrent { get; set; } = new();
        public List<TotalMarket> TotalMarketsStart { get; set; } = new();
        public List<TotalMarket> TotalMarketsCurrent { get; set; } = new();

        public List<BothToScoreMarket> BothToScoreMarketsStart { get; set; } = new();
        public List<BothToScoreMarket> BothToScoreMarketsCurrent { get; set; } = new();

        public Freeze Freeze = new();

        public void FreezeFor(int minutes)
        {
            Freeze.FreezedForMinutes = minutes;
            Freeze.IsFreezed = true;
            Freeze.FromMinutes = DateTime.Now;

        }


        public bool IsActive()
        {
            return Teams.Count == 2 &&
                !string.IsNullOrEmpty(Teams[0].Name) &&
                !string.IsNullOrEmpty(Teams[1].Name) &&
                ElapsedMinutes is not -1;
        }

        public List<CorrectScoreMarket> CorrectScoreMarkets { get; set; } = new();
        public List<CorrectScoreMarket> CorrectScoreMarketsPrematch { get; set; } = new();

        public EventTimeline EventTimeline { get; set; }

        public bool IsUpdating { get; set; }
        public StatisticsResponse Statistics { get; set; }
        
        [JsonIgnore]
        public DateTime LastUpdated { get; set; }
    }
}
