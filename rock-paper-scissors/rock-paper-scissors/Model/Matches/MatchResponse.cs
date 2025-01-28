namespace RockPaperScissors.Model.Matches;

public class MatchResponse
{
    public Guid? MatchId { get; set; }
    
    public MatchStatus Status { get; set; }
    
    public string Message { get; set; }
}