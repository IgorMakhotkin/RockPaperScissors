using RockPaperScissors.Model.Transfer.Dto;

namespace RockPaperScissors.Interface;

public interface ITransactionsService
{
    Task<TransactionResponse> Transfer(Transaction transaction, CancellationToken cancellationToken);
}