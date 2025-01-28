using Google.Protobuf.WellKnownTypes;

namespace RockPaperScissors.Model.Entity;

public class GameTransaction
{
    public Guid TransactionId { get; set; }
    public Guid GameId { get; set; }
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt  { get; set; }
    public TransactionType TransactionType  { get; set; }
    
    public virtual User Sender { get; set; }
    
    public virtual User Receiver { get; set; }
    
    public virtual MatchHistory Match { get; set; }
}