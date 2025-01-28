using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Db.Repository.Interface;
using RockPaperScissors.Model;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Transfer.Dto;

namespace RockPaperScissors.Db.Repository;

public class TransactionRepository : ITransactionRepository
{
    private readonly GameDbContext _dbContext;
    private readonly IUserRepository _userRepository;

    public TransactionRepository(GameDbContext dbContext, IUserRepository userRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }
    
    public async Task<TransactionResponse> TransferMoney(GameTransaction gameTransaction, CancellationToken cancellationToken)
    {
        var fromUser = await _userRepository.GetUserById(gameTransaction.FromUserId, cancellationToken);
        var toUser = await _userRepository.GetUserById(gameTransaction.ToUserId, cancellationToken);
        
        if (fromUser.Balance < gameTransaction.Amount)
        {
            return new TransactionResponse()
            {
                Status = TransactionStatus.Failure
            };
        }
        
        fromUser.Balance -= gameTransaction.Amount;
        toUser.Balance += gameTransaction.Amount;
        gameTransaction.TransactionType = TransactionType.Transfer;
        gameTransaction.CreatedAt = DateTime.UtcNow;
      

        var result = _dbContext.GameTransactions.Add(gameTransaction);
        await _userRepository.UpdateUserBalance(gameTransaction.FromUserId, fromUser.Balance, cancellationToken);
        await _userRepository.UpdateUserBalance(gameTransaction.ToUserId, toUser.Balance, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TransactionResponse()
        {
            Status = TransactionStatus.Success,
            TransactionId = result.Entity.TransactionId
        };
    }
}