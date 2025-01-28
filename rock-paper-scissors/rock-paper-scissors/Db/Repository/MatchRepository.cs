using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Db.Repository.Interface;
using RockPaperScissors.Model;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Matches;
using MatchStatus = RockPaperScissors.Model.MatchStatus;

namespace RockPaperScissors.Db.Repository;

public class MatchRepository : IMatchRepository
{
    private readonly GameDbContext _dbContext;

    public MatchRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    

    public async Task<List<Match?>> GetAllMatches(CancellationToken cancellationToken)
    {
        return await _dbContext.Matches.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<MatchResponse> CreateMatch(Match match, CancellationToken cancellationToken)
    {
        var result= await _dbContext.Matches.AddAsync(match, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new MatchResponse()
        {
            MatchId = result.Entity.MatchId,
            Status = MatchStatus.Success,
            Message = "Матч успешно создан"
        };
    }

    public async Task<Match?> GetMatchById(Guid matchId, CancellationToken cancellationToken)
    {
        return await _dbContext.Matches.FirstOrDefaultAsync(x => x.MatchId == matchId, cancellationToken);
    }
}