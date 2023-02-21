// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using Newtonsoft.Json;

public class Action
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("ActionTime")]
    public string ActionTime { get; set; }

    [JsonProperty("ActionPhase")]
    public string ActionPhase { get; set; }

    [JsonProperty("MatchDuration")]
    public MatchDurationEntity  MatchDuration { get; set; }

    [JsonProperty("PlayerOff")]
    public string PlayerOff { get; set; }

    [JsonProperty("PlayerOn")]
    public string PlayerOn { get; set; }

    [JsonProperty("SubstitutionDirection")]
    public string SubstitutionDirection { get; set; }

    [JsonProperty("Phase")]
    public string Phase { get; set; }

    [JsonProperty("Time")]
    public double Time { get; set; }

    [JsonProperty("Sequence")]
    public int Sequence { get; set; }

    [JsonProperty("TeamSide")]
    public string TeamSide { get; set; }

    [JsonProperty("State")]
    public string State { get; set; }

    [JsonProperty("TeamName")]
    public string TeamName { get; set; }

    [JsonProperty("PlayerName")]
    public string PlayerName { get; set; }

    [JsonProperty("FoulingTeam")]
    public string FoulingTeam { get; set; }

    [JsonProperty("FouledPlayer")]
    public string FouledPlayer { get; set; }

    [JsonProperty("FouledByPlayer")]
    public string FouledByPlayer { get; set; }
}

public class Away
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("TeamName")]
    public string TeamName { get; set; }

    [JsonProperty("PlayersOnField")]
    public List<PlayersOnField> PlayersOnField { get; set; }

    [JsonProperty("PlayersOnBench")]
    public List<PlayersOnBench> PlayersOnBench { get; set; }
}

public class AwayTeam
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("TeamId")]
    public string TeamId { get; set; }
}

public class Commentary
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("FootballCommentaryActions")]
    public List<FootballCommentaryAction> FootballCommentaryActions { get; set; }
}

public class FirstHalf
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Started")]
    public bool Started { get; set; }

    [JsonProperty("Current")]
    public bool Current { get; set; }

    [JsonProperty("Total")]
    public int Total { get; set; }

    [JsonProperty("Added")]
    public int Added { get; set; }

    [JsonProperty("Normal")]
    public int Normal { get; set; }

    [JsonProperty("From")]
    public int From { get; set; }

    [JsonProperty("Icons")]
    public List<Icon> Icons { get; set; }
}

public class FirstHalfExtraTime
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Started")]
    public bool Started { get; set; }

    [JsonProperty("Current")]
    public bool Current { get; set; }

    [JsonProperty("Total")]
    public int Total { get; set; }

    [JsonProperty("From")]
    public int From { get; set; }

    [JsonProperty("Icons")]
    public List<object> Icons { get; set; }
}

public class FootballCommentaryAction
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Timestamp")]
    public string Timestamp { get; set; }

    [JsonProperty("TeamName")]
    public string TeamName { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("ElapsedTime")]
    public string ElapsedTime { get; set; }

    [JsonProperty("Phase")]
    public string Phase { get; set; }

    [JsonProperty("HomeScore")]
    public int HomeScore { get; set; }

    [JsonProperty("AwayScore")]
    public int AwayScore { get; set; }

    [JsonProperty("MatchDuration")]
    public MatchDurationEntity MatchDuration { get; set; }

    [JsonProperty("TeamSide")]
    public string TeamSide { get; set; }

    [JsonProperty("PlayerName")]
    public string PlayerName { get; set; }

    [JsonProperty("FootballPhaseScores")]
    public FootballPhaseScores FootballPhaseScores { get; set; }
}

