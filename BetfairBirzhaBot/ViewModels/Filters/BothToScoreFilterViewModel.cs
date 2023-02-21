using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Converters;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.ViewModels.Filters
{
    public class BothToScoreFilterViewModel : BaseViewModel
    {
        public ObservableCollection<EFilterCondition> ConditionList { get; set; }
        public ObservableCollection<EBothToScoreType> Types { get; set; }

        public BothToScoreFilter Filter { get; set; }

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


        public BothToScoreFilterViewModel(BothToScoreFilter filter)
        {
            Filter = filter;
            From = filter.From.ToString().Replace(",", ".");
            To = filter.To.ToString().Replace(",", ".");
            Types = new ObservableCollection<EBothToScoreType>(EnumUtility.GetValues<EBothToScoreType>());
            ConditionList = new ObservableCollection<EFilterCondition>(EnumUtility.GetValues<EFilterCondition>());
            RemoveFilterCommand = new AsyncCommand(Remove);
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
