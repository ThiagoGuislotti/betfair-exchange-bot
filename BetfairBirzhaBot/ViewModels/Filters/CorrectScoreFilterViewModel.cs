using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.ViewModels.Filters
{
    public class CorrectScoreFilterViewModel : BaseViewModel
    {
        public ObservableCollection<EFilterCondition> ConditionList { get; set; }

        public CorrectScoreFilter Filter { get; set; }



        public CorrectScoreFilterViewModel(CorrectScoreFilter filter)
        {
            Filter = filter;

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