public class FootballPhaseScores
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("HomeFirstHalf")]
    public int HomeFirstHalf { get; set; }

    [JsonProperty("AwayFirstHalf")]
    public int AwayFirstHalf { get; set; }

    [JsonProperty("HomeSecondHalf")]
    public int HomeSecondHalf { get; set; }

    [JsonProperty("AwaySecondHalf")]
    public int AwaySecondHalf { get; set; }

    [JsonProperty("CurrentPeriod")]
    public string CurrentPeriod { get; set; }
}

public class Home
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("TeamName")]
    public string TeamName { get; set; }

    [JsonProperty("PlayersOnField")]
    public List<PlayersOnField> PlayersOnField { get; set; }

    [JsonProperty("PlayersOnBench")]
    public List<PlayersOnBench> PlayersOnBench { get; set; }
}

public class HomeTeam
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("TeamId")]
    public string TeamId { get; set; }
}

public class Icon
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Position")]
    public double Position { get; set; }

    [JsonProperty("Time")]
    public int Time { get; set; }

    [JsonProperty("Team")]
    public string Team { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Home")]
    public bool Home { get; set; }
}

public class Incident
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("BetgeniusEventId")]
    public string BetgeniusEventId { get; set; }

    [JsonProperty("BookmakerEventId")]
    public string BookmakerEventId { get; set; }

    [JsonProperty("HomeTeam")]
    public HomeTeam HomeTeam { get; set; }

    [JsonProperty("AwayTeam")]
    public AwayTeam AwayTeam { get; set; }

    [JsonProperty("HomeScore")]
    public int HomeScore { get; set; }

    [JsonProperty("AwayScore")]
    public int AwayScore { get; set; }

    [JsonProperty("MatchDuration")]
    public MatchDurationEntity MatchDuration { get; set; }
}

public class Incident2
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Timestamp")]
    public string Timestamp { get; set; }

    [JsonProperty("Phase")]
    public string Phase { get; set; }

    [JsonProperty("Confirmed")]
    public bool Confirmed { get; set; }

    [JsonProperty("NextPhase")]
    public string NextPhase { get; set; }

    [JsonProperty("TeamId")]
    public string TeamId { get; set; }

    [JsonProperty("TimeInPhase")]
    public string TimeInPhase { get; set; }

    [JsonProperty("OwnGoal")]
    public bool? OwnGoal { get; set; }

    [JsonProperty("PlayerName")]
    public string PlayerName { get; set; }
}

public class Lineups
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("LineupsConfirmed")]
    public bool LineupsConfirmed { get; set; }

    [JsonProperty("Home")]
    public Home Home { get; set; }

    [JsonProperty("Away")]
    public Away Away { get; set; }
}

public class LiveUpdates
{
    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("NumberOfIncidents")]
    public int NumberOfIncidents { get; set; }

    [JsonProperty("Incidents")]
    public List<Incident> Incidents { get; set; }
}

public class MatchDurationEntity
{
    [JsonProperty("MatchDuration")]
    public int MatchDuration { get; set; }

    [JsonProperty("ExtraTimeDuration")]
    public int ExtraTimeDuration { get; set; }
}

public class PitchGraphic
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Actions")]
    public List<Action> Actions { get; set; }

    [JsonProperty("FootballPhaseScores")]
    public FootballPhaseScores FootballPhaseScores { get; set; }
}

public class PlayersOnBench
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Actions")]
    public List<Action> Actions { get; set; }

    [JsonProperty("BetgeniusId")]
    public string BetgeniusId { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("ShirtNumber")]
    public int ShirtNumber { get; set; }
}

public class PlayersOnField
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Actions")]
    public List<Action> Actions { get; set; }

    [JsonProperty("BetgeniusId")]
    public string BetgeniusId { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("ShirtNumber")]
    public int ShirtNumber { get; set; }
}

public class StatisticsResponse
{

    [JsonProperty("Scoreboard")]
    public Scoreboard Scoreboard { get; set; }

    [JsonProperty("Statistics")]
    public Statistics Statistics { get; set; }

    public bool IsValid() => Scoreboard != null && Scoreboard.HomeTeamName is not null;
}

