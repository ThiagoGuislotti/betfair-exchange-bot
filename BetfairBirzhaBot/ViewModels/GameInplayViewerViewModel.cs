using BetfairBirzhaBot.Base;
namespace BetfairBirzhaBot.ViewModels
{
    public class GameInplayViewerViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public GameInplayViewerViewModel()
        {
            Title = "Просмотр коефициентов при парсинге в лайве.";
        }
    }
}
