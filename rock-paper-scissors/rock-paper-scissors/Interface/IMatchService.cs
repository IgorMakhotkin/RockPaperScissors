using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Matches;
using RockPaperScissors.Model.Matches.Dto;

namespace RockPaperScissors.Interface;

public interface IMatchService
{
    Task<MatchResponse> CreateMatch(Match match, CancellationToken cancellationToken);
}