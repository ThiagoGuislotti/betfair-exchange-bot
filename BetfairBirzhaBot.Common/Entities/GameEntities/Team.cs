namespace BetfairBirzhaBot.Common.Entities
{
    public class Team
    {
        public string Name { get; set; }
        public ETeamType Type { get; set; }
        public int Score { get; set; }
        public int Attacks { get; set; }
        public int DangerousAttacks { get; set; }
        public int BallPercent { get; set; }
        public int ShotsOnTarget { get; set; }
        public int ShotsOffTarget { get; set; }
        public int Corners { get; set; }
        public int RedCards { get; set; }
        public int YellowCards { get; set; }

        public Team(string name, ETeamType type)
        {
            Name = name;
            Type = type;
        }

        public Team()
        {

        }
    }
}
