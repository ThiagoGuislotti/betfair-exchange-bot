using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Models;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.ViewModels
{
    public class LeaguesBlackListViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly SessionSettings _settings;


        public List<LeagueModel> _currentLeagues;
        public ObservableCollection<LeagueModel> CurrentLeagues { get; set; }

        private Strategy _choosenStrategy;
        public Strategy ChoosenStrategy
        {
            get => _choosenStrategy;
            set
            {
                _choosenStrategy = value;
                CurrentLeagues = new ObservableCollection<LeagueModel>(value.Leagues);
                _currentLeagues = value.Leagues;
                OnPropertyChanged(nameof(ChoosenStrategy));
                OnPropertyChanged(nameof(CurrentFilterSearch));
                OnPropertyChanged(nameof(CurrentLeagues));
            }
        }

        private string _searchFilter { get; set; }
        public string CurrentFilterSearch
        {
            get => _searchFilter;
            set
            {
                _searchFilter = value;

                var filtered = _currentLeagues.FindAll(x => x.Name.ToLower().Contains(value.ToLower()));
                CurrentLeagues = new ObservableCollection<LeagueModel>(filtered);
                OnPropertyChanged(nameof(CurrentFilterSearch));
                OnPropertyChanged(nameof(CurrentLeagues));
            }
        }

        public List<Strategy> Strategies { get; set; }

        public string Title { get; set; }
        
        public IAsyncCommand SaveCommand { get; set; }
        public IAsyncCommand RemoveSelectedCommand { get; set; }
        public IAsyncCommand SelectAllCommand { get; set; }
        public IAsyncCommand DeselectAllCommand { get; set; }
        

        public LeaguesBlackListViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _settings = settingsService.Get();

            Strategies = _settings.Strategies;

            ChoosenStrategy = _settings.Strategies.FirstOrDefault();
            Title = "Настройка черного списка лиг";

            SaveCommand = new AsyncCommand(Save);
            RemoveSelectedCommand = new AsyncCommand(RemoveSelected);
            SelectAllCommand = new AsyncCommand(SelectAll);
            DeselectAllCommand = new AsyncCommand(ClearBlackList);
        }

        

        public async Task Save()
        {
            _settings.Leagues = CurrentLeagues.ToList();
            _settingsService.Save();
        }

        public async Task RemoveSelected()
        {
            var list = CurrentLeagues.ToList().FindAll(x => x.SelectedItem).Select(x=>x.Name).ToList();
            ChoosenStrategy.Leagues.RemoveAll(x => list.Exists(c => c == x.Name));
            ChoosenStrategy = _choosenStrategy;
            OnPropertyChanged(nameof(ChoosenStrategy));
            OnPropertyChanged(nameof(CurrentFilterSearch));
        }

        public void SetAllItemsSelected(bool selected)
        {
            _choosenStrategy.Leagues.ForEach(l => l.SelectedItem = selected);
            ChoosenStrategy = _choosenStrategy;
        }

        public async Task SelectAll()
        {
            SetAllItemsSelected(true);
        }

        public async Task ClearBlackList()
        {
            _choosenStrategy.Leagues.ForEach(l => l.IncludeToBlacklist = false);
            SetAllItemsSelected(false);
            CurrentFilterSearch = "";
        }


    }
}
