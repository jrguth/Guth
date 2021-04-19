using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using Guth.OpenTrivia.GrainInterfaces;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.Abstractions;

namespace Guth.OpenTrivia.Grains
{
    [Reentrant]
    public class GameGrain : Grain<Game>, IGameGrain
    {
        private readonly IOpenTriviaClient _openTriviaClient;
        private readonly ILogger<IGameGrain> _logger;
        private string _openTriviaSessionToken;

        private HashSet<IPlayerGrain> _players;

        private GameOptions _gameOptions;
        private QuestionOptions _questionOptions;

        private int _roundNumber = 0;

        public GameGrain(IOpenTriviaClient openTriviaClient, ILogger<IGameGrain> logger)
        {
            _openTriviaClient = openTriviaClient;
            _logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            State.Key = this.GetPrimaryKey();
            _logger.LogInformation("Activating new Game grain with id {0}", State.Key);
            _players = new HashSet<IPlayerGrain>();
            _openTriviaSessionToken = await _openTriviaClient.GetSessionToken();
            _logger.LogInformation("Obtained open trivia session token: {0}", _openTriviaSessionToken);
            await base.OnActivateAsync();
        }

        public Task ConfigureOptions(GameOptions gameOptions, QuestionOptions questionOptions)
        {
            _gameOptions = gameOptions;
            _questionOptions = questionOptions;
            return Task.CompletedTask;
        }

        public async Task Play()
        {
            _logger.LogInformation("Starting game...");
            await CacheTriviaQuestions();
            while (State.Questions.Count > 0)
            {
                await StartNextRound();

            }
        }

        public async Task AddPlayer(IPlayerGrain player)
        {
            Player p = await player.GetPlayer();
            State.Players.Add(p);
            _players.Add(player);
        }

        public async Task RemovePlayer(IPlayerGrain player)
        {
            Player p = await player.GetPlayer();
            State.Players.Remove(p);
            _players.Remove(player);
        }

        public Task SubmitAnswer(TriviaAnswer answer)
        {
            State.Answers[State.CurrentQuestion].Add(answer);
            return Task.CompletedTask;
        }

        private async Task AllAnswersReceivedOrTimeout()
        {
            Task.
        }

        private async Task StartNextRound()
        {
            var tasks = new List<Task>();
            State.CurrentQuestion = State.Questions.Pop();
            State.Answers.Add(State.CurrentQuestion, new List<TriviaAnswer>());
            var round = new TriviaRound
            {
                Question = State.CurrentQuestion.Question,
                Choices = State.CurrentQuestion.IncorrectAnswers.Insert(new Random().Next(State.CurrentQuestion.IncorrectAnswers.Length), State.CurrentQuestion.CorrectAnswer),
                CountdownSeconds = _gameOptions.SecondsPerRound,
                RoundNumber = ++_roundNumber
            };
            foreach (IPlayerGrain player in _players)
            {
                tasks.Add(Task.Factory.StartNew(async () => await player.SetNextRound(round)));
            }
            await Task.WhenAll(tasks);
        }

        private async Task AcceptPlayerAnswer(TriviaRound round, IPlayerGrain player, CancellationToken cancellationToken)
        {
            string answer = await player.AnswerRoundQuestion(round, cancellationToken);

            if (answer == State.CurrentQuestion.CorrectAnswer)
            {

            }
        }

        private async Task CacheTriviaQuestions()
        {
            ImmutableList<TriviaQuestion> questions = await _openTriviaClient.GetTriviaQuestions(_questionOptions, _openTriviaSessionToken);
            State.Questions = new Stack<TriviaQuestion>(questions.ToArray());
            _logger.LogInformation("Cached {0} trivia questions", questions.Count);
        }
    }
}
