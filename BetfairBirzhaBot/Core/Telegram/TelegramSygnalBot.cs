using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.IO;
using Newtonsoft.Json;
using File = System.IO.File;
using BetfairBirzhaBot.Settings;
using System.Linq;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Converters;

namespace BetfairBirzhaBot.Core.Telegram
{
    public static class TelegramMarkdownHelper
    {
        public static string Bold(this string text)
        {
            return $"<b>{text}</b>";
        }
    }
    public class TelegramSygnalBot
    {
        private TelegramBotClient _client;
        private CancellationTokenSource _tokenSource;
        private readonly Dictionary<string, List<Message>> _sygnalMessages = new();
        private string _chatsStorageName = "telegram-chats.json";
        private List<string> _chats;
        ISettingsService _service;
        SessionSettings _settings;



        public TelegramSygnalBot(ISettingsService service)
        {
            var chats = LoadAllChats();
            if (chats is null || chats.Count == 0)
                _chats = new List<string>();
            else
                _chats = chats.ToList();
            SaveChats();
            _service = service;
            _settings = service.Get();
        }

        public void Initialize(string key, List<string> strategyNames)
        {
            if (key == null)
                return;
            _client = new TelegramBotClient(key);
            _tokenSource = new CancellationTokenSource();
            //_chats = new List<string>();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            _client.StartReceiving(updateHandler: HandleUpdateAsync,
                                    pollingErrorHandler: HandlePollingErrorAsync,
                                    receiverOptions: receiverOptions,
                                    cancellationToken: _tokenSource.Token);

            SendAll($"Готов высылать сигналы.\n<b>Активные стратегии</b>\n{string.Join('\n', strategyNames)}");
        }

