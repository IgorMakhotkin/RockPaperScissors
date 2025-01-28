

namespace RockPaperScissors.Model.Transfer.Dto;

public class TransactionResponse
{
    public Guid? TransactionId { get; set; }
    
    public TransactionStatus Status { get; set; }
}