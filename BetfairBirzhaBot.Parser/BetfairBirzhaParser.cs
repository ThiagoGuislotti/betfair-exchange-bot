using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Common.Entities.MarketEntities;
using BetfairBirzhaBot.Common.Entities.ResponseEntities;
using BetfairBirzhaBot.Common.Helpers;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Parser.RequestManager;
using BetfairBirzhaBot.Parser.RequestManager.Managers;
using System;
using System.Diagnostics;
using System.Net;
using BetfairBirzhaBot.Converters;
namespace BetfairBirzhaBot.Parser
{
    public static class TeamsParserExtension
    {
        private static List<string> _separators = new List<string>
        {
            " v ",
            " @ ",
            " vs "
        };

        public static string[] GetTeams(this string text)
        {
            string sep = _separators.Find(s => text.Contains(text));
            if (sep is null)
                return null;
            string[] teams = text.Split(sep, StringSplitOptions.None);
            return teams;
        }
    }

    public class BetfairBirzhaParser
    {
        private readonly BetfairAPI _api;

        public BetfairBirzhaParser()
        {
            _api = new BetfairAPI();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        public void SendUserRequestData(UserRequestData data)
        {
            _api.InitializeRequestData(data);
        }

        public void SetupRequestManager(IRequestManager manager)
        {
            _api.SetupRequestManager(manager);
        }

        public async Task ParseAllEventIds()
        {
            var result = await _api.GetAllEvents();
        }


        private object _locker = new();

        public async Task<Game> Parse(string eventId)
        {
            var game = new Game(eventId);

            try
            {
                var marketsDataTask = _api.GetMarketData(eventId).ConfigureAwait(false);
                var eventTimelineTask = _api.GetEventTimeLine(eventId).ConfigureAwait(false);
                var statisticsTask = _api.GetAllGameStatistics(eventId).ConfigureAwait(false);

                var marketsData = await marketsDataTask;
                var eventTimeline = await eventTimelineTask;
                var statistics = await statisticsTask;

                game.Title = marketsData.Attachments.Events.FirstOrDefault().Value.Name;
                game.Url = $"https://www.betfair.com/exchange/plus/ru/%D1%84%D1%83%D1%82%D0%B1%D0%BE%D0%BB/%D0%BA%D1%83%D0%B1%D0%BE%D0%BA-%D0%B3%D0%B5%D1%80%D0%BC%D0%B0%D0%BD%D0%B8%D0%B8/waldhof-mannheim-%D1%85%D0%BE%D0%BB%D1%8C%D1%88%D1%82%D0%B0%D0%B9%D0%BD-%D0%BA%D0%B8%D0%BB%D1%8C-%D1%81%D1%82%D0%B0%D0%B2%D0%B8%D1%82%D1%8C-{eventId}";

                FillGameStaticData(game, statistics, eventTimeline);
                if (marketsData is null)
                {
                    Debug.WriteLine($"FAIL PARSE MARKET DATA");
                    return default;
                }

                var marketNodes = await GetMarketNodes(marketsData.Attachments.LiteMarkets).ConfigureAwait(false);
                FillGameMarketNodes(game, marketNodes);

                game.InPlayMatchStatus = eventTimeline?.InPlayMatchStatus;

                game.LastUpdated = DateTime.Now;
                return game;
            }
            catch
            {
                Debug.WriteLine("fail to parse game");
            }

            return default;
        }

        private void FillGameStaticData(Game game, StatisticsResponse response, EventTimeline timeline)
        {
            if (timeline != null && !string.IsNullOrEmpty(timeline.InPlayMatchStatus))
                game.Status = timeline.InPlayMatchStatus;

            if (response is not null)
            {
                var score = response.Scoreboard;

                game.League = response.Scoreboard.Competition;
                var teams = game.Title.GetTeams();
                string home = score.HomeTeamName ?? teams[0];
                string away = score.AwayTeamName ?? teams[1];




                game.Teams.Add(new Team(home, ETeamType.Home));
                game.Teams.Add(new Team(away, ETeamType.Away));



                game.Teams[0].Score = score.HomeGoals;
                game.Teams[1].Score = score.AwayGoals;
                game.Statistics = response;
            }


            try
            {
                if (timeline.InPlayMatchStatus is not null)
                    game.ElapsedMinutes = timeline.ElapsedRegularTime;

                DateTime ctime = DateTime.UtcNow;
                DateTime gtime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(response.Scoreboard.StartTime)).DateTime;

                var offset = ctime - gtime;

                game.ConcreteElapsedMinutes = (int)offset.TotalMinutes;
            }
            catch
            {

            }


        }

