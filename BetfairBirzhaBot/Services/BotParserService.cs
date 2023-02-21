using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Common.Entities.MarketEntities;
using BetfairBirzhaBot.Common.Helpers;
using BetfairBirzhaBot.Core.Telegram;
using BetfairBirzhaBot.Models;
using BetfairBirzhaBot.Parser;
using BetfairBirzhaBot.Parser.RequestManager.Managers;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Settings;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Event = BetfairBirzhaBot.Parser.Event;

namespace BetfairBirzhaBot.Services
{
    public class BotParserService
    {
        private IBrowserContext _context;

        private CancellationTokenSource _tokenSource;

        private readonly BetfairBirzhaParser _parser;
        private readonly ISettingsService _settingsService;
        private readonly HttpClientRequestManager _reqManager;

        private readonly Dictionary<string, Task> _gameWorkers = new();

        private readonly List<string> _validUpdatedGames = new();
        private readonly LogService _logger;

        private string _templateUrl = "https://www.betfair.com/exchange/plus/ru/%D1%84%D1%83%D1%82%D0%B1%D0%BE%D0%BB/uefa-women-s-euro/%D0%B0%D0%BD%D0%B3%D0%BB%D0%B8%D1%8F-%D0%B6-%D0%B3%D0%B5%D1%80%D0%BC%D0%B0%D0%BD%D0%B8%D1%8F-%D0%B6-%D1%81%D1%82%D0%B0%D0%B2%D0%B8%D1%82%D1%8C-";

        private bool _serviceReady = false;

        private SessionSettings _settings;
        private UserRequestData _data;
        private BetfairAPI _api;
        private TelegramSygnalBot _tgBot;

        public bool BrowserExists { get; set; } = false;
        public bool Active { get; set; } = false;
        public double LastGameUpdateTime { get; set; }
        public int CountGamesUpdating { get; set; }

        public event Func<Game, Task> OnGameUpdated;



        private List<string> _gamesBlacklist = new();
        public List<Game> Games { get; set; } = new();




        private readonly ConcurrentDictionary<string, Game> _games = new();



        public BotParserService(ISettingsService settingsService, LogService logger, TelegramSygnalBot tgBot)
        {
            _reqManager = new HttpClientRequestManager();

            _api = new BetfairAPI();
            _parser = new BetfairBirzhaParser();

            _settingsService = settingsService;
            _settings = _settingsService.Get();
            _data = _settings.ParserBotRequestData;
            _logger = logger;

            _parser.SendUserRequestData(_data);
            _api.InitializeRequestData(_data);
            _parser.SetupRequestManager(_reqManager);
            _api.SetupRequestManager(_reqManager);

            _tgBot = tgBot;
        }

        public async Task SetupContext(IBrowserContext context)
        {
            _context = context;

            await context.RouteAsync("**/*", RequestHandler);
            await context.Pages[0].ReloadAsync();

            BrowserExists = true;
        }

        private void RequestHandler(IRoute route)
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

        public async Task StartParsing()
        {
            if (Active)
                return;

            await _data.WaitValidData();
            await WaitPageLoadedAndLogin();

            Active = true;

            _tokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(async () => await GamesParserWorker(), _tokenSource.Token);
        }

        public void StopParsing()
        {
            if (!Active)
                return;

            _tokenSource.Cancel();
            Task.WaitAll(_gameWorkers.Values.ToArray());
            _gameWorkers.Clear();

            Games.ForEach(g => g.IsUpdating = false);

            Active = false;
        }

        public async Task<bool> IsReady()
        {
            if (!_serviceReady)
                _serviceReady = await IsLogined();

            return _serviceReady;
        }

        private async Task GamesParserWorker()
        {

            while (_tokenSource.Token.IsCancellationRequested == false)
            {
                if (_tokenSource.Token.IsCancellationRequested)
                    return;

                await Updater();

                if (_tokenSource.Token.IsCancellationRequested)
                    return;
            }
        }
        private async Task WaitPageLoadedAndLogin()
        {
            if (string.IsNullOrEmpty(_settings.BookmakerAccountData.Login))
                return;

            await WaitPageLoaded();
            await Login(_settings.BookmakerAccountData.Login, _settings.BookmakerAccountData.Password);
            await Task.Delay(3 * 1000);
            await WaitPageLoaded();

            if (await IsLogined() == false)
                throw new Exception("wtf");
        }
        private async Task<bool> IsLogined()
        {
            return await AsyncFunction<bool>("IsLogined()");
        }

