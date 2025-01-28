using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Transfer.Dto;

namespace RockPaperScissors.Db.Repository.Interface;

public interface ITransactionRepository
{
    Task<TransactionResponse> TransferMoney(GameTransaction gameTransaction, CancellationToken cancellationToken);
}