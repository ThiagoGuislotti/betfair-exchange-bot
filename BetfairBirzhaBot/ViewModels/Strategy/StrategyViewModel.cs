using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Settings;
using BetfairBirzhaBot.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.ViewModels
{
    public class StrategyViewModel : BaseViewModel
    {
        private readonly FilterFactory _filterFactory;

        public Strategy Strategy { get; set; }
        public EMarket ChoosenFilterType { get; set; }
        public string ChoosenFilterName { get; set; }
        public ObservableCollection<EMarket> FilterTypes { get; set; }

        public StrategyViewModel(Strategy strategy)
        {
            _filterFactory = App.ServiceProvider.GetService<FilterFactory>();

            Strategy = strategy;
            FilterTypes = new ObservableCollection<EMarket>(EnumUtility.GetValues<EMarket>());
        }

        public IAsyncCommand DeleteCommand { get; set; }
        public IAsyncCommand CreateFilterCommand { get; set; }
        
        private async Task Delete()
        {
            //Скорее всего что то типо такого будет, но не тестил
            //
            //var settingsService = App.ServiceProvider.GetService<ISettingsService>();
            //var settings = settingsService.Get();

            //var currentStrategy = settings.Strategies.Find(x => x.Id == Strategy.Id);
            //settings.Strategies.Remove(currentStrategy);
        }

        private async Task CreateFilter()
        {
            Strategy.Filters.Add(_filterFactory.CreateDefaultFilter(ChoosenFilterType, ChoosenFilterName));
        }
    }
}
