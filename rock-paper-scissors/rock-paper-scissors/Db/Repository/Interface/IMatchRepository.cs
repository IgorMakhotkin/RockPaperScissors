using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Matches;

namespace RockPaperScissors.Db.Repository.Interface;

public interface IMatchRepository
{
    Task<List<Match?>> GetAllMatches(CancellationToken cancellationToken);
    Task<MatchResponse> CreateMatch(Match match, CancellationToken cancellationToken);
    
    Task<Match?> GetMatchById(Guid matchId, CancellationToken cancellationToken);
}