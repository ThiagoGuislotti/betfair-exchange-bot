using BetfairBirzhaBot.Core;
using BetfairBirzhaBot.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using BetfairBirzhaBot.Base;

namespace BetfairBirzhaBot.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public IAsyncCommand StartCommand { get; set; }

        public BetfairBirzhaBotCore Core { get; }
        public StrategyManagerViewModel StrategyManagerViewModel { get; set; }

        public MainWindowViewModel()
        {
            Core = App.ServiceProvider.GetService<BetfairBirzhaBotCore>();
            StrategyManagerViewModel = App.ServiceProvider.GetService<StrategyManagerViewModel>();

            StartCommand = new AsyncCommand(Start);
        }

        private async Task Start()
        {
            Task.Run(async () => await Core.Start());    
        }
    }
}
