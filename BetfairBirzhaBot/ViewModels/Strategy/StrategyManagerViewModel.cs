using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Core;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Settings;
using BetfairBirzhaBot.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.ViewModels
{
    public class StrategyManagerViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly SessionSettings _settings;
        private readonly FilterFactory _filterFactory;

        public ObservableCollection<EFilterCondition> ConditionList { get; set; }

        public IAsyncCommand AddStrategyCommand { get; set; }
        public IAsyncCommand SaveStrategyCommand { get; set; }
        public IAsyncCommand RemoveStrategyCommand { get; set; }
        public IAsyncCommand AddFilterCommand { get; set; }


        public string Title { get; set; }

        public ObservableCollection<Strategy> Strategies { get; set; }
        private Strategy _choosenStrategy;
        public Strategy ChoosenStrategy
        {
            get => _choosenStrategy;
            set
            {
                if (_choosenStrategy != value)
                    _choosenStrategy = value;

                FilterItemsContainerViewModel.Update(ChoosenStrategy);
                
             
                OnPropertyChanged(nameof(ChoosenStrategy));
                OnPropertyChanged(nameof(TelegramMessageSettings));
            }
        }

        public ObservableCollection<EMarket> AllFilterTypes { get; set; }
        public ObservableCollection<EMarket> BettingMarketTypes { get; set; }
        public ObservableCollection<ETotalType> TotalTypeList { get; set; }
        public ObservableCollection<EBothToScoreType> BothToScoreTypes { get; set; }
        public ObservableCollection<ETimePart> TotalTimeTypeList { get; set; }
        public ObservableCollection<double> TotalParameterValues { get; set; } = new()
        {
            0.5,
            1.5,
            2.5,
            3.5,
            4.5,
            5.5,
            6.5,
            7.5,
            8.5,
        };

        private double _choosenBetTotalParameter;
        public double ChoosenBetTotalParameter 
        { 
            get => _choosenBetTotalParameter;
            set
            {
                _choosenBetTotalParameter = value;
                ChoosenStrategy.Bet.TotalParameter = value.ToString();

                OnPropertyChanged(nameof(ChoosenBetTotalParameter));
            }
        }

        public EMarket ChoosenFilter { get; set; }
        public string FilterName { get; set; }
        public string StrategyName { get; set; }
        public FilterItemsContainerViewModel FilterItemsContainerViewModel { get; set; }
        public TelegramMessageSettings TelegramMessageSettings { get; set; }

        public StrategyManagerViewModel(ISettingsService service, FilterFactory filterFactory)
        {
            _settingsService = service;
            _settings = _settingsService.Get();
            _filterFactory = filterFactory;

            Strategies = new ObservableCollection<Strategy>(_settings.Strategies);
            
            AddStrategyCommand = new AsyncCommand(AddStrategy);
            RemoveStrategyCommand = new AsyncCommand(RemoveStrategy);
            AddFilterCommand = new AsyncCommand(AddFilter);
            SaveStrategyCommand = new AsyncCommand(SaveStrategy);
            
            FilterItemsContainerViewModel = new FilterItemsContainerViewModel();
            TelegramMessageSettings = new TelegramMessageSettings();
            
            ChoosenStrategy = _settings.Strategies.FirstOrDefault();
            FilterItemsContainerViewModel.Update(ChoosenStrategy);

            FillEnumMarketTypes();

            Title = "Панель управления стратегиями";
        }

        private void FillEnumMarketTypes()
        {
            var filterTypes = EnumUtility.GetValues<EMarket>().ToList();
            filterTypes.Remove(EMarket.WinAway);
            filterTypes.Remove(EMarket.WinHome);
            filterTypes.Remove(EMarket.Draw);
            filterTypes.Remove(EMarket.BothToScore);


            AllFilterTypes = new ObservableCollection<EMarket>(filterTypes);
            ConditionList = new ObservableCollection<EFilterCondition>(EnumUtility.GetValues<EFilterCondition>());
            TotalTypeList = new ObservableCollection<ETotalType>(EnumUtility.GetValues<ETotalType>());
            BothToScoreTypes = new ObservableCollection<EBothToScoreType>(EnumUtility.GetValues<EBothToScoreType>());
            TotalTimeTypeList = new ObservableCollection<ETimePart>(EnumUtility.GetValues<ETimePart>());

            FillMarketForBetting();
        }


        private void FillMarketForBetting()
        {
            var marketTypes = EnumUtility.GetValues<EMarket>().ToList();

            marketTypes.Remove(EMarket.Attacks);
            marketTypes.Remove(EMarket.DangerousAttacks);
            marketTypes.Remove(EMarket.Corners);
            marketTypes.Remove(EMarket.KickToGateBorder);
            marketTypes.Remove(EMarket.KickToGateDirection);
            marketTypes.Remove(EMarket.RedCards);
            marketTypes.Remove(EMarket.YellowCards);
            marketTypes.Remove(EMarket.Goals);

            BettingMarketTypes = new ObservableCollection<EMarket>(marketTypes);
        }

        private async Task AddFilter()
        {
            if (ChoosenStrategy is null)
                return;

            if (ChoosenFilter is EMarket.None)
                return;

            var filter = _filterFactory.CreateDefaultFilter(ChoosenFilter, FilterName);
            
            ChoosenStrategy.Filters.Add(filter);
            FilterItemsContainerViewModel.Update(ChoosenStrategy);
        }

        public async Task RemoveFilter(string id)
        {
            var filter = ChoosenStrategy.Filters.Find(x => x.Id == id);
            ChoosenStrategy.Filters.Remove(filter);
            FilterItemsContainerViewModel.Update(ChoosenStrategy, true);
        }

        private async Task SaveStrategy()
        {
            var filterData = FilterItemsContainerViewModel.GetAllFilters();
            
            ChoosenStrategy.Filters = filterData;

            _settingsService.Save();
        }

        private async Task AddStrategy()
        {
            var strategy = new Strategy
            {
                Name = "Название стратегии",
                Id = Guid.NewGuid().ToString(),
                Filters = _filterFactory.CreateDefaultFilters() 
            };
            Strategies.Add(strategy);
            _settings.Strategies.Add(strategy);
        }

        private async Task RemoveStrategy()
        {
            _settings.Strategies.Remove(ChoosenStrategy);
            Strategies.Remove(ChoosenStrategy);
            _settingsService.Save();

            if (_settings.Strategies.Count != 0)
                ChoosenStrategy = _settings.Strategies.First();
        }

    }

    
}
