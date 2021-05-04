using System;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Enums;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.FirebaseDB
{
    public class TriviaRealtimeDB
    {
        public const string GAMES = "Games";
        public const string PLAYERS = "Players";
        public const string QUESTIONS = "Questions";
        public const string CONNECTION_CODES = "ConnectionCodes";
        public const string ROUNDS = "Rounds";

        public FirebaseClient DbClient { get; private set; }
        public TriviaRealtimeDB(FirebaseClient client)
        {
            DbClient = client;
        }

        public async Task<Game> CreateGame(string connectionCode, QuestionOptions questionOptions = null)
        {
            ConnectionCode connection = await GetChild(CONNECTION_CODES, connectionCode).OnceSingleAsync<ConnectionCode>();
            var id = Guid.NewGuid().ToString();
            var game = new Game
            {
                Id = id,
                ConnectionCode = connection.Code,
                QuestionOptions = questionOptions
            };
            await GetChild(GAMES, id).PutAsync(game);
            connection.GameId = id;
            await GetChild(CONNECTION_CODES, connectionCode).PatchAsync(connection);
            return game;
        }

        public async Task<Game> GetGame(string gameId)
            => await GetChild(GAMES, gameId).OnceSingleAsync<Game>();

        public async Task<Game> UpdateGameOptions(string gameId, QuestionOptions options)
        {
            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();

            game.QuestionOptions = options;

            await GetChild(GAMES, gameId).PatchAsync(game);
            return game;
        }

        public async Task<Player> CreatePlayer(string name)
        {
            FirebaseObject<Player> created = await GetChild(PLAYERS).PostAsync(new Player { Name = name });
            Player player = created.Object;
            player.Id = created.Key;
            return player;
        }

        public async Task<Player> GetPlayer(string playerId)
            => await GetChild(PLAYERS, playerId).OnceSingleAsync<Player>();

        public async Task AddPlayerToGame(string gameId, string playerId)
        {
            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();
            game.Players.Add(playerId);
            await GetChild(GAMES, gameId).PatchAsync(game);
        }

        public async Task<Game> StartNextRound(string gameId)
        {
            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();
            if (game.Questions.Count < 1)
            {
                game.State = GameState.Complete;
                await GetChild(GAMES, gameId).PatchAsync(game);
            }
            else
            {
                TriviaQuestion nextQuestion = game.Questions.Pop();
                var round = new TriviaRound(nextQuestion);
                game.Rounds.Add(round);
                game.State = GameState.RoundStart;
                await GetChild(GAMES, gameId).PatchAsync(game);
                await GetChild(GAMES, gameId, ROUNDS).PostAsync(round);
            }
            return game;
        }

        public async Task EndRound(string gameId)
        {
            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();
            game.State = GameState.RoundEnd;
            await GetChild(GAMES, gameId).PatchAsync(game);
        }

        public async Task<ConnectionCode> GenerateConnectionCode(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string code = GetRandomConnectionCode();
                ConnectionCode active = await GetChild(CONNECTION_CODES, code).OnceSingleAsync<ConnectionCode>();
                if (active == null)
                {
                    var gameConnection = new ConnectionCode { Code = code };
                    await GetChild(CONNECTION_CODES, code).PutAsync(gameConnection);
                    return gameConnection;
                }
            }
            throw new Exception("Failed to generate a connection code");
        }

        public async Task<ConnectionCode> GetConnectionCode(string code)
            => await GetChild(CONNECTION_CODES, code).OnceSingleAsync<ConnectionCode>();

        public async Task<bool> AnswerQuestion(string gameId, string playerId, string answer)
        {
            var triviaAnswer = new TriviaAnswer
            {
                Answer = answer,
                PlayerId = playerId
            };

            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();
            var currentRound = game.Rounds.LastOrDefault();
            currentRound?.Answers.Add(triviaAnswer);
            await GetChild(GAMES, gameId).PatchAsync(game);
            return answer == currentRound.Question.CorrectAnswer;
        }

        public void SubscribeToGame(string gameId, Action<Game> next)
        {
            GetChild(GAMES)
                .AsObservable<Game>()
                .Where(g => g.Key == gameId)
                .Subscribe(e => next(e.Object));
        }

        public ChildQuery GetChild(params string[] segments)
        {
            ChildQuery query = null;
            foreach (var segment in segments)
            {
                query = query == null
                    ? DbClient.Child(segment)
                    : query.Child(segment);
            }
            return query;
        }

        private string GetRandomConnectionCode()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable
                .Repeat(chars, 4)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
    }
}
