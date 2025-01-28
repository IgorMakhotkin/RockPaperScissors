namespace RockPaperScissors.Model.Matches;

public class Match
{
    public Guid MatchId { get; set; }
    public decimal MatchBet { get; set; }
    public MatchStatus Status { get; set; }
}