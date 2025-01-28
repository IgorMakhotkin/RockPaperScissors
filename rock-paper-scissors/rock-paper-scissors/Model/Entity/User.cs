namespace RockPaperScissors.Model.Entity;

public class User
{
    public Guid UserId { get; set; }
    
    public string Username { get; set; }
    
    public decimal Balance { get; set; }
    
    public virtual  ICollection<GameTransaction> GameTransactionsSent { get; set; } = new List<GameTransaction>();
    
    public virtual  ICollection<GameTransaction> GameTransactionsReceived { get; set; } = new List<GameTransaction>();
}