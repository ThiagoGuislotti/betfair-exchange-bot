using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Common.Entities.MarketEntities;
using BetfairBirzhaBot.Common.Helpers;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Parser;
using BetfairBirzhaBot.Parser.RequestManager;
using BetfairBirzhaBot.Parser.RequestManager.Managers;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Settings;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.Services
{
    public class BotBettingService
    {
        private readonly ISettingsService _settingsService;
        private readonly BetfairAPI _api;

        private IBrowserContext _context;
        private SessionSettings _settings;
        private UserRequestData _data;

        private bool _serviceReady = false;

        public bool BrowserExists { get; set; } = false;
        public bool Active { get; set; } = false;


        public async Task<bool> IsReady()
        {
            if (!BrowserExists)
                return false;

            if (!_serviceReady)
                if (await IsLogined())
                    _serviceReady = true;

            return _serviceReady;
        } 

        public BotBettingService(ISettingsService settingsService)
        {
            _api = new BetfairAPI();

            _settingsService = settingsService;
            _settings = _settingsService.Get();

            _data = _settings.BetBotRequestData;

            _api.InitializeRequestData(_settings.ParserBotRequestData);
            _api.SetupRequestManager(new HttpClientRequestManager());
        }



        public async Task SetupContext(IBrowserContext context)
        {
            _context = context;

            await context.RouteAsync("**/*", Handler);
            await context.Pages[0].ReloadAsync();

            BrowserExists = true;
        }

        public async Task Start()
        {
            if (Active)
                return;
           
            await WaitPageLoadedAndLogin();

            Active = true;
        }

        private async Task WaitPageLoadedAndLogin()
        {
            await WaitPageLoaded();
            await Login(_settings.BookmakerAccountData.Login, _settings.BookmakerAccountData.Password);
            await WaitPageLoaded();

            if (await IsLogined() == false)
                throw new Exception("wtf");
        }

        private async Task WaitPageLoaded()
        {
            while (await IsPageLoaded() == false)
                await Task.Delay(500);
        }

        private async Task<bool> IsPageLoaded()
        {
            return await AsyncFunction<bool>("IsPageLoaded()");
        }

        private async Task<bool> IsLogined()
        {
            return await AsyncFunction<bool>("IsLogined()");
        }

        private async Task Login(string username, string password)
        {
            await AsyncFunction($"Login('{username}', '{password}')");
        }

        private void Handler(IRoute route)
        {
            if (route.Request.Url.Contains("scan-inbf.betfair.com"))
            {
                string urlData = route.Request.Url.Split("_ak=")[1];
                string apiKey = urlData.Split("&")[0];

                _data.Key = apiKey;

                foreach (var header in route.Request.Headers)
                {
                    string key = header.Key;
                    if (!header.Key.Contains("sec-"))
                        key = header.Key.ToUpperFirstLetters();

                    _data.Headers[key] = header.Value;
                }
            }

            route.ContinueAsync();
        }

        public async Task<bool> TryPlaceBet(BetData betData)
        {
            return await _api.PlaceStake(betData);
        }

        private string FormatAsyncFunction(string function)
        {
            return $"async () => await {function}";
        }

        private async Task AsyncFunction(string function)
        {
            try
            {
                await _context.Pages[0].EvaluateAsync(FormatAsyncFunction(function));
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<T> AsyncFunction<T>(string function)
        {
            try
            {
                var result = await _context.Pages[0].EvaluateAsync<T>(FormatAsyncFunction(function));

                return result;
            }
            catch (Exception ex)
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
        }
    }
}
