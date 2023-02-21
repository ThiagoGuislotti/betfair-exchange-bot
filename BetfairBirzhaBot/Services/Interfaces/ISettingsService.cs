using BetfairBirzhaBot.Settings;

namespace BetfairBirzhaBot.Services.Interfaces
{
    public interface ISettingsService
    {
        SessionSettings Get();
        void Save();
    }
}
