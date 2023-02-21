using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Core;
using BetfairBirzhaBot.Models;
using BetfairBirzhaBot.Services;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Settings;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BetfairBirzhaBot.ViewModels
{
    public class BotControlPanelViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly SessionSettings _settings;
        private readonly BetfairBirzhaBotCore _botCore;
        private readonly LogService _logger;

        private Stopwatch _stopwatch = new();
        private DispatcherTimer _timer = new();

        public string SessionTimeElapsed { get; set; }
        private int _maxGamesToParsingCount { get; set; }
        public int MaxGamesToParsingCount
        {
            get => _maxGamesToParsingCount;
            set
            {
                _maxGamesToParsingCount = value;

                _settings.MaxGamesToParsingCount = value;

                OnPropertyChanged(nameof(MaxGamesToParsingCount));
            }
        }

        public IAsyncCommand StartCommand { get; set; }
        public IAsyncCommand StopCommand { get; set; }

        public string Title { get; set; }
        public string Status { get; set; } = "Готово к запуску";
       
        public int CountParsingGames { get; set; }
        public int CountGamesWithStatistics { get; set; }


        public string BookmakerUsername { get; set; }
        public string BookmakerPassword { get; set; }

        private ObservableCollection<LogItemModel> _logList;
        public ObservableCollection<LogItemModel> LogList
        {
            get
            {
                if (_logList == null)
                    _logList = new ObservableCollection<LogItemModel>();
                return _logList;
            }
            set
            {
                if (value == _logList)
                    return;

                _logList = value;

                OnPropertyChanged(nameof(LogList));
            }
        }

        public BotControlPanelViewModel(ISettingsService settingsService, BetfairBirzhaBotCore botCore, LogService logger)
        {
            _settingsService = settingsService;
            _botCore = botCore;
            _logger = logger;

            _logger.OnLog += _logger_OnLog;

            Title = "Панель управления ботом";

            StartCommand = new AsyncCommand(Start);
            StopCommand = new AsyncCommand(Stop);

            _settings = _settingsService.Get();

            DisplaySettings();
            SetupTimer();
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval += new TimeSpan(0, 0, 1);
        }

        private void _timer_Tick(object? sender, System.EventArgs e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                SessionTimeElapsed = $"{_stopwatch.Elapsed.Hours}:{_stopwatch.Elapsed.Minutes}:{_stopwatch.Elapsed.Seconds}";
                OnPropertyChanged(nameof(SessionTimeElapsed));
            });

        }

        private void _logger_OnLog(LogItemModel obj)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                LogList.Insert(0, obj);

                if (LogList.Count >= 100)
                    LogList.RemoveAt(LogList.Count - 1);

                OnPropertyChanged(nameof(LogList));
            });
        }

        private void DisplaySettings()
        {
            BookmakerUsername = _settings.BookmakerAccountData.Login;
            BookmakerPassword = _settings.BookmakerAccountData.Password;
            MaxGamesToParsingCount = _settings.MaxGamesToParsingCount;


            _logger.Info("Программа запущена и готова к работе.");
        }

        private async Task Start()
        {
            TimerStart();

            _settings.BookmakerAccountData.Login = BookmakerUsername;
            _settings.BookmakerAccountData.Password = BookmakerPassword;
            _settingsService.Save();

            await _botCore.Start();


            Status = "парсер запущен";
            OnPropertyChanged(nameof(Status));
        }

        private async Task Stop()
        {
            TimerStop();

            await _botCore.Stop();
            _settingsService.Save();
            Status = "парсер остановлен";
            OnPropertyChanged(nameof(Status));
        }

        private void TimerStart()
        {
            _stopwatch.Start();
            _timer.Start();
        }

        private void TimerStop()
        {
            _stopwatch.Stop();
            _timer.Stop();
        }
    }
}
