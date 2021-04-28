using System;
using System.Collections.Generic;
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

namespace Guth.OpenTrivia.Firebase
{
    public class TriviaRealtimeDB
    {
        private const string GAMES = "Games";
        private const string PLAYERS = "Players";
        private const string QUESTIONS = "Questions";
        private const string CONNECTION_CODES = "ConnectionCodes";

        public FirebaseClient DbClient { get; private set; }
        public TriviaRealtimeDB(FirebaseClient client)
        {
            DbClient = client;
        }

        public async Task<string> CreateGame(string playerId)
        {
            CancellationToken cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;
            ConnectionCode connection = await GenerateGameConnection(cancellationToken);

            FirebaseObject<Game> created = await GetChild(GAMES)
                .PostAsync(new Game 
                { 
                    HostPlayerId = playerId, 
                    ConnectionCode = connection.Code 
                });

            return created.Key;
        }

        public async Task<Game> UpdateGameOptions(string gameId, QuestionOptions options)
        {
            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();

            game.QuestionOptions = options;

            await GetChild(GAMES, gameId).PatchAsync(game);
            return game;
        }

        public async Task<string> CreatePlayer(string name)
        {
            FirebaseObject<Player> player = await GetChild(PLAYERS).PostAsync(new Player { Name = name });
            return player.Key;
        }

        public async Task AddPlayerToGame(string connectionCode, string playerId)
        {
            ConnectionCode code = await GetChild(CONNECTION_CODES, connectionCode).OnceSingleAsync<ConnectionCode>();
            Game game = await GetChild(GAMES, code.GameId).OnceSingleAsync<Game>();
            game.Players.Add(playerId);
            await GetChild(GAMES, code.GameId).PatchAsync(game);
        }

        public async Task<Game> StartNextRound(string gameId)
        {
            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();
            if (game.Questions.Count < 1)
            {
                game.State = GameState.Complete;
            }
            else
            {
                TriviaQuestion nextQuestion = game.Questions.Pop();
                game.Rounds.Add(new TriviaRound(nextQuestion));
                game.State = GameState.RoundStart;
            }
            await GetChild(GAMES, gameId).PatchAsync(game);
            return game;
        }

        public async Task EndRound(string gameId)
        {
            Game game = await GetChild(GAMES, gameId).OnceSingleAsync<Game>();
            game.State = GameState.RoundEnd;
            await GetChild(GAMES, gameId).PatchAsync(game);
        }

        public async Task<ConnectionCode> GenerateGameConnection(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string code = GetRandomConnectionCode();
                ConnectionCode active = await GetChild(CONNECTION_CODES, code).OnceSingleAsync<ConnectionCode>();
                if (active == null)
                {
                    var gameConnection = new ConnectionCode(code);
                    await GetChild(CONNECTION_CODES, code).PutAsync(gameConnection);
                    return gameConnection;
                }
            }
            throw new Exception("Failed to generate a connection code");
        }

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

        public void SubscribeToGame(string gameId, Action<FirebaseEvent<Game>> next)
        {
            GetChild(GAMES)
                .AsObservable<Game>()
                .Where(g => g.Key == gameId)
                .Subscribe(next);
        }

        private ChildQuery GetChild(params string[] segments)
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
