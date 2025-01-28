using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Matches;

namespace RockPaperScissors.Db.Repository.Interface;

public interface IMatchHistoryRepository
{
    Task<MatchHistory?> GetMatchHistoryById(Guid matchId, CancellationToken cancellationToken);
    Task<List<MatchHistory?>> GetAllMatches(CancellationToken cancellationToken);
    Task<MatchResponse> CreateMatch(Match match, CancellationToken cancellationToken);
    Task UpdateMatchHistory(MatchHistory matchHistory, CancellationToken cancellationToken);
}