public class Scoreboard
{
    [JsonProperty("HomeTeamName")]
    public string HomeTeamName { get; set; }

    [JsonProperty("AwayTeamName")]
    public string AwayTeamName { get; set; }

    [JsonProperty("StartTime")]
    public string StartTime { get; set; }

    [JsonProperty("Competition")]
    public string Competition { get; set; }

    [JsonProperty("VenueName")]
    public string VenueName { get; set; }

    [JsonProperty("CurrentPeriod")]
    public string CurrentPeriod { get; set; }

    [JsonProperty("CurrentPeriodStartTime")]
    public string CurrentPeriodStartTime { get; set; }

    [JsonProperty("HomePrimaryShirtColour")]
    public string HomePrimaryShirtColour { get; set; }

    [JsonProperty("HomeSecondaryShirtColour")]
    public string HomeSecondaryShirtColour { get; set; }

    [JsonProperty("AwayPrimaryShirtColour")]
    public string AwayPrimaryShirtColour { get; set; }

    [JsonProperty("AwaySecondaryShirtColour")]
    public string AwaySecondaryShirtColour { get; set; }

    [JsonProperty("HomeGoals")]
    public int HomeGoals { get; set; }

    [JsonProperty("HomeCorners")]
    public int HomeCorners { get; set; }

    [JsonProperty("HomeRedCards")]
    public int HomeRedCards { get; set; }

    [JsonProperty("HomeYellowCards")]
    public int HomeYellowCards { get; set; }

    [JsonProperty("HomePenalties")]
    public int HomePenalties { get; set; }

    [JsonProperty("AwayGoals")]
    public int AwayGoals { get; set; }

    [JsonProperty("AwayCorners")]
    public int AwayCorners { get; set; }

    [JsonProperty("AwayRedCards")]
    public int AwayRedCards { get; set; }

    [JsonProperty("AwayYellowCards")]
    public int AwayYellowCards { get; set; }
    public int RedCardsAll => HomeRedCards + AwayRedCards;
    public int YellowCardsAll => HomeYellowCards + AwayYellowCards;

    [JsonProperty("AwayPenalties")]
    public int AwayPenalties { get; set; }

    [JsonProperty("MatchDuration")]
    public MatchDurationEntity  MatchDuration { get; set; }
}

public class SecondHalf
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Started")]
    public bool Started { get; set; }

    [JsonProperty("Current")]
    public bool Current { get; set; }

    [JsonProperty("Total")]
    public int Total { get; set; }

    [JsonProperty("Added")]
    public int Added { get; set; }

    [JsonProperty("Normal")]
    public int Normal { get; set; }

    [JsonProperty("From")]
    public int From { get; set; }

    [JsonProperty("Icons")]
    public List<Icon> Icons { get; set; }
}

public class SecondHalfExtraTime
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Started")]
    public bool Started { get; set; }

    [JsonProperty("Current")]
    public bool Current { get; set; }

    [JsonProperty("Total")]
    public int Total { get; set; }

    [JsonProperty("From")]
    public int From { get; set; }

    [JsonProperty("Icons")]
    public List<object> Icons { get; set; }
}

