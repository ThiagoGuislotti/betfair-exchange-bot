using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Services;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Settings;
using BetfairBirzhaBot.WebBot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.Core
{
    public class BetfairBirzhaBotCore
    {
        private readonly ISettingsService _settingsService;
        private readonly BotParserService _parserService;
        private readonly BotBettingService _betService;
        private readonly SygnalProcessService _sygnalService;
        private readonly LogService _logger;

        private readonly PlaywrightBotFactory _factory;
        private readonly SessionSettings _settings;

        private readonly FilterManager _filterManager;

        private CancellationTokenSource _cancelToken;

        public readonly List<Game> Games = new();
        public readonly List<Strategy> Strategies = new();

        public BetfairBirzhaBotCore(
            ISettingsService settingsService,
            SygnalProcessService sygnalService,
            BotParserService parserService,
            BotBettingService betService,
            LogService logService)
        {
            _factory = new PlaywrightBotFactory();
            _filterManager = new FilterManager();


            _sygnalService = sygnalService;
            _betService = betService;
            _parserService = parserService;
            _settingsService = settingsService;
            _logger = logService;

            _settings = settingsService.Get();

            //excel.UploadSygnals(_settings.SygnalStore);
        }

        public async Task Start()
        {
            LogStart();

            _settingsService.Save();
            _filterManager.SetupStrategies(_settings.Strategies);
            _sygnalService.ActivateTelegramSending(_settings.TelegramSettings.Key, _settings.Strategies.Select(x=>x.Name).ToList());

            await CreateBotServices();
            await StartServices();
        }

        private void LogStart()
        {
            if (_parserService.Active)
            {
                _logger.Warning("Парсер уже запущен.");
                return;
            }

            var activeStrategyNames = _settings.Strategies
                .FindAll(x => x.IsActive)
                .Select(q => q.Name);

            _logger.Info($"Запуск парсера.\nАктивные стратегии : {string.Join("\n", activeStrategyNames)}");
        }

        private void LogStop()
        {
            if (!_parserService.Active)
            {
                _logger.Warning("Парсер уже остановлен.");
                return;
            }

            _logger.Info($"Парсер остановлен");
        }

        public async Task Stop()
        {
            LogStop();
            _parserService.StopParsing();
            _settingsService.Save();
        }


        private async Task CreateBotServices()
        {
            if (_parserService.BrowserExists == false)
            {
                var parserBot = await _factory.CreateBot("");
                await _parserService.SetupContext(parserBot);
                _parserService.OnGameUpdated += _parserService_OnGameUpdated;
            }
        }

        private async Task StartServices()
        {
            Task.Run(async () => await _parserService.StartParsing());

            //if (IsNeedBetService())
            //    await _betService.Start();
        }

        private async Task _parserService_OnGameUpdated(Game game)
        {
            await ProcessStrategyFilters(game).ConfigureAwait(false);
        }


        private bool IsNeedBetService()
        {
            return _settings.Strategies.Exists(x => x.IsActive && x.Bet.Market != EMarket.None) == true;
        }
        private async Task ProcessStrategyFilters(Game game)
        {
            try
            {
                Debug.WriteLine($"Check filters [{game.GetTitle()}]");
                
                var sygnals = await _filterManager.CheckStrategyFilters(game);
                foreach (var sygnal in sygnals)
                    _sygnalService.AddSygnalToQueue(sygnal);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Filters check error");
            }
        }

    }
}