        private async Task Login(string username, string password)
        {
            await AsyncFunction($"Login('{username}', '{password}')");
        }
        private async Task Updater()
        {
            //Collect all urls
            //Add urls to game list if not exists
            //Process, and start workers for urls
            //In workers parsing data, updating for current game
            try
            {
                var events = await _api.GetAllEvents();

                //events = new List<Event>()
                //{
                //    new Event
                //    {
                //        EventId = "31630864"
                //    }
                //};

                ProcessEventsConcurrent(events);
                ProcessWorkersConcurrent();
                ProcessFinishedGames();

                AddNewLeagues();

                CountGamesUpdating = _games.Count;

                await Task.Delay(5 * 1000);
            }
            catch
            {
                
            }
        }

        private void ProcessFinishedGames()
        {
            foreach (var game in _games.Values)
                if (game.LastUpdated != DateTime.MinValue && (DateTime.Now - game.LastUpdated).TotalMinutes >= 5)
                    RemoveGameFromParsing(game.EventId);
        }
        private void AddNewLeagues()
        {
            var leagues = _games.Values.Select(g => g.League).Where(x=> !string.IsNullOrEmpty(x)).ToList();
            _settings.AddNewLeagues(leagues);

            //foreach (var game in _games.Values)
            //{
            //    if (string.IsNullOrEmpty(game.League))
            //        continue;


            //    bool added = false;
            //    _settings.Strategies.ForEach(s => {
            //        if (s.Leagues.Exists(l => l.Name == game.League))
            //            return;
            //        s.Leagues.Add(new LeagueModel(game.League));
            //        added = true;
            //        });

            //    if (added)
            //        _logger.Processing($"В настройки ЧС лиг добавлена лига : {game.League}");
            //}
            
            _settingsService.Save();
        }

        private async Task WaitPageLoaded()
        {
            while (true)
            {
                Debug.WriteLine("Wait page loading");
                bool loaded = await AsyncFunction<bool>("IsPageLoaded()");
                if (loaded)
                    break;

                await Task.Delay(2000);
            }
        }

        private async Task ProcessUrls(IEnumerable<string> urls)
        {
            foreach (var url in urls)
            {
                if (!Games.Exists(x => x.Url == url))
                {
                    var game = new Game(url);
                    if (_gamesBlacklist.Contains(game.EventId))
                        continue;

                    Games.Add(game);
                }
            }
        }

        private void ProcessEvents(IEnumerable<Event> events)
        {
            foreach (var e in events)
                if (Games.Count < _settings.MaxGamesToParsingCount)
                    if (!Games.Exists(x => x.EventId == e.EventId.ToString()))
                    {
                        var game = new Game(e.EventId.ToString(), e.Name);
                        if (_gamesBlacklist.Contains(game.EventId))
                            continue;

                        Games.Add(game);
                    }
        }
        private void ProcessEventsConcurrent(IEnumerable<Event> events)
        {
            foreach (var e in events)
            {
                if (_games.Count >= _settings.MaxGamesToParsingCount)
                    continue;

                if (_games.ContainsKey(e.EventId) || _gamesBlacklist.Contains(e.EventId))
                    continue;

                var game = new Game(e.EventId, e.Name);

                _games.AddOrUpdate(e.EventId, game, (oldkey, oldvalue) => game);
            }
        }


        private void ProcessWorkers()
        {
            foreach (var game in Games)
                if (game.IsUpdating == false)
                    _gameWorkers.Add(game.EventId, Task.Factory.StartNew(async () => await StartNewWorker(game), _tokenSource.Token));
        }