public class Statistics
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("HomeGoals")]
    public int HomeGoals { get; set; }

    [JsonProperty("HomePenalties")]
    public int HomePenalties { get; set; }

    [JsonProperty("HomeYellowCards")]
    public int HomeYellowCards { get; set; }

    [JsonProperty("HomeRedCards")]
    public int HomeRedCards { get; set; }

    [JsonProperty("HomeTotalCards")]
    public int HomeTotalCards { get; set; }

    [JsonProperty("HomeSubstitutions")]
    public int HomeSubstitutions { get; set; }

    [JsonProperty("HomeShotsOnTarget")]
    public int HomeShotsOnTarget { get; set; }

    [JsonProperty("HomeShotsOffTarget")]
    public int HomeShotsOffTarget { get; set; }

    [JsonProperty("HomeBlockedShots")]
    public int HomeBlockedShots { get; set; }

    [JsonProperty("HomeShotsOffWoodwork")]
    public int HomeShotsOffWoodwork { get; set; }

    [JsonProperty("HomeTotalShots")]
    public int HomeTotalShots { get; set; }

    [JsonProperty("HomeCorners")]
    public int HomeCorners { get; set; }

    [JsonProperty("HomeFouls")]
    public int HomeFouls { get; set; }

    [JsonProperty("HomeOffsides")]
    public int HomeOffsides { get; set; }

    [JsonProperty("HomeGoalKicks")]
    public int HomeGoalKicks { get; set; }

    [JsonProperty("HomeFreeKicks")]
    public int HomeFreeKicks { get; set; }

    [JsonProperty("HomeDangerousFreeKicks")]
    public int HomeDangerousFreeKicks { get; set; }

    [JsonProperty("HomeThrowIns")]
    public int HomeThrowIns { get; set; }

    [JsonProperty("HomeAttacks")]
    public int HomeAttacks { get; set; }

    [JsonProperty("HomeDangerousAttacks")]
    public int HomeDangerousAttacks { get; set; }

    [JsonProperty("AwayGoals")]
    public int AwayGoals { get; set; }

    [JsonProperty("AwayPenalties")]
    public int AwayPenalties { get; set; }

    [JsonProperty("AwayYellowCards")]
    public int AwayYellowCards { get; set; }

    [JsonProperty("AwayRedCards")]
    public int AwayRedCards { get; set; }

    [JsonProperty("AwayTotalCards")]
    public int AwayTotalCards { get; set; }

    [JsonProperty("AwaySubstitutions")]
    public int AwaySubstitutions { get; set; }

    [JsonProperty("AwayShotsOnTarget")]
    public int AwayShotsOnTarget { get; set; }

    [JsonProperty("AwayShotsOffTarget")]
    public int AwayShotsOffTarget { get; set; }

    [JsonProperty("AwayBlockedShots")]
    public int AwayBlockedShots { get; set; }

    [JsonProperty("AwayShotsOffWoodwork")]
    public int AwayShotsOffWoodwork { get; set; }

    [JsonProperty("AwayTotalShots")]
    public int AwayTotalShots { get; set; }

    [JsonProperty("AwayCorners")]
    public int AwayCorners { get; set; }

    [JsonProperty("AwayFouls")]
    public int AwayFouls { get; set; }

    [JsonProperty("AwayOffsides")]
    public int AwayOffsides { get; set; }

    [JsonProperty("AwayGoalKicks")]
    public int AwayGoalKicks { get; set; }

    [JsonProperty("AwayFreeKicks")]
    public int AwayFreeKicks { get; set; }

    [JsonProperty("AwayDangerousFreeKicks")]
    public int AwayDangerousFreeKicks { get; set; }

    [JsonProperty("AwayThrowIns")]
    public int AwayThrowIns { get; set; }

    [JsonProperty("AwayAttacks")]
    public int AwayAttacks { get; set; }

    [JsonProperty("AwayDangerousAttacks")]
    public int AwayDangerousAttacks { get; set; }
}

public class Timeline
{
    [JsonProperty("Entity")]
    public string Entity { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("CurrentPeriod")]
    public string CurrentPeriod { get; set; }

    [JsonProperty("FirstHalf")]
    public FirstHalf FirstHalf { get; set; }

    [JsonProperty("SecondHalf")]
    public SecondHalf SecondHalf { get; set; }

    [JsonProperty("FirstHalfExtraTime")]
    public FirstHalfExtraTime FirstHalfExtraTime { get; set; }

    [JsonProperty("SecondHalfExtraTime")]
    public SecondHalfExtraTime SecondHalfExtraTime { get; set; }
}

