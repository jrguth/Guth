using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;
using Orleans;
using Orleans.Streams;
using Guth.OpenTrivia.GrainInterfaces;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.Abstractions;

namespace Guth.OpenTrivia.Grains
{
    public class GameObserverGrain : Grain<GameStatus>
    { 
        public async Task 
    }
    public class GameSessionGrain : Grain<Game>, IGameSessionGrain
    {
        private readonly IOpenTriviaClient _openTriviaClient;
        private readonly List<Player> _players = new List<Player>();
        private string _openTriviaSessionToken;
        private TriviaQuestion _currentQuestion;
        private IDisposable _roundTimer;
        private Stopwatch _roundStopWatch;
        private Dictionary<Guid, string> _currentQuestionAnswers;

        private IAsyncStream<TriviaQuestion> _questionStream;

        public GameSessionGrain(IOpenTriviaClient openTriviaClient)
        {
            _openTriviaClient = openTriviaClient;
            _currentQuestionAnswers = new Dictionary<Guid, string>();
            _roundStopWatch = new Stopwatch();
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            _questionStream = GetStreamProvider(Constants.QuestionStreamProvider)
                .GetStream<TriviaQuestion>(this.GetPrimaryKey(), Constants.QuestionStreamNamespace);
            _openTriviaSessionToken = await _openTriviaClient.GetSessionToken();
        }

        public Task AddPlayer(Player player)
        {
            _players.Add(player);
            State.PlayerResults.TryAdd(player.Key, 0);
            return Task.CompletedTask;
        }

        public Task<bool> AnswerCurrentQuestion(Player player, string answer)
        {
            if (answer == _currentQuestion.CorrectAnswer)
            {
                State.PlayerResults[player.Key] += 1;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public async Task GetQuestions(Action<QuestionOptions> configureOptions)
        {
            ImmutableList<TriviaQuestion> questions = await _openTriviaClient.GetTriviaQuestions(configureOptions, _openTriviaSessionToken);
            State.Questions = ImmutableStack.Create(questions.ToArray());
        }

        public Task ConfigureSecondsPerRound(int seconds)
        {
            State.MaxSecondsPerRound = seconds;
            return Task.CompletedTask;
        }

        public Task<ImmutableList<Player>> GetPlayers()
        {
            return Task.FromResult(_players.ToImmutableList());
        }

        public async Task PublishNextQuestion()
        {
            _currentQuestionAnswers.Clear();
            if (State.Questions.IsEmpty)
            {
                // publish results
            }
            State.Questions.Pop(out _currentQuestion);
            await _questionStream.OnNextAsync(_currentQuestion);
            _roundStopWatch.Start();
            _roundTimer = RegisterTimer(CheckRoundComplete, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        public Task RemovePlayer(Player player)
        {
            _players.Remove(player);
            return Task.CompletedTask;
        }

        private Task CheckRoundComplete(object _)
        {
            if (_players.All(p => _currentQuestionAnswers.ContainsKey(p.Key)) || 
                _roundStopWatch.Elapsed.TotalSeconds >= State.MaxSecondsPerRound)
            {
                _roundTimer.Dispose();
                _roundTimer = null;
                _roundStopWatch.Reset();
            }
            return Task.CompletedTask;
        }
    }
}