        private async Task<List<string>> GenerateMessage(StrategySygnalResult sygnal, bool isUpdatingScore = false)
        {
            var lines = new List<string>();
            try
            {
                string strategyName = _settings.Strategies.Find(x => x.Id == sygnal.StrategyId)?.Name;
                var tgSettings = _settings.TelegramSettings.MessageSettings;
                sygnal.Game.TotalMarketsCurrent.Sort((a, b) => a.Parameter.CompareTo(b.Parameter));
                sygnal.Game.TotalMarketsFirstHalfCurrent.Sort((a, b) => a.Parameter.CompareTo(b.Parameter));
                sygnal.Game.TotalMarketsFirstHalfStart.Sort((a, b) => a.Parameter.CompareTo(b.Parameter));
                sygnal.Game.TotalMarketsStart.Sort((a, b) => a.Parameter.CompareTo(b.Parameter));

                lines.Add("✅ Сигнал");
                lines.Add("");
                lines.Add($"Стратегия : {strategyName}".Bold());
                lines.Add($"Лига : {sygnal.Game.League}");
                lines.Add($"Игра : {sygnal.Game.GetTitle()}");
                lines.Add($"Минута : {sygnal.Game.ElapsedMinutes}");
                
                lines.Add($"");
                lines.Add($"Счёт : [ {sygnal.Game.Teams[0].Score} - {sygnal.Game.Teams[1].Score} ]".Bold());
                if (isUpdatingScore)
                    lines.Add($"🔄 [ {sygnal.Game.LastScoreUpdatedMinute} м. ] Обновленный счёт : [ {sygnal.Game.LastHomeScore} - {sygnal.Game.LastAwayScore}]");
                lines.Add($"");


                lines.Add($"");
                lines.Add($"<a href='{sygnal.Game.Url}'>Ссылка на игру</a>".Bold());
                lines.Add($"");

                if (tgSettings.WinResultsStart && sygnal.Game.WinMarketsStartGame.Count > 0)
                {
                    var home = sygnal.Game.WinMarketsStartGame.Find(x => x.Type == Common.Entities.ETeamType.Home) ?? new WinMarket();
                    var away = sygnal.Game.WinMarketsStartGame.Find(x => x.Type == Common.Entities.ETeamType.Away) ?? new WinMarket();
                    var draw = sygnal.Game.WinMarketsStartGame.Find(x => x.Type == Common.Entities.ETeamType.Draw) ?? new WinMarket();
                    lines.Add("1х2 начало".Bold());
                    lines.Add("1             Х            2");
                    lines.Add($"{home?.Coefficient}        {draw?.Coefficient}        {away?.Coefficient}".Bold());
                    lines.Add($"");
                }
                if (tgSettings.WinResultsLive && sygnal.Game.WinMarketsCurrent.Count > 0)
                {
                    var home = sygnal.Game.WinMarketsCurrent.Find(x => x.Type == Common.Entities.ETeamType.Home) ?? new WinMarket();
                    var away = sygnal.Game.WinMarketsCurrent.Find(x => x.Type == Common.Entities.ETeamType.Away) ?? new WinMarket();
                    var draw = sygnal.Game.WinMarketsCurrent.Find(x => x.Type == Common.Entities.ETeamType.Draw) ?? new WinMarket();
                    lines.Add("1х2 лайв".Bold());
                    lines.Add("1             Х            2");
                    lines.Add($"{home?.Coefficient}        {draw?.Coefficient}        {away?.Coefficient}".Bold());
                    lines.Add($"");
                }

                if (tgSettings.TotalsStartFirstHalf && sygnal.Game.TotalMarketsFirstHalfStart.Count > 0)
                {
                    var filterData = tgSettings.TotalsStartFirstHalfDiapason
                        .Split('-')
                        .Select(x => x.ConvertToDouble())
                        .ToList();

                    lines.Add($"Тоталы начало, 1 тайм".Bold());
                    foreach (var total in sygnal.Game.TotalMarketsFirstHalfStart.Where(x => x.Parameter >= filterData[0] && x.Parameter <= filterData[1]))
                        lines.Add($"ТБ {total.Parameter}   {total.Over.Coefficient}     |      ТМ {total.Parameter}   {total.Under.Coefficient}");
                    lines.Add($"");
                }


                if (tgSettings.TotalsStartFullGame && sygnal.Game.TotalMarketsStart.Count > 0)
                {
                    var filterData = tgSettings.TotalsStartFullDiapason
                        .Split('-')
                        .Select(x => x.ConvertToDouble())
                        .ToList();
                    lines.Add($"Тоталы начало, вся игра".Bold());
                    foreach (var total in sygnal.Game.TotalMarketsStart.Where(x => x.Parameter >= filterData[0] && x.Parameter <= filterData[1]))
                        lines.Add($"ТБ {total.Parameter}   {total.Over.Coefficient}     |      ТМ {total.Parameter}   {total.Under.Coefficient}");
                    lines.Add($"");
                }


                if (tgSettings.TotalsFirstHalfLive && sygnal.Game.TotalMarketsFirstHalfCurrent.Count > 0 && sygnal.Game.ElapsedMinutes <= 45)
                {
                    var filterData = tgSettings.TotalsFirstHalfLiveDiapason
                       .Split('-')
                       .Select(x => x.ConvertToDouble())
                       .ToList();
                    lines.Add($"Тоталы лайв, 1 тайм".Bold());
                    foreach (var total in sygnal.Game.TotalMarketsFirstHalfCurrent.Where(x => x.Parameter >= filterData[0] && x.Parameter <= filterData[1]))
                        lines.Add($"ТБ {total.Parameter}   {total.Over.Coefficient}     |      ТМ {total.Parameter}   {total.Under.Coefficient}");
                    lines.Add($"");
                }


                if (tgSettings.TotalsFullGameLive && sygnal.Game.TotalMarketsCurrent.Count > 0)
                {
                    var filterData = tgSettings.TotalsFullGameLiveDiapason
                       .Split('-')
                       .Select(x => x.ConvertToDouble())
                       .ToList();
                    lines.Add($"Тоталы лайв, вся игра".Bold());
                    foreach (var total in sygnal.Game.TotalMarketsCurrent.Where(x => x.Parameter >= filterData[0] && x.Parameter <= filterData[1]))
                        lines.Add($"ТБ {total.Parameter}   {total.Over.Coefficient}     |      ТМ {total.Parameter}   {total.Under.Coefficient}");
                    lines.Add($"");
                }


                if (tgSettings.BothToScoreStart && sygnal.Game.BothToScoreMarketsStart.Count > 0)
                {
                    lines.Add($"Обе забьют, начало".Bold());
                    lines.Add($"ДА          НЕТ");
                    lines.Add($"{sygnal.Game.BothToScoreMarketsStart.Find(x => x.Type == Common.Entities.EBothToScoreType.Yes)?.Coefficient}          {sygnal.Game.BothToScoreMarketsStart.Find(x => x.Type == Common.Entities.EBothToScoreType.No)?.Coefficient}");

                    lines.Add($"");
                }

                if (tgSettings.BothToScoreLive &&
                    sygnal.Game.BothToScoreMarketsCurrent.Count > 0 &&
                    (sygnal.Game.Teams[0].Score == 0 || sygnal.Game.Teams[1].Score == 0))
                {
                    lines.Add($"Обе забьют, лайв".Bold());
                    lines.Add($"ДА          НЕТ".Bold());
                    lines.Add($"{sygnal.Game.BothToScoreMarketsCurrent.Find(x => x.Type == Common.Entities.EBothToScoreType.Yes)?.Coefficient}          {sygnal.Game.BothToScoreMarketsCurrent.Find(x => x.Type == Common.Entities.EBothToScoreType.No)?.Coefficient}");

                }

                lines.Add($"");

                if (sygnal.Game.Statistics is not null && sygnal.Game.Statistics.IsValid())
                {
                    var stats = sygnal.Game.Statistics.Statistics;

                    lines.Add($"Атаки : {stats.HomeAttacks} | {stats.AwayAttacks}");
                    lines.Add($"Опасные атаки : {stats.HomeDangerousAttacks} | {stats.AwayDangerousAttacks}");
                    lines.Add($"Удары в створ : {stats.HomeShotsOnTarget} | {stats.AwayShotsOnTarget}");
                    lines.Add($"Удары в сторону : {stats.HomeShotsOffTarget} | {stats.AwayShotsOffTarget}");
                    lines.Add($"Желтые карточки : {stats.HomeYellowCards} | {stats.AwayYellowCards}");
                    lines.Add($"Красные карточки : {stats.HomeRedCards} | {stats.AwayRedCards}");
                    lines.Add($"Угловые : {stats.HomeCorners} | {stats.AwayCorners}");
                }
            }
            catch (Exception ex)
            {
                lines.Add("");
                lines.Add($"ОШИБКА, ПЕРЕШЛИТЕ ПРОГРАММИСТУ".Bold());
                lines.Add($"{ex.ToString()}");
            }

            lines.Add("");
            lines.Add($"ID Игры #GAME_{sygnal.Game.EventId}".Bold());
            lines.Add("");

            return lines;
        }

