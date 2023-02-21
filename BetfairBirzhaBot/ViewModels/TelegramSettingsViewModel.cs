using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Core.Telegram;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Settings;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.ViewModels
{
    public class TelegramSettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly SessionSettings _settings;
        public string Title { get; set; }

        public IAsyncCommand SaveCommand { get; set; }

        public TelegramSettings TelegramSettings { get; set; }

        public TelegramSettingsViewModel(ISettingsService settingsService)
        {
            Title = "Настройка Telegram бота";

            SaveCommand = new AsyncCommand(Save);

            _settingsService = settingsService;
            _settings = _settingsService.Get();

            TelegramSettings = _settings.TelegramSettings;
            OnPropertyChanged(nameof(TelegramSettings));
        }

        private async Task Save()
        {
            _settings.TelegramSettings = TelegramSettings;
            _settingsService.Save();
        }
    }
}
