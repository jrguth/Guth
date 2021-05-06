using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public const string RESULTS = "Results";

        public FirebaseClient DbClient { get; private set; }
        public TriviaRealtimeDB(FirebaseClient client)
        {
            DbClient = client;
        }

        public async Task<Game> CreateGame(string connectionCode, string hostId = null, QuestionOptions questionOptions = null)
        {
            ConnectionCode connection = await GetChild(CONNECTION_CODES, connectionCode).OnceSingleAsync<ConnectionCode>();
            var id = Guid.NewGuid().ToString();
            var game = new Game
            {
                Id = id,
                HostPlayerId = hostId,
                ConnectionCode = connection.Code,
                QuestionOptions = questionOptions,
                Players = string.IsNullOrWhiteSpace(hostId) ? null : new List<string> { hostId }
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

        public async Task StartGame(string gameId, ImmutableList<TriviaQuestion> questions)
        {
            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();
            game.Questions = new Stack<TriviaQuestion>(questions);
            game.State = GameState.Started;
            await GetChild(GAMES, gameId).PatchAsync(game);
        }

        public async Task<TriviaRound> GetCurrentRound(string gameId)
        {
            Game game = await GetGame(gameId);
            return new TriviaRound
            {
                RoundNumber = game.RoundNumber,
                Question = game.Questions.ElementAtOrDefault(game.RoundNumber - 1)
            };
        }

        public async Task<Player> CreatePlayer(string name)
        {
            var id = Guid.NewGuid().ToString();
            var player = new Player
            {
                Id = id,
                Name = name
            };
            await GetChild(PLAYERS, id).PutAsync(player);
            return player;
        }

        public async Task<Player> GetPlayer(string playerId)
            => await GetChild(PLAYERS, playerId).OnceSingleAsync<Player>();

        public async Task AddPlayerToGame(string gameId, string playerId)
        {
            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();
            game.Players.Add(playerId);
            game.Results.Add(new PlayerScore
            {
                PlayerId = playerId,
                Points = 0
            });
            await GetChild(GAMES, gameId).PatchAsync(game);
        }

        public async Task<ICollection<Player>> GetGamePlayers(string gameId)
        {
            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();
            return await Task.WhenAll(game.Players.Select(async (playerId) => await GetChild(PLAYERS, playerId).OnceSingleAsync<Player>()));
        }

        public async Task<Game> StartNextRound(string gameId)
        {
            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();
            if (game.RoundNumber >= game.Questions.Count)
            {
                game.State = GameState.Complete;
                await GetChild(GAMES, gameId).PatchAsync(game);
            }
            else
            {
                TriviaQuestion nextQuestion = game.Questions.ElementAt(++game.RoundNumber);
                var round = new TriviaRound
                {
                    RoundNumber = game.RoundNumber,
                    Question = nextQuestion
                };
                game.Rounds.Add(round);
                game.State = GameState.RoundBegin;
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
            => await GetChild(CONNECTION_CODES, code?.ToUpper()).OnceSingleAsync<ConnectionCode>();

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
            bool correct = answer == currentRound.Question.CorrectAnswer;
            if (correct)
            {
                game.Results.FirstOrDefault(r => r.PlayerId == playerId).Points++;
            }
            await GetChild(GAMES, gameId).PatchAsync(game);
            return correct;
        }

        public async Task CompleteGame(string gameId)
        {
            Game game = await GetGame(gameId);
            ICollection<Player> players = await GetGamePlayers(gameId);
            foreach (var player in players)
            {

            }
        }

        public void OnGameUpdate(string gameId, Action<Game> next)
        {
            GetChild(GAMES)
                .AsObservable<Game>()
                .Where(g => g.Key == gameId)
                .Subscribe(e => next(e.Object));
        }

        public void OnPlayerAddedOrRemoved(string gameId, Action<ICollection<Player>> next)
        {
            GetChild(GAMES, gameId, PLAYERS)
                .AsObservable<string>()
                .Subscribe(async (e) =>
                {
                    ICollection<Player> players = await GetGamePlayers(gameId);
                    next(players);
                });
        }

        public void OnRoundUpdate(string gameId, Action<TriviaRound> next)
        {
            GetChild(GAMES, gameId, ROUNDS)
                .AsObservable<TriviaRound>()
                .Subscribe(e =>
                {
                    if (e.Object != null)
                    {
                        next(e.Object);
                    }
                });
        }

        public void OnScoresUpdated(string gameId, Action<ICollection<PlayerScore>> next)
        {
            GetChild(GAMES, gameId, ROUNDS)
                .AsObservable<PlayerScore>()
                .Subscribe(async (e) =>
                {
                    IEnumerable<PlayerScore> scores = (await GetGame(gameId)).Results;
                    next(scores.ToList());
                });
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
