using Mapster;
using RockPaperScissors.Db.Repository.Interface;
using RockPaperScissors.Interface;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Transfer.Dto;

namespace RockPaperScissors.Services
{
    public class TransactionsService : ITransactionsService
    {

        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;

        public TransactionsService(ITransactionRepository transactionRepository, IUserRepository userRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
        }

        public async Task<TransactionResponse> Transfer(Transaction transaction, CancellationToken cancellationToken)
        {
          return await _transactionRepository.TransferMoney(transaction.Adapt<GameTransaction>(), cancellationToken);
        }
        
    }
}
