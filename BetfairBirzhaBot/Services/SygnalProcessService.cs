using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Common.Entities.MarketEntities;
using BetfairBirzhaBot.Converters;
using BetfairBirzhaBot.Core.Telegram;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Services;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.Core
{
    public class SygnalProcessService
    {
        private TelegramSygnalBot _telegram;
        private BotBettingService _betService;
        private ISettingsService _settingsService;
        private SessionSettings _settings;
        private BotParserService _parser;
        private readonly LogService _logger;
        private readonly List<StrategySygnalResult> _blackList = new();
        private readonly Queue<StrategySygnalResult> _workingSygnals = new();

        private readonly Dictionary<EMarket, ETeamType> _marketToWinType = new Dictionary<EMarket, ETeamType> {
                {EMarket.WinAway, ETeamType.Away  },
                {EMarket.WinHome, ETeamType.Home  },
                {EMarket.Draw, ETeamType.Draw  }
            };

        private readonly Dictionary<EMarket, EDoubleChanceType> _marketToDoubleChanceType = new Dictionary<EMarket, EDoubleChanceType> {
                {EMarket.DoubleChanceWinHome, EDoubleChanceType.WinHomeOrDraw  },
                {EMarket.DoubleChanceWinAway, EDoubleChanceType.WinAwayOrDraw},
                {EMarket.DoubleChanceBoth, EDoubleChanceType.WinHomeOrAway  }
            };

        public SygnalProcessService(TelegramSygnalBot telegram, BotBettingService betService, ISettingsService settingsService, LogService logger, BotParserService parserService)
        {
            _telegram = telegram;
            _betService = betService;
            _settingsService = settingsService;
            _logger = logger;
            _settings = _settingsService.Get();
            _parser = parserService;

            Task.Run(async () => await SygnalsWorker());
        }


        private async Task SygnalsWorker()
        {
            while (true)
            {
                if (_workingSygnals.TryDequeue(out var sygnal))
                {
                    sygnal.Index = _settings.LastSygnalIndex++;
                    _settings.SygnalStore.Add(sygnal);
                    await Process(sygnal);
                    _settingsService.Save();
                }
            }
        }

        private async Task Process(StrategySygnalResult sygnal)
        {
            try
            {
                var strategy = _settings.Strategies.Find(x => x.Id == sygnal.StrategyId);
                var sygnalInStore  = _settings.SygnalStore.Find(x => x != null && x.Id == sygnal.Id);
                if (sygnalInStore == null)
                    return;

                _logger.Success($"Сигнал на игру {sygnal.Game.GetTitle()}. Название стратегии : {strategy.Name}");
                sygnal.StrategyName = strategy.Name;

                await _telegram.SendSygnalInfo(sygnal, strategy);

                if (await _parser.IsReady() == false)
                    return;

                var betData = FindMarketForBetting(strategy.Bet, sygnal.Game);
                if (betData.IsMarketExists)
                {
                    betData.SetStakeSumm(strategy.Bet.Stake.ConvertToDouble());

                    var result = await _parser.TryPlaceBet(betData);

                    if (result)
                        await _telegram.SendBetAcceptedInfo(strategy.Bet, sygnal.Game);
                    else
                        await _telegram.SendBetErrorInfo("Ставка не поставилась.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FAILED TO PROCESS SYGNAL");
                Debug.WriteLine($"{ex.ToString()}");
            }
        }

        

        public void AddSygnalToQueue(StrategySygnalResult sygnal)
        {
            if (ExistsInBlackList(sygnal))
                return;

            AddToBlackList(sygnal);
            
            _workingSygnals.Enqueue(sygnal);
        }

        private BetData FindMarketForBetting(BetModel bet, Game game)
        {
            if (bet.Market == EMarket.WinHome || bet.Market == EMarket.WinAway || bet.Market == EMarket.Draw)
            {
                var market = game.WinMarketsCurrent.Find(x => x.Type == _marketToWinType[bet.Market]);
                if (market is null)
                    return BetData.NotExists;

                return new BetData(market.MarketId, market.SelectionId, market.Coefficient);
            }

            if (bet.Market == EMarket.DoubleChanceWinHome || bet.Market == EMarket.DoubleChanceWinAway || bet.Market == EMarket.DoubleChanceBoth)
            {
                var market = game.DoubleChanceMarketsCurrent.Find(x => x.Type == _marketToDoubleChanceType[bet.Market]);
                if (market is null)
                    return BetData.NotExists;

                return new BetData(market.MarketId, market.SelectionId, market.Coefficient);
            }

            if (bet.Market == EMarket.Total)
            {
                var totals = new List<TotalMarket>();
                if (bet.TotalTimePart == ETimePart.None || bet.TotalTimePart == ETimePart.FullGame)
                    totals = game.TotalMarketsCurrent;
                else if (bet.TotalTimePart == ETimePart.FirstHalf)
                    totals = game.TotalMarketsFirstHalfCurrent;

                var market = totals.Find(x => x.Parameter == bet.TotalParameter.ConvertToDouble());
                if (market is null)
                    return BetData.NotExists;

                TotalMarketData marketData = null;
                if (bet.TotalType == ETotalType.Under)
                    marketData = market.Under;
                else if (bet.TotalType == ETotalType.Over)
                    marketData = market.Over;

                if (marketData is null)
                    return BetData.NotExists;

                return new BetData(market.MarketId, marketData.SelectionId, marketData.Coefficient);
            }

            if (bet.Market == EMarket.BothToScore)
            {
                var market = game.BothToScoreMarketsCurrent.Find(x => x.Type == bet.BothToScoreType);
                if (market is null)
                    return BetData.NotExists;

                return new BetData(market.MarketId, market.SelectionId, market.Coefficient);
            }

            if (bet.Market == EMarket.CorrectScore)
            {
                var market = game.CorrectScoreMarkets.Find(x => x.Home == bet.Home && x.Away == bet.Away);
                if (market is null)
                    return BetData.NotExists;

                return new BetData(market.MarketId, market.SelectionId, market.Coefficient);
            }

            return BetData.NotExists;
        }


        private bool ExistsInBlackList(StrategySygnalResult sygnal)
        {
            return _blackList.Exists(s => s.StrategyId == sygnal.StrategyId && s.Game.EventId == sygnal.Game.EventId);
        }

        private void AddToBlackList(StrategySygnalResult sygnal)
        {
            _blackList.Add(sygnal);
        }

        public void ActivateTelegramSending(string key, List<string> names)
        {
            _telegram.Initialize(key, names);
        }
    }
}
