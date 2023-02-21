using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Models;
using System.Collections.Generic;

namespace BetfairBirzhaBot.Settings
{
    public class SessionSettings
    {
        public UserRequestData ParserBotRequestData { get; set; } = new();
        public UserRequestData BetBotRequestData { get; set; } = new();
        public List<Strategy> Strategies { get; set; } = new();
        public TelegramSettings TelegramSettings { get; set; } = new();
        public BookmakerAccountData BookmakerAccountData { get; set; } = new();
        public List<LeagueModel> Leagues { get; set; } = new();


        public void AddNewLeagues(List<string> leagues)
        {
           foreach (var league in leagues)
            {
                if (Leagues.Exists(l => l.Name == league))
                    continue;

                Leagues.Add(new LeagueModel(league));
            }
        }


        public List<StrategySygnalResult> SygnalStore { get; set; } = new();
        public int LastSygnalIndex { get; set; } = 0;
        public int MaxGamesToParsingCount { get; set; } = 0;
    }
}