        private void ProcessWorkersConcurrent()
        {
            foreach (var e in _games)
            {
                if (e.Value.IsUpdating == false && _gameWorkers.Count < _settings.MaxGamesToParsingCount)
                {
                    var updaterTask = Task.Factory.StartNew(async () => await StartNewWorkerByEventId(e.Key), _tokenSource.Token);

                    _gameWorkers.Add(e.Value.EventId, updaterTask);
                }
            }
        }

        private async Task StartNewWorkerByEventId(string eventId)
        {
            var game = _games[eventId];

            game.IsUpdating = true;

            while (game.IsUpdating && !_tokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var sw = Stopwatch.StartNew();

                    var updated = await _parser.Parse(eventId).ConfigureAwait(false);

                    await ProcessGameTime(updated);
                    
                    if (updated != default && updated.IsActive())
                    {
                        ProcessGameUpdate(game, updated);
                        OnGameUpdated(game);

                        LastGameUpdateTime = Math.Round(sw.Elapsed.TotalSeconds, 2);
                    }

                    if (IsNeedToBlacklist(game))
                    {
                        RemoveGameFromParsing(eventId);
                        return;
                    }

                    if (_tokenSource.Token.IsCancellationRequested)
                        return;

                    await Task.Delay(7 * 1000, _tokenSource.Token);
                }
                catch
                {
                    Debug.WriteLine("Failed to update the game");
                }
            }
        }
        private async Task StartNewWorker(Game game)
        {
            game.IsUpdating = true;
            while (game.IsUpdating)
            {
                try
                {
                    if (_tokenSource.Token.IsCancellationRequested)
                        return;

                    var sw = Stopwatch.StartNew();

                    var updated = await _parser.Parse(game.EventId).ConfigureAwait(false);
                    if (updated is not null && updated.IsActive())
                    {
                        if (updated.ElapsedMinutes >= 90)
                        {
                            _logger.Processing($"Игра закончилась, записываем счёт");

                            RemoveGameFromParsing(game.EventId);
                        }

                        if (updated.ConcreteElapsedMinutes < 0)
                        {
                            _logger.Processing($"Парсинг игры {updated.Title} начнется через + {Math.Abs(updated.ConcreteElapsedMinutes) - 5} минут. ");
                            await Delay(Math.Abs(updated.ConcreteElapsedMinutes + 5));
                        }


                        ProcessGameUpdate(game, updated);
                        OnGameUpdated(game);

                        if (_validUpdatedGames.Contains(game.EventId) == false)
                        {
                            _validUpdatedGames.Add(game.EventId);
                            _logger.Processing($"Игра {game.Title} добавлена в парсинг. Интервал обновления кефов : 5c.");
                        }

                        double elapsedSeconds = sw.Elapsed.TotalSeconds;

                        LastGameUpdateTime = Math.Round(elapsedSeconds, 2);
                    }

                    if (IsNeedToBlacklist(game))
                    {
                        RemoveGameFromParsing(game.EventId);
                        return;
                    }



                    if (_tokenSource.Token.IsCancellationRequested)
                        return;

                    await Task.Delay(7 * 1000);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to update the game");
                }
            }
        }
        private async Task Delay(int minutes)
        {
            var sw = Stopwatch.StartNew();
            while ((int)sw.Elapsed.TotalMinutes <= minutes)
            {
                await Task.Delay(30 * 1000);
                Debug.WriteLine($"Delay. {minutes - (int)sw.Elapsed.TotalMinutes} minutes left.");
            }
        }
        private void RemoveGameFromParsing(string eventId)
        {
            var game = _games[eventId];

            game.IsUpdating = false;
            _gamesBlacklist.Add(eventId);

            Games.Remove(game);
            _gameWorkers.Remove(eventId);
            _games.Remove(eventId, out game);
        }

        private async Task ProcessGameTime(Game updated)
        {
            if (updated.ElapsedMinutes >= 90 || updated.Status == "Finished")
            {
                _logger.Processing($"Игра {updated.EventId} закончилась, записываем счёт");
                RemoveGameFromParsing(updated.EventId);
            }

            if (updated.ConcreteElapsedMinutes < 0)
            {
                _logger.Processing($"Парсинг игры {updated.EventId} начнется через + {Math.Abs(updated.ConcreteElapsedMinutes) - 1} минут. ");
                await Task.Delay((Math.Abs(updated.ConcreteElapsedMinutes - 1) * 1000), _tokenSource.Token);
            }
        }

