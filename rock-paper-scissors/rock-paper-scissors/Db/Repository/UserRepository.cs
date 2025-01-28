using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Db.Repository.Interface;
using RockPaperScissors.Model.Entity;

namespace RockPaperScissors.Db.Repository;

public class UserRepository : IUserRepository
{
    private readonly GameDbContext _dbContext;

    public UserRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Получить пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<User?> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => userId == x.UserId, cancellationToken: cancellationToken);
    }

    public async Task<User?> GetUserByName(string username, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Получить баланс 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<decimal> GetUserBalanceById(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken: cancellationToken);
        return user?.Balance ?? 0m;
    }

    /// <summary>
    /// Обновить баланс пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="neBalance"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<decimal> UpdateUserBalance(Guid userId, decimal neBalance, CancellationToken cancellationToken)
    {
        var user = await GetUserById(userId, cancellationToken);
        user!.Balance += neBalance;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user.Balance;
    }

    /// <summary>
    /// Создать нового пользователя
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<User> CreateUser(string username, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
        if (user != null)
        {
            return user;
        }
        var newUser = new User
        {
            Username = username,
            Balance = 0m
        }; 
        
        _dbContext.Users.Add(newUser);
       await _dbContext.SaveChangesAsync(cancellationToken);
       return newUser;
    }
}