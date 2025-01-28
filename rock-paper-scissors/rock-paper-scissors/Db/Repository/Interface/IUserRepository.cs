using RockPaperScissors.Model.Entity;

namespace RockPaperScissors.Db.Repository.Interface;

public interface IUserRepository
{
    Task<User?> GetUserById(Guid userId, CancellationToken cancellationToken);
    
    Task<User?> GetUserByName(string username, CancellationToken cancellationToken);
    
    Task<decimal> GetUserBalanceById(Guid userId, CancellationToken cancellationToken);
    
    Task<decimal> UpdateUserBalance(Guid userId, decimal neBalance, CancellationToken cancellationToken);
    
    Task<User> CreateUser(string username, CancellationToken cancellationToken);
}