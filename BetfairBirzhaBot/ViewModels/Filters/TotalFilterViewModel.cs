using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Utilities;
using BetfairBirzhaBot.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BetfairBirzhaBot.Converters;

namespace BetfairBirzhaBot.Controls.Filters
{
    public class TotalFilterViewModel : BaseViewModel
    {
        public TotalsFilter Filter { get; set; }
        public string Name { get; set; }
        public ObservableCollection<EFilterCondition> ConditionList { get; set; }
        public ObservableCollection<ETimePart> TotalPartList { get; set; }
        public ObservableCollection<ETotalType> TotalTypeList { get; set; }

        private double _choosenTotalParameter;

        public double ChoosenTotalParameter
        {
            get
            { 
                return _choosenTotalParameter;
            }
            set 
            {
                _choosenTotalParameter = value;

                Filter.TotalParameter = value;

                OnPropertyChanged(nameof(ChoosenTotalParameter));
            }
        }

        public int TotalParameterSelectedIndex { get; set; }

        private string _points;
        public string Points 
        { 
            get => _points;
            set
            {
                if (value == null)
                    return;

                
                Filter.TotalParameter = value.ConvertToDouble();

                _points = value;

                OnPropertyChanged(nameof(Points));
            }
        }

        private string _from;
        public string From
        {
            get => _from;
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
            get => _to;
            set
            {
                Filter.To = value.ConvertToDouble();

                _to = value;

                OnPropertyChanged(nameof(To));
            }
        }

        public string TotalTypeGroupKey { get;set; }
        public string TargetForParsingGroupKey { get; set; }

        private bool _isUnderChecked;
        private bool _isOverChecked;
        private bool _isPrematchChecked;
        private bool _isLiveChecked;
        public bool IsUnderChecked { get => _isUnderChecked; 
            set 
            {
                if (value)
                    Filter.TotalType = ETotalType.Under;
                else
                    Filter.TotalType = ETotalType.Over;

                _isUnderChecked = value;
            }
        }
        public bool IsOverChecked
        {
            get => _isOverChecked;
            set
            {
                if (value)
                    Filter.TotalType = ETotalType.Over;
                else
                    Filter.TotalType = ETotalType.Under;

                _isOverChecked = value;
            }
        }
        public bool IsPrematchChecked
        {
            get => _isPrematchChecked;
            set
            {
                Filter.IsPrematch = value;
                _isPrematchChecked = value;
            }
        }
        public bool IsLiveChecked
        {
            get => _isLiveChecked;
            set
            {
                if (value)
                    Filter.IsPrematch = false;
                else
                    Filter.IsPrematch = true;

                _isLiveChecked = value;
            }
        }

        public ObservableCollection<double> ParameterValues { get; set; }
        public TotalFilterViewModel(TotalsFilter filter)
        {
            Filter = filter;

            Points = Filter.TotalParameter.ToString();
            From = Filter.From.ToString();
            To = Filter.To.ToString();
            

            ConditionList = new ObservableCollection<EFilterCondition>(EnumUtility.GetValues<EFilterCondition>());
            TotalPartList = new ObservableCollection<ETimePart>(EnumUtility.GetValues<ETimePart>());
            TotalTypeList = new ObservableCollection<ETotalType>(EnumUtility.GetValues<ETotalType>());
            RemoveFilterCommand = new AsyncCommand(Remove);

            ParameterValues = new ObservableCollection<double>
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
                9.5,
            };

            TotalTypeGroupKey = Guid.NewGuid().ToString(); 
            TargetForParsingGroupKey = Guid.NewGuid().ToString();

            if (filter.IsPrematch)
                IsPrematchChecked = true;
            else
                IsLiveChecked = true;

            if (filter.TotalType == ETotalType.Under)
                IsUnderChecked = true;
            else
                IsOverChecked = true;

            if (filter.TotalParameter != 0)
            {
                Points = filter.TotalParameter.ToString();
                TotalParameterSelectedIndex = ParameterValues.ToList().IndexOf(filter.TotalParameter);
                ChoosenTotalParameter = filter.TotalParameter;
                OnPropertyChanged(nameof(TotalParameterSelectedIndex));
            }
            
            Name = $"Тотал {Points}";
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