        public async Task SendSygnalInfo(StrategySygnalResult sygnal, Strategy strategy)
        {
            var messageLines = await GenerateMessage(sygnal);
            var messages = await SendAll(messageLines);
            _sygnalMessages.Add(sygnal.Id, messages);
        }

        public async Task SendBetErrorInfo(string errorCode)
        {
            var lines = new List<string>();
            lines.Add("Не смогли поставить ставку");
            lines.Add("");
            lines.Add(errorCode);

            await SendAll(lines);
        }


        public async Task SendBetAcceptedInfo(BetModel bet, Common.Entities.Game game)
        {
            var lines = new List<string>();
            lines.Add("✅ Ставка успешно поставилась");
            lines.Add("");

            lines.Add($"Игра : {game.GetTitle()}");
            lines.Add($"Минута : {game.ElapsedMinutes}");

            string betInfoText = $"Ставим на {bet.Market.GetMarketDescription()} ";

            if (bet.Market == EMarket.Total)
                betInfoText += bet.TotalType.GetMarketDescription() + " " + bet.TotalParameter;
            if (bet.Market == EMarket.CorrectScore)
                betInfoText += $" [ {bet.Home} - {bet.Away}] ";
            lines.Add($"Маркет : {betInfoText}");
            lines.Add($"Сумма : {bet.Stake}");

            await SendAll(lines);
        }

        public async Task EditScoreMessages(StrategySygnalResult sygnal)
        {
            if (_sygnalMessages.ContainsKey(sygnal.Id))
            {
                foreach (var message in _sygnalMessages[sygnal.Id])
                {
                    var newMessageLines = await GenerateMessage(sygnal, true);
                    string messageText = string.Join("\n", newMessageLines);
                    await _client.EditMessageTextAsync(message.Chat.Id, message.MessageId, messageText, ParseMode.Html);
                }
            }
        }

        public async Task<List<Message>> SendAll(List<string> lines)
        {
            var result = new List<Message>();
            string text = string.Join("\n", lines);
            foreach (var chat in _chats)
            {

                try
                {
                    result.Add(await _client.SendTextMessageAsync(chat, text, ParseMode.Html, disableWebPagePreview: true));
                }
                catch (Exception ex)
                {

                }
            }

            return result;
        }
        public async Task SendAll(string text)
        {
            foreach (var chat in _chats)
            {
                try
                {
                    await _client.SendTextMessageAsync(chat, text, ParseMode.Html, disableWebPagePreview: true);
                }
                catch (Exception ex)
                {

                }
            }
        }


        public void SaveChats()
        {
            File.WriteAllText(_chatsStorageName, JsonConvert.SerializeObject(_chats));
        }

        private List<string> LoadAllChats()
        {
            if (File.Exists(_chatsStorageName) is false)
                File.AppendAllText(_chatsStorageName, "");

            string chats = File.ReadAllText(_chatsStorageName);

            return JsonConvert.DeserializeObject<List<string>>(chats);
        }








        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message.Text.Trim() == "/start")
            {
                var chat = update.Message.Chat;
                if (_chats.Exists(chatId => chatId == chat.Id.ToString()) is false)
                {
                    _chats.Add(chat.Id.ToString());
                    SaveChats();
                    await _client.SendTextMessageAsync(chat.Id, "Теперь вы будете получать сигналы.");
                }

            }
        }
        private async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return;
        }
    }
}
