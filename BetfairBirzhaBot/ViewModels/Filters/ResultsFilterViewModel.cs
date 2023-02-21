using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Converters;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Settings;
using BetfairBirzhaBot.Utilities;
using BetfairBirzhaBot.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.Controls.Filters
{
    public class ResultsFilterViewModel : BaseViewModel
    {
        public ResultsFilter Filter { get; set; }

        public int CloseButtonColumnWidth { get; set; }
        public int EnableButtonColumnWidth { get; set; }
        public int MarketNameColumnWidth { get; set; }

        private string _from;
        public string From
        {
            get { return _from; }
            set 
            { 
                Filter.From = value.ConvertToDouble();

                _from = value;

                OnPropertyChanged(nameof(From));
            }
        }

        private string _to;
        public string To
        {
            get { return _to; }
            set 
            {
                Filter.To = value.ConvertToDouble();

                _to = value;

                OnPropertyChanged(nameof(To));
            }
        }


        public ObservableCollection<EFilterCondition> ConditionList { get; set; }
        public ResultsFilterViewModel(ResultsFilter filter)
        {
            Filter = filter;
            From = filter.From.ToString().Replace(",", ".");
            To = filter.To.ToString().Replace(",", ".");
            ConditionList = new ObservableCollection<EFilterCondition>(EnumUtility.GetValues<EFilterCondition>());
            RemoveFilterCommand = new AsyncCommand(Remove);

            if (Filter.Type == EMarket.WinAway || Filter.Type == EMarket.WinHome || Filter.Type == EMarket.Draw)
            {
                CloseButtonColumnWidth = 0;
                EnableButtonColumnWidth = 15;
                MarketNameColumnWidth = 35;
            }
            else
            {
                CloseButtonColumnWidth = 40;
                EnableButtonColumnWidth = 15;
                MarketNameColumnWidth = 100;
            }

            OnPropertyChanged(nameof(CloseButtonColumnWidth));
            OnPropertyChanged(nameof(EnableButtonColumnWidth));
            OnPropertyChanged(nameof(MarketNameColumnWidth));
        }

        public IAsyncCommand RemoveFilterCommand { get; set; }
        private StrategyManagerViewModel _managerVm;
        private async Task Remove()
        {
            _managerVm = App.ServiceProvider.GetService<StrategyManagerViewModel>();
            await _managerVm.RemoveFilter(Filter.Id);
        }
    }
}
