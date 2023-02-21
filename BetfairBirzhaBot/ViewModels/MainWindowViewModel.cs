using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public IAsyncCommand NavigateToMainCommand { get; set; }
        public IAsyncCommand NavigateToStrategyManagerCommand { get; set; }
        public IAsyncCommand NavigateToLeagueBlackListCommand { get; set; }
        public IAsyncCommand NavigateToSygnalStoreCommand { get; set; }
        public IAsyncCommand NavigateToTelegramSettingsCommand { get; set; }
        public IAsyncCommand NavigateToGameInplayViewerCommand { get; set; }

        public BaseViewModel CurrentViewModel { get; set; }

        public int ProgramMemoryUsageMb { get; set; }
        public int GamesInParsingCount { get; set; }
        public double AverageGameParsingTime { get; set; }

        private readonly BotParserService _parser;

        public MainWindowViewModel()
        {
            _parser = App.ServiceProvider.GetService<BotParserService>();
            CurrentViewModel = App.ServiceProvider.GetService<BotControlPanelViewModel>();

            NavigateToMainCommand = new AsyncCommand(() => Navigate<BotControlPanelViewModel>());
            NavigateToStrategyManagerCommand = new AsyncCommand(() => Navigate<StrategyManagerViewModel>());
            NavigateToLeagueBlackListCommand = new AsyncCommand(() => Navigate<LeaguesBlackListViewModel>());
            NavigateToSygnalStoreCommand = new AsyncCommand(() => Navigate<SygnalsStoreViewModel>());
            NavigateToTelegramSettingsCommand = new AsyncCommand(() => Navigate<TelegramSettingsViewModel>());
            NavigateToGameInplayViewerCommand = new AsyncCommand(() => Navigate<GameInplayViewerViewModel>());

            Task.Run(async () => await DebugInfoUpdater());
        }

        private async Task DebugInfoUpdater()
        {
            File.WriteAllText("memlogs.txt", "");
            while (true)
            {
                var currentProc = Process.GetCurrentProcess();
                var memoryUsed = currentProc.PrivateMemorySize64 / 1024 / 1024;

                ProgramMemoryUsageMb = (int)memoryUsed;
                GamesInParsingCount = _parser.CountGamesUpdating;
                AverageGameParsingTime = _parser.LastGameUpdateTime;

                OnPropertyChanged(nameof(ProgramMemoryUsageMb));
                OnPropertyChanged(nameof(GamesInParsingCount));
                OnPropertyChanged(nameof(AverageGameParsingTime));


                File.AppendAllText("memlogs.txt", $"[ {DateTime.Now.ToLongTimeString} ] MEMORY USAGE : {ProgramMemoryUsageMb} MB\n");
                
                await Task.Delay(1 * 1000);
            }
        }

        private async Task Navigate<T>() where T : BaseViewModel
        {
            var vm = App.ServiceProvider.GetRequiredService<T>();

            CurrentViewModel = vm;
            OnPropertyChanged(nameof(CurrentViewModel));
        }

    }
}