        private bool IsNeedToBlacklist(Game game)
        {
            if (game.Url.Split("/").Length != 9)
                return true;

            if (game.EventId.Length < 3)
                return true;

            if (_settings.Leagues.Exists(l => l.Name == game.League && l.IncludeToBlacklist))
            {
                _logger.Error($"Прекращён парсинг игры {game.Title}, лига игры в ЧС");
                return true;
            }

            return false;
        }

        private void ProcessUpdateScoreInSygnal(Game updated)
        {
            var existingSygnals = _settings.SygnalStore.Where(s => s.Game.EventId == updated.EventId && s.Game.Total < updated.Total).ToList();
            foreach (var sygnal in existingSygnals)
            {
                //sygnal.Game.Statistics = updated.Statistics;
                sygnal.Game.LastHomeScore = updated.Teams[0].Score;
                sygnal.Game.LastAwayScore = updated.Teams[1].Score;
                sygnal.Game.LastScoreUpdatedMinute = updated.ElapsedMinutes;
                if (updated.ElapsedMinutes <= 45)
                {
                    sygnal.Game.FirstHalfHomeScore = updated.Teams[0].Score;
                    sygnal.Game.FirstHalfAwayScore = updated.Teams[1].Score;
                }
                Task.Run(async() => await _tgBot.EditScoreMessages(sygnal));
                //await _tgBot.SendAll($"\nID Игры #GAME_{updated.EventId}\n\nВ сигнале по стратегии {sygnal.StrategyName} изменился счёт.\nИгра : {sygnal.Game.Title}\nМинута : {sygnal.Game.ElapsedMinutes}\nБыло {sygnal.Game.Teams[0].Score} - {sygnal.Game.Teams[1].Score}\nСтало {sygnal.Game.LastHomeScore} - {sygnal.Game.LastAwayScore}");
            }

            _settingsService.Save();
        }

