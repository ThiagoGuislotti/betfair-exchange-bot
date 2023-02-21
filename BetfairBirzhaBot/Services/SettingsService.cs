using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Interfaces;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Models;
using BetfairBirzhaBot.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BetfairBirzhaBot.Settings
{
    public class SettingsService : ISettingsService
    {
        private string _fileName = "settings.json";
        private SessionSettings _currentSettings;
        private object _locker = new object();
        public SessionSettings Get()
        {
            //Get from mongodb or initialize default
            if (_currentSettings == null)
                _currentSettings = ReadFromFileOrDefault();

            return _currentSettings;
        }


        public void Save()
        {
            lock (_locker)
            {

                foreach (var s in _currentSettings.Strategies)
                {
                    var leagues = _currentSettings.Leagues
                    .Where(l => !s.Leagues.Exists(sl => sl.Name == l.Name)).ToList();

                    s.Leagues.AddRange(leagues.Select(l => new LeagueModel(l.Name))) ;


                    if (s.MarketFilters is null)
                        s.MarketFilters = new MarketFilters();
                    s.MarketFilters.ResultsFilters = CastToSpecificFilter<ResultsFilter>(s.Filters, EFilterGroup.Results);
                    s.MarketFilters.StaticFilters = CastToSpecificFilter<StaticDataFilter>(s.Filters, EFilterGroup.Static);
                    s.MarketFilters.TotalFilters = CastToSpecificFilter<TotalsFilter>(s.Filters, EFilterGroup.Total);
                    s.MarketFilters.CorrectScoreFilters = CastToSpecificFilter<CorrectScoreFilter>(s.Filters, EFilterGroup.CorrectScore);
                    s.MarketFilters.BothToScoreFilters = CastToSpecificFilter<BothToScoreFilter>(s.Filters, EFilterGroup.BothToScore);
                }
                File.WriteAllText(_fileName, JsonConvert.SerializeObject(_currentSettings));
            }
        }

        private static List<T> CastToSpecificFilter<T>(List<IFilter> filters, EFilterGroup group)
        {

            var result =filters
                    .Where(f => f.Group == group)
                    .Select(x => (T)x)
                    .ToList();

            return result;
        }

        private SessionSettings ReadFromFileOrDefault()
        {
            try
            {
                if (!File.Exists(_fileName))
                    File.WriteAllText(_fileName, "");

                string text = File.ReadAllText(_fileName);

                if (string.IsNullOrEmpty(text))
                    return new SessionSettings();

                var settings = JsonConvert.DeserializeObject<SessionSettings>(text);

                foreach (var strategy in settings.Strategies)
                {
                    if (strategy.MarketFilters is null)
                        continue;

                    strategy.Filters.AddRange(strategy.MarketFilters.ResultsFilters);
                    strategy.Filters.AddRange(strategy.MarketFilters.StaticFilters);
                    strategy.Filters.AddRange(strategy.MarketFilters.TotalFilters);
                    strategy.Filters.AddRange(strategy.MarketFilters.CorrectScoreFilters);
                    strategy.Filters.AddRange(strategy.MarketFilters.BothToScoreFilters);
                }

                return settings;
            }
            catch (Exception ex)
            {
                return new SessionSettings();
            }
        }
    }

    
}
