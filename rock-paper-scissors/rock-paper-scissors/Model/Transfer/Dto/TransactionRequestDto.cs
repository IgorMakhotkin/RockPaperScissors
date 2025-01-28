namespace RockPaperScissors.Model.Transfer.Dto
{
    public class TransactionRequestDto
    {
        public Guid FromUserId { get; set; }
        
        public Guid ToUserId { get; set; }

        public decimal Amount { get; set; }
    }
}
