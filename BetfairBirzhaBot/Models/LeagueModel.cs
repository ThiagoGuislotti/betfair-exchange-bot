

namespace BetfairBirzhaBot.Models
{
    public class LeagueModel
    {
        public string Name { get; init; }
        public bool IncludeToBlacklist { get; set; } = false;
        public bool SelectedItem { get; set; } = false;
        public LeagueModel(string name)
        {
            Name = name;
        }
    }
}
