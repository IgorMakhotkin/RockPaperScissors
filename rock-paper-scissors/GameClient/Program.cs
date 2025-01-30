using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using RockPaperScissors;
using RockPaperScissors.Model.Entity;

namespace GameClient
{
    class Program
    {
        public static string UserId;
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
            {
                HttpClient = httpClient
            });
            var client = new GameService.GameServiceClient(channel);

            await CreateUser(client);

            while (true)
            {
                Console.WriteLine("Введите команду:");
                Console.WriteLine("1 - Просмотр баланса");
                Console.WriteLine("2 - Получение списка игр");
                Console.WriteLine("3 - Подключение к игре по её ID");
                Console.WriteLine("4 - Получить результат игры");
                Console.WriteLine("q - Выход");

                var command = Console.ReadLine();

                switch (command)
                {
                    case "1":
                        await GetBalance(client);
                        break;
                    case "2":
                        await ListGames(client);
                        break;
                    case "3":
                        await JoinGame(client);
                        break;
                    case "4":
                        await GetGameResult(client);
                        break;
                    case "q":
                        return;
                    default:
                        Console.WriteLine("Неверная команда.");
                        break;
                }
            }
        }

        private static async Task CreateUser(GameService.GameServiceClient client)
        {
            Console.WriteLine("Введите имя пользователя:");
            var username = Console.ReadLine();

            Console.WriteLine("Введите баланс пользователя:");
            var balance = double.Parse(Console.ReadLine()!);

            var request = new CreateUserRequest() { Username = username, Balance = balance};
            var response = await client.CreateUserAsync(request);
            UserId = response.UserId;
            Console.WriteLine($"Id пользователя {response.UserId}");
        }

        private static async Task GetBalance(GameService.GameServiceClient client)
        {
            var request = new BalanceRequest()
            {
                UserId = UserId
            };
            if (string.IsNullOrEmpty(UserId))
            {
                Console.WriteLine("Введите ID пользователя:");
                request.UserId = Console.ReadLine();
            }

            var response = await client.GetBalanceAsync(request);

            Console.WriteLine($"Баланс пользователя {request.UserId}: {response.Balance}");
        }

        private static async Task ListGames(GameService.GameServiceClient client)
        {
            var request = new ListGamesRequest();
            var response = await client.ListGamesAsync(request);

            Console.WriteLine("Список игр:");
            foreach (var game in response.Games)
            {
                Console.WriteLine($"ID: {game.GameId}, Ставка: {game.BetAmount}, Ожидание: {game.IsWaiting}");
            }
        }

        private static async Task JoinGame(GameService.GameServiceClient client)
        {
            Console.WriteLine("Введите ID игры:");
            var gameId = Console.ReadLine();

            var request = new JoinGameRequest { GameId = gameId, UserId = UserId };
            var response = await client.JoinGameAsync(request);

            Console.WriteLine($"Результат подключения: {response.Success} ({response.Message})");
            await MakeMove(client, gameId);
        }

        private static async Task MakeMove(GameService.GameServiceClient client, string gameId)
        {
            
            Console.WriteLine("Введите ход (К, Н или Б):");
            var move = Console.ReadLine();

            var request = new MakeMoveRequest { UserId = UserId, MatchId = gameId, Move = move };
            var response = await client.MakeMoveAsync(request);

            Console.WriteLine($"Результат хода: {response.Result}");
        }

        private static async Task GetGameResult(GameService.GameServiceClient client)
        {

            Console.WriteLine("Введите ID игры:");
            var gameId = Console.ReadLine();

            var request = new GetGameResultRequest { UserId = UserId, GameId = gameId };
            var response = await client.GetGameResultAsync(request);

            Console.WriteLine($"Результат игры: {gameId} - {response.Result}");
        }
    }
}