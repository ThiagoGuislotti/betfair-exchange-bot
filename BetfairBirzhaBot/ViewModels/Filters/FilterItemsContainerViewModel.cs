using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Controls.Filters;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Interfaces;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Settings;
using BetfairBirzhaBot.ViewModels.Filters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BetfairBirzhaBot.ViewModels
{
    public class FilterItemsContainerViewModel : BaseViewModel
    {
        private Strategy _strategy;
        public ObservableCollection<ResultsFilterViewModel> ResultsFilters { get; set; } = new();
        public ObservableCollection<StaticDataFilterViewModel> StaticDataFilters { get; set; } = new();
        public ObservableCollection<TotalFilterViewModel> TotalDataFilters { get; set; } = new();
        public ObservableCollection<CorrectScoreFilterViewModel> CorrectScoreFilters { get; set; } = new();
        public ObservableCollection<ResultsFilterViewModel> VisibleResultFilters { get; set; } = new();
        public ObservableCollection<ResultsFilterViewModel> WinResultsLiveFilters { get; set; } = new();
        public ObservableCollection<ResultsFilterViewModel> WinResultsPrematchFilters { get; set; } = new();
        public ObservableCollection<BothToScoreFilterViewModel> BothToScorePrematchFilters { get; set; } = new();
        public ObservableCollection<BothToScoreFilterViewModel> BothToScoreLiveFilters { get; set; } = new();

        public FilterItemsContainerViewModel()
        {
        }

        public void Update(Strategy strategy, bool forceUpdate = false)
        {
            if (strategy is null)
                return;

            Clear();
            _strategy = strategy;

            var currentFilters = GetAllFilters();

            foreach (var filter in strategy.Filters)
            {
                var filterBase = (FilterBase)filter;
                

                if (filterBase.Group == EFilterGroup.Results)
                {
                    if (currentFilters.Exists(x => x.Type == filterBase.Type))
                        continue;
                    var vm = new ResultsFilterViewModel(filter as ResultsFilter);
                    if (filterBase.IsPrematch)
                        WinResultsPrematchFilters.Add(vm);
                    else
                        WinResultsLiveFilters.Add(vm);
                }
                if (filterBase.Group == EFilterGroup.BothToScore)
                {
                    if (currentFilters.Exists(x => x.Type == filterBase.Type))
                        continue;

                    var vm = new BothToScoreFilterViewModel(filter as BothToScoreFilter);
                    if (filterBase.IsPrematch)
                        BothToScorePrematchFilters.Add(vm);
                    else
                        BothToScoreLiveFilters.Add(vm);
                }

                if (filter.Group == EFilterGroup.Static)
                    StaticDataFilters.Add(new StaticDataFilterViewModel(filter as StaticDataFilter));

                if (filter.Group == EFilterGroup.Total)
                {
                    filterBase.IsActive = true;
                    TotalDataFilters.Add(new TotalFilterViewModel(filter as TotalsFilter));
                }

                if (filter.Group == EFilterGroup.CorrectScore)
                    CorrectScoreFilters.Add(new CorrectScoreFilterViewModel(filter as CorrectScoreFilter));

                if (filter.Type.ToString().Contains("Chance"))
                    VisibleResultFilters.Add(new ResultsFilterViewModel(filter as ResultsFilter));
            }

            
            
            OnPropertyChanged(nameof(VisibleResultFilters));
            OnPropertyChanged(nameof(StaticDataFilters));
            OnPropertyChanged(nameof(TotalDataFilters));
            OnPropertyChanged(nameof(CorrectScoreFilters));

            OnPropertyChanged(nameof(WinResultsPrematchFilters));
            OnPropertyChanged(nameof(WinResultsLiveFilters));
            OnPropertyChanged(nameof(BothToScorePrematchFilters));
            OnPropertyChanged(nameof(BothToScoreLiveFilters));
            

            _strategy = strategy;
        }

        private void Clear()
        {
            StaticDataFilters.Clear();
            TotalDataFilters.Clear();
            CorrectScoreFilters.Clear();
            VisibleResultFilters.Clear();
            WinResultsPrematchFilters.Clear();
            WinResultsLiveFilters.Clear();

            BothToScorePrematchFilters.Clear();
            BothToScoreLiveFilters.Clear();
        }

        public List<IFilter> GetAllFilters()
        {
            var list = new List<IFilter>();

            list.AddRange(StaticDataFilters.Select(x => x.Filter));
            list.AddRange(TotalDataFilters.Select(x => x.Filter));
            list.AddRange(CorrectScoreFilters.Select(x => x.Filter));

            list.AddRange(WinResultsPrematchFilters.Select(x => x.Filter));
            list.AddRange(WinResultsLiveFilters.Select(x => x.Filter));
            list.AddRange(BothToScorePrematchFilters.Select(x => x.Filter));
            list.AddRange(BothToScoreLiveFilters.Select(x => x.Filter));

            return list;
        }
    }


}
