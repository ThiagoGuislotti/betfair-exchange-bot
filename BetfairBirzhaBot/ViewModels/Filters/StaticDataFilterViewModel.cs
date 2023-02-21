using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Utilities;
using BetfairBirzhaBot.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.Controls.Filters
{
    public class StaticDataFilterViewModel : BaseViewModel
    {
        public StaticDataFilter Filter { get; set; }
        public ObservableCollection<EFilterCondition> ConditionList { get; set; }

        private bool _secondModeActive { get; set; }
        private EFilterCondition _secondModeCondition { get; set; }
        private int _secondModeValue { get; set; }

        public bool SecondModeActive
        {
            get => _secondModeActive;
            set
            {
                _secondModeActive = value;

                Filter.SecondModeActive = value;
                OnPropertyChanged(nameof(SecondModeActive));
            }
        }

        public EFilterCondition SecondModeCondition
        {
            get => _secondModeCondition;
            set
            {
                _secondModeCondition = value;

                Filter.SecondModeCondition = value;
                OnPropertyChanged(nameof(SecondModeCondition));

            }
        }

        public int SecondModeValue
        {
            get => _secondModeValue;
            set
            {
                _secondModeValue = value;

                Filter.SecondModeValue = value;
                OnPropertyChanged(nameof(SecondModeValue));

            }
        }

        public StaticDataFilterViewModel(StaticDataFilter filter)
        {
            ConditionList = new ObservableCollection<EFilterCondition>(EnumUtility.GetValues<EFilterCondition>());
            Filter = filter;
            SecondModeActive = filter.SecondModeActive;
            SecondModeCondition = filter.SecondModeCondition;
            SecondModeValue = filter.SecondModeValue;
            RemoveFilterCommand = new AsyncCommand(Remove);
            OnPropertyChanged(nameof(Filter));
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
