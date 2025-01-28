using RockPaperScissors.Db.Repository.Interface;
using RockPaperScissors.Interface;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Matches;

namespace RockPaperScissors.Services;

public class MatchService : IMatchService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<MatchService> _logger;
    private readonly IMatchHistoryRepository _matchHistoryRepository;
    
    public MatchService(IUserRepository userRepository, ILogger<MatchService> logger, IMatchHistoryRepository matchHistoryRepository)
    {
        _userRepository = userRepository;
        _logger = logger;
        _matchHistoryRepository = matchHistoryRepository;
    }
    
    
    public async Task<MatchResponse> CreateMatch(Match match, CancellationToken cancellationToken)
    {
        var newMatch = new Match()
        {
            MatchBet = match.MatchBet
        };
        
        return await _matchHistoryRepository.CreateMatch(newMatch, cancellationToken);
    }
}