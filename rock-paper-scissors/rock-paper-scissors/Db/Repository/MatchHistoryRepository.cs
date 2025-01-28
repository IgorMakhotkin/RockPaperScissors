using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Db.Repository.Interface;
using RockPaperScissors.Model;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Matches;
using MatchStatus = RockPaperScissors.Model.MatchStatus;

namespace RockPaperScissors.Db.Repository;

public class MatchHistoryRepository : IMatchHistoryRepository
{
    private readonly GameDbContext _dbContext;

    public MatchHistoryRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<MatchHistory?> GetMatchHistoryById(Guid matchId, CancellationToken cancellationToken)
    {
        return await _dbContext.MatchHistories.FirstOrDefaultAsync(x => x.MatchId == matchId, cancellationToken);
    }

    public async Task<List<MatchHistory?>> GetAllMatches(CancellationToken cancellationToken)
    {
        return await _dbContext.MatchHistories.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<MatchResponse> CreateMatch(Match match, CancellationToken cancellationToken)
    {
        var newMatch = new MatchHistory
        {
            MatchId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AmountBet = match.MatchBet,
            PlayerOneMove = string.Empty,
            PlayerTwoMove = string.Empty,
            Status = MatchStatus.NotStarted
        };
        var result= await _dbContext.MatchHistories.AddAsync(newMatch, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new MatchResponse()
        {
            MatchId = newMatch.MatchId,
            Status = MatchStatus.Success,
            Message = "Матч успешно создан"
        };
    }
    

    public async Task UpdateMatchHistory(MatchHistory? matchHistory, CancellationToken cancellationToken)
    {
        var entity = _dbContext.MatchHistories.FirstOrDefault(x => x.MatchId == matchHistory.MatchId);
        if (entity == null) {
            await _dbContext.MatchHistories.AddAsync(matchHistory, cancellationToken);
        }
        else
        {
            _dbContext.MatchHistories.Update(matchHistory);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}