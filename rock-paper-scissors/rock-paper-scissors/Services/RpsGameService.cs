using System.Data.Common;
using Google.Protobuf.Collections;
using Grpc.Core;
using Mapster;
using RockPaperScissors.Db.Repository.Interface;
using RockPaperScissors.Interface;
using RockPaperScissors.Model;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Transfer.Dto;

namespace RockPaperScissors.Services;

public class RpsGameService : GameService.GameServiceBase
{
    
    private readonly ILogger<RpsGameService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IMatchHistoryRepository _matchHistoryRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly ITransactionsService _transactionsService;

    public RpsGameService(IUserRepository userRepository, ILogger<RpsGameService> logger, IMatchHistoryRepository matchHistoryRepository, IMatchRepository matchRepository, ITransactionsService transactionsService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _matchHistoryRepository = matchHistoryRepository;
        _matchRepository = matchRepository;
        _transactionsService = transactionsService;
    }
    
    public override async Task<ListGamesResponse> ListGames(ListGamesRequest request, ServerCallContext context)
    {
        var matchList = await _matchRepository.GetAllMatches(context.CancellationToken);
        var gameList = new RepeatedField<Game>();
        foreach (var match in matchList)
        {
            var game = new Game
            {
                GameId = match.MatchId.ToString(),
                BetAmount = (double)match.MatchBet,
                IsWaiting = match.Status is MatchStatus.NotStarted ? true : false,

            };
            gameList.Add(game);
        }

        return new ListGamesResponse()
        {
            Games = { gameList }
        };
    }
    
    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var user = await _userRepository.CreateUser(request.Username, context.CancellationToken);
        var balance = await _userRepository.UpdateUserBalance(user.UserId, (decimal)request.Balance, context.CancellationToken);
        return new CreateUserResponse()
        {
            UserId = user.UserId.ToString()
        };
    }
    
    public override async Task<BalanceResponse> GetBalance(BalanceRequest request, ServerCallContext context)
    {
        var balance = await _userRepository.GetUserBalanceById(Guid.Parse(request.UserId), context.CancellationToken);
        return new BalanceResponse()
        {
            Balance = (double)balance
        };
    }
    
    public override async Task<JoinGameResponse> JoinGame(JoinGameRequest request, ServerCallContext context)
    {
        var newMatch = await _matchRepository.GetMatchById(Guid.Parse(request.GameId), context.CancellationToken);
        var matchHistory = await _matchHistoryRepository.GetMatchHistoryById(newMatch.MatchId, context.CancellationToken);
        var response = new JoinGameResponse();
        if (matchHistory is not null)
        {
            switch (matchHistory.Status)
            {
                case MatchStatus.Waiting:
                    await JoinInGame(matchHistory, Guid.Parse(request.UserId), context.CancellationToken);
                    response.Success = true;
                    response.Message = "Game joined successfully.";
                    return response;
            }
        }

        var newMathHistory = new MatchHistory()
        {
            MatchId = Guid.Parse(request.GameId),
            CreatedAt = DateTime.UtcNow,
            AmountBet = newMatch.MatchBet,
        };
        await JoinInGame(newMathHistory, Guid.Parse(request.UserId), context.CancellationToken);
        response.Success = true;
        response.Message = "Game joined successfully.";
        return response;
    }

    public override async Task<GetGameResultResponse> GetGameResult(GetGameResultRequest request, ServerCallContext context)
    { 
        var matchHistory = await _matchHistoryRepository.GetMatchHistoryById(Guid.Parse(request.GameId), context.CancellationToken);
        var response = new GetGameResultResponse();
        if (matchHistory is not null)
        {
            if (!string.IsNullOrEmpty(matchHistory.PlayerOneMove) && !string.IsNullOrEmpty(matchHistory.PlayerOneMove))
            {
                var p1 = matchHistory.PlayerOneMove; 
                var p2 = matchHistory.PlayerTwoMove;
                
                if (p1 == p2)
                {
                    matchHistory.Status = MatchStatus.Draw;
                    response.Result = "Ничья"; 
                }
                else if (
                    (p1 == "к" && p2 == "н") ||
                    (p1 == "н" && p2 == "б") ||
                    (p1 == "б" && p2 == "к")
                )
                {
                    matchHistory.Status = MatchStatus.PlayerOneWin;
                    response.Result = "Выиграл первый игрок";
                }
                else
                {
                    matchHistory.Status = MatchStatus.PlayerTwoWin;
                    response.Result = "Выиграл второй игрок";
                }
            }
            else
            {
                matchHistory.Status = MatchStatus.NotStarted;
                response.Result = "Один из игроков не сделал свой ход";
            }
        }
        await _matchHistoryRepository.UpdateMatchHistory(matchHistory, context.CancellationToken);
        await TransferMoneyToWinner(matchHistory, Guid.Parse(request.GameId), context.CancellationToken);
        return response;
    }

    private async Task TransferMoneyToWinner(MatchHistory matchHistory, Guid matchId, CancellationToken cancellationToken)
    {
        var transaction = new Transaction();
        if (matchHistory.Status == MatchStatus.PlayerOneWin)
        {

            transaction.FromUserId = matchHistory.PlayerTwoId;
            transaction.ToUserId = matchHistory.PlayerOneId;
            transaction.Amount = matchHistory.AmountBet;
            transaction.GameId = matchHistory.MatchId;
        }
        else if (matchHistory.Status == MatchStatus.PlayerTwoWin)
        {
            transaction.FromUserId = matchHistory.PlayerOneId;
            transaction.ToUserId = matchHistory.PlayerTwoId;
            transaction.Amount = matchHistory.AmountBet;
            transaction.GameId = matchHistory.MatchId;
        }
        await _transactionsService.Transfer(transaction, cancellationToken);
    }
    public override async Task<MakeMoveResponse> MakeMove(MakeMoveRequest request, ServerCallContext context)
    {
        var matchHistory = await _matchHistoryRepository.GetMatchHistoryById(Guid.Parse(request.MatchId), context.CancellationToken);
        if (matchHistory is not null)
        {
            switch (matchHistory.Status)
            {
                case MatchStatus.Waiting:
                    if (matchHistory.PlayerOneId == Guid.Parse(request.UserId))
                    {
                        matchHistory.PlayerOneMove = request.Move;
                    }
                    if (matchHistory.PlayerTwoId == Guid.Parse(request.UserId))
                    {
                        matchHistory.PlayerTwoMove = request.Move;
                    }
                    break;
            }
        }

        if (!string.IsNullOrEmpty(matchHistory.PlayerOneMove) && !string.IsNullOrEmpty(matchHistory.PlayerTwoMove))
        {
            matchHistory.Status = MatchStatus.Comparison;
        }
        await _matchHistoryRepository.UpdateMatchHistory(matchHistory, context.CancellationToken);
        return new MakeMoveResponse()
        {
            Result = request.Move
        };
    }

    private MatchHistory SetupUserId(MatchHistory match, Guid userId)
    {
        if (match.PlayerOneId == null)
        {
            match.PlayerOneId = userId;
        }
        else if (match.PlayerTwoId == null && match.PlayerOneId != userId)
        {
            match.PlayerTwoId = userId;
        }
        return match;
    }
    private async Task JoinInGame(MatchHistory match, Guid userId, CancellationToken cancellationToken)
    {
        if (match.Status is MatchStatus.NotStarted || match.Status is MatchStatus.Waiting)
        {
            match = SetupUserId(match, userId);
        }

        if (match.PlayerTwoId is not null || match.PlayerOneId is not null)
        {
            match.Status = MatchStatus.Waiting;
        }
        await _matchHistoryRepository.UpdateMatchHistory(match, cancellationToken);
    }
}