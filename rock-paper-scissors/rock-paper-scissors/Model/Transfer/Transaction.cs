namespace RockPaperScissors.Model.Transfer.Dto
{
    public class Transaction
    {
        public Guid GameId { get; set; }
        public Guid? FromUserId { get; set; }
        
        public Guid? ToUserId { get; set; }

        public decimal Amount { get; set; }
    }
}