        private void ProcessGameUpdate(Game c, Game u)
        {
            try
            {
                c.Title = u.Title;
                c.Url = u.Url;
                c.LastUpdated = u.LastUpdated;

                if (u.Total > c.Total)
                    ProcessUpdateScoreInSygnal(u);

                c.Teams = new List<Team>(u.Teams);

                if (u.ElapsedMinutes != -1 && u.ElapsedMinutes > c.ElapsedMinutes)
                {
                    _logger.Info($"Время игры {u.GetTitle()} обновилось. [{c.ElapsedMinutes} -> {u.ElapsedMinutes}]");
                    c.ElapsedMinutes = u.ElapsedMinutes;
                }

                if (u.League != null && string.IsNullOrEmpty(c.League))
                    c.League = u.League;

                if (u.Statistics.IsValid())
                    c.Statistics = u.Statistics;

                if (u.WinMarketsCurrent.IsValid())
                    c.WinMarketsCurrent = new List<WinMarket>(u.WinMarketsCurrent);

                if (u.BothToScoreMarketsCurrent.IsValid())
                    c.BothToScoreMarketsCurrent = new List<BothToScoreMarket>(u.BothToScoreMarketsCurrent);
                
                c.TotalMarketsCurrent = new List<TotalMarket>(u.TotalMarketsCurrent);
                c.TotalMarketsFirstHalfCurrent = new List<TotalMarket>(u.TotalMarketsFirstHalfCurrent);
                c.CorrectScoreMarkets = new List<CorrectScoreMarket>(u.CorrectScoreMarkets);
                c.DoubleChanceMarketsCurrent = new List<DoubleChanceMarket>(u.DoubleChanceMarketsCurrent);


                if (u.ElapsedMinutes == 0)
                {
                    string title = u.GetTitle();
                    if (c.TotalMarketsFirstHalfStart.Count == 0)
                        if (u.TotalMarketsFirstHalfCurrent.Count != 0)
                        {
                            _logger.Info($"[{title}] Записали тоталы 1 тайм прематч ");
                            
                            c.TotalMarketsFirstHalfStart = new List<TotalMarket>(u.TotalMarketsFirstHalfCurrent);
                        }

                    if (u.TotalMarketsCurrent.Count != 0 && c.TotalMarketsStart.Count == 0)
                    {
                        _logger.Info($"[{title}] Записали тоталы прематч ");
                        c.TotalMarketsStart = new List<TotalMarket>(u.TotalMarketsCurrent);
                    }

                    if (u.BothToScoreMarketsCurrent.IsValid())
                        if (u.BothToScoreMarketsCurrent.Count != 0 && c.BothToScoreMarketsStart.Count == 0)
                        {
                            _logger.Info($"[{title}] Записали обе забьют прематч ");
                            c.BothToScoreMarketsStart = new List<BothToScoreMarket>(u.BothToScoreMarketsCurrent);

                        }

                    if(u.WinMarketsCurrent.IsValid())
                        if (u.WinMarketsCurrent.Count != 0 && c.WinMarketsStartGame.Count == 0)
                        {
                            _logger.Info($"[{title}] Записали 1х2 прематч ");
                            c.WinMarketsStartGame = new List<WinMarket>(u.WinMarketsCurrent);

                        }
                    if (u.DoubleChanceMarketsCurrent.Count != 0 && c.DoubleChanceMarketsStart.Count == 0)
                    {
                        _logger.Info($"[{title}] Записали двойной шанс прематч ");
                        c.DoubleChanceMarketsStart = new List<DoubleChanceMarket>(u.DoubleChanceMarketsCurrent);
                    }

                    if (u.CorrectScoreMarkets.Count != 0 && c.CorrectScoreMarketsPrematch.Count == 0)
                    {
                        _logger.Info($"[{title}] Записали точный счёт прематч");
                        c.CorrectScoreMarketsPrematch = new List<CorrectScoreMarket>(u.CorrectScoreMarkets);
                    }
                }

                if (u.ElapsedMinutes == 45 && u.InPlayMatchStatus == "FirstHalfEnd")
                {
                    c.FirstHalfHomeScore = u.Teams[0].Score;
                    c.FirstHalfAwayScore = u.Teams[1].Score;

                    ProcesUpdateScoreFirstHalf(u);
                }

            }
            catch
            {
                Debug.WriteLine("Fail to update game data");
            }
        }

        private void ProcesUpdateScoreFirstHalf(Game updated)
        {
            var existingSygnals = _settings.SygnalStore.Where(s => s.Game.EventId == updated.EventId).ToList();
            foreach (var sygnal in existingSygnals)
            {
                sygnal.Game.FirstHalfHomeScore = updated.Teams[0].Score;
                sygnal.Game.FirstHalfAwayScore = updated.Teams[1].Score;

                Task.Run(async () => await _tgBot.EditScoreMessages(sygnal));
                //await _tgBot.SendAll($"\nID Игры #GAME_{updated.EventId}\n\nВ сигнале по стратегии {sygnal.StrategyName} изменился счёт.\nИгра : {sygnal.Game.Title}\nМинута : {sygnal.Game.ElapsedMinutes}\nБыло {sygnal.Game.Teams[0].Score} - {sygnal.Game.Teams[1].Score}\nСтало {sygnal.Game.LastHomeScore} - {sygnal.Game.LastAwayScore}");
            }

            _settingsService.Save();
        }


        private async Task<List<string>> CollectAllUrls()
        {
            var resultList = new List<string>();

            string json = await AsyncFunction<string>("CollectAllUrls('футбол')");
            var result = JsonConvert.DeserializeObject<List<string>>(json);

            if (result is null)
                return resultList;

            return result;
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

    public static class MarketsValidator
    {
        public static bool IsValid(this List<WinMarket> markets)
        {
            var sum = markets.Sum(x => x.Coefficient - 1);
            return sum > 1;
        }

        public static bool IsValid(this List<BothToScoreMarket> markets)
        {
            return markets.Sum(x => x.Coefficient - 1) > 1;
        }
    }
}
