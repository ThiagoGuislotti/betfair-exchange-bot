using BetfairBirzhaBot.Core;

namespace BetfairBirzhaBot.Settings
{
    public class TelegramSettings
    {
        public bool IsEnabled { get; set; }
        public string Key { get; set; }
        public TelegramMessageSettings MessageSettings { get; set; } = new();
    }
}
