using Google.Protobuf.WellKnownTypes;

namespace RockPaperScissors.Model.Entity;

public class MatchHistory
{
    public Guid MatchId { get; set; }
    
    public Guid? PlayerOneId { get; set; }
    
    public Guid? PlayerTwoId { get; set; }
    
    public decimal AmountBet { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public MatchStatus Status { get; set; }
    
    public string? PlayerOneMove { get; set; }
    
    public string? PlayerTwoMove { get; set; }
    
    public virtual User PlayerOne { get; set; }
    
    public virtual User PlayerTwo { get; set; }
    
    public virtual ICollection<GameTransaction> Transactions { get; set; } = new List<GameTransaction>();
}