        private async Task<List<MarketNode>> GetMarketNodes(Dictionary<string, Market> liteMarkets)
        {
            List<MarketNode> marketNodes = new();

            if (liteMarkets.Count > 20)
            {
                try
                {
                    var arr = liteMarkets.Select(x => x.Key).ToArray();
                    var parts = arr.Split(20).ToList();

                    foreach (var part in parts)
                    {
                        var details = await _api.GetMarketsDetails(part).ConfigureAwait(false);
                        if (details != null && details.Event?.EventNode != null)
                            marketNodes.AddRange(details.Event.EventNode.MarketNodes);
                    }
                }
                catch
                {
                }


            }
            else
            {
                try
                {
                    var details = await _api.GetMarketsDetails(liteMarkets.Select(x => x.Key)).ConfigureAwait(false);
                    if (details is null)
                        return default;
                    marketNodes.AddRange(details.Event.EventNode.MarketNodes);
                }
                catch
                {

                }
            }

            return marketNodes;
        }

        private void FillGameMarketNodes(Game game, List<MarketNode> marketNodes)
        {
            foreach (var market in marketNodes)
            {
                try
                {
                    string marketName = market.Description.MarketName.Trim().ToLower();
                    var marketRunners = market.Runners.Where(x => x.Exchange.AvailableToBack is not null).ToList();
                    if (marketName == "ставки на результат")
                    {
                        if (marketRunners.Count == 3)
                        {
                            game.WinMarketsCurrent.Add(new WinMarket
                            {
                                Coefficient = marketRunners[0].Exchange.AvailableToBack[0].Price,
                                MarketId = market.MarketId,
                                Type = ETeamType.Home,
                                SelectionId = marketRunners[0].SelectionId.ToString(),
                            });
                            game.WinMarketsCurrent.Add(new WinMarket
                            {
                                Coefficient = marketRunners[1].Exchange.AvailableToBack[0].Price,
                                MarketId = market.MarketId,
                                Type = ETeamType.Away,
                                SelectionId = marketRunners[1].SelectionId.ToString(),
                            });
                            game.WinMarketsCurrent.Add(new WinMarket
                            {
                                Coefficient = marketRunners[2].Exchange.AvailableToBack[0].Price,
                                MarketId = market.MarketId,
                                Type = ETeamType.Draw,
                                SelectionId = marketRunners[2].SelectionId.ToString(),
                            });
                        }
                        else
                        {
                            foreach (var runner in marketRunners)
                            {
                                try
                                {
                                    if (runner.Description is null || runner.Exchange.AvailableToBack is null)
                                        continue;

                                    var team = game.Teams.Find(x => x.Name == runner.Description.RunnerName);
                                    ETeamType type = ETeamType.None;
                                    if (runner.Description.RunnerName == $"{game.Teams[0].Name}-{game.Teams[1].Name}" || runner.Description.RunnerName.ToLower() == "ничья")
                                        type = ETeamType.Draw;
                                    else
                                    {
                                        if (game.Teams is null || runner.Description is null)
                                            continue;

                                        type = team.Type;
                                    }



                                    game.WinMarketsCurrent.Add(new WinMarket
                                    {
                                        Coefficient = runner.Exchange.AvailableToBack[0].Price,
                                        MarketId = market.MarketId,
                                        Type = type,
                                        SelectionId = runner.SelectionId.ToString(),
                                    });
                                }
                                catch
                                {

                                }
                            }
                        }


                    }

                    if (marketName == "обе команды забьют?")
                    {
                        foreach (var runner in marketRunners)
                        {
                            EBothToScoreType type = EBothToScoreType.NONE;

                            if (runner.Description.RunnerName == "Да")
                                type = EBothToScoreType.Yes;
                            else
                                type = EBothToScoreType.No;

                            if (type == EBothToScoreType.NONE)
                                continue;

                            game.BothToScoreMarketsCurrent.Add(new BothToScoreMarket
                            {
                                Coefficient = runner.Exchange.AvailableToBack[0].Price,
                                MarketId = market.MarketId,
                                SelectionId = runner.SelectionId.ToString(),
                                Type = type,
                            });
                        }
                    }

                    if (marketName.Contains("голы первого тайма"))
                    {
                        double parameter = marketName.Split(' ').Last().ConvertToDouble();
                        var totalMarket = new TotalMarket();

                        totalMarket.Parameter = parameter;
                        totalMarket.MarketId = market.MarketId;
                        foreach (var runner in marketRunners)
                        {
                            string name = runner.Description.RunnerName.ToLower();
                            if (name.Contains("менее"))
                            {
                                totalMarket.Under.Coefficient = runner.Exchange.AvailableToBack.First().Price;
                                totalMarket.Under.SelectionId = runner.SelectionId.ToString();
                            }
                            if (name.Contains("более"))
                            {
                                totalMarket.Over.Coefficient = runner.Exchange.AvailableToBack.First().Price;
                                totalMarket.Over.SelectionId = runner.SelectionId.ToString();
                            }
                        }

                        game.TotalMarketsFirstHalfCurrent.Add(totalMarket);
                    }

                    if (marketName.Contains("голов") && marketName.Contains("более/менее"))
                    {
                        double parameter = marketName.Split(' ')[1].ConvertToDouble();
                        var totalMarket = new TotalMarket();

                        totalMarket.Parameter = parameter;
                        totalMarket.MarketId = market.MarketId;

                        foreach (var runner in marketRunners)
                        {

                            string name = runner.Description.RunnerName.ToLower();
                            if (name.Contains("менее"))
                            {
                                totalMarket.Under.Coefficient = runner.Exchange.AvailableToBack.First().Price;
                                totalMarket.Under.SelectionId = runner.SelectionId.ToString();
                            }
                            if (name.Contains("более"))
                            {
                                totalMarket.Over.Coefficient = runner.Exchange.AvailableToBack.First().Price;
                                totalMarket.Over.SelectionId = runner.SelectionId.ToString();
                            }
                        }

                        game.TotalMarketsCurrent.Add(totalMarket);
                    }

                    if (market.Description.MarketType == "CORRECT_SCORE")
                    {
                        foreach (var runner in marketRunners)
                        {
                            try
                            {
                                var scoreData = runner.Description.RunnerName.Split('-');
                                if (scoreData.Length != 2)
                                    continue;

                                int home = int.Parse(scoreData[0].Trim());
                                int away = int.Parse(scoreData[1].Trim());
                                double price = runner.Exchange.AvailableToBack.FirstOrDefault().Price;

                                game.CorrectScoreMarkets.Add(new CorrectScoreMarket
                                {
                                    Home = home,
                                    Away = away,
                                    Coefficient = price,
                                    ScoreText = runner.Description.RunnerName,
                                    SelectionId = runner.SelectionId.ToString(),
                                    MarketId = market.MarketId
                                });
                            }
                            catch
                            {

                            }
                        }
                    }

                    if (market.Description.MarketType == "DOUBLE_CHANCE")
                    {
                        foreach (var runner in marketRunners)
                        {
                            if (runner.Description.RunnerName == "Хозяева или Ничья")
                                game.DoubleChanceMarketsCurrent.Add(new DoubleChanceMarket
                                {
                                    Type = EDoubleChanceType.WinHomeOrDraw,
                                    Coefficient = runner.Exchange.AvailableToBack[0].Price,
                                    MarketId = market.MarketId,
                                    SelectionId = runner.SelectionId.ToString()
                                });

                            if (runner.Description.RunnerName == "Хозяева или Гости")
                                game.DoubleChanceMarketsCurrent.Add(new DoubleChanceMarket
                                {
                                    Type = EDoubleChanceType.WinHomeOrAway,
                                    Coefficient = runner.Exchange.AvailableToBack[0].Price,
                                    MarketId = market.MarketId,
                                    SelectionId = runner.SelectionId.ToString()
                                });

                            if (runner.Description.RunnerName == "Ничья или Гости")
                                game.DoubleChanceMarketsCurrent.Add(new DoubleChanceMarket
                                {
                                    Type = EDoubleChanceType.WinAwayOrDraw,
                                    Coefficient = runner.Exchange.AvailableToBack[0].Price,
                                    MarketId = market.MarketId,
                                    SelectionId = runner.SelectionId.ToString()
                                });
                        }

                    }
                }
                catch
                {

                }
            }
        }

    }
}