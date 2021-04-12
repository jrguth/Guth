using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Guth.OpenTrivia.GrainInterfaces;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.Abstractions;

namespace Guth.OpenTrivia.Grains
{
    public class GameGrain : Grain<Game>, IGameGrain
    {
        private readonly IOpenTriviaClient _openTriviaClient;
        private readonly ILogger<IGameGrain> _logger;
        private string _openTriviaSessionToken;

        private ObserverSubscriptionManager<IGameObserver> _subscriptionManager;

        private ImmutableHashSet<IPlayerGrain> _players;

        private GameOptions _gameOptions;
        private QuestionOptions _questionOptions;

        public GameGrain(IOpenTriviaClient openTriviaClient, ILogger<IGameGrain> logger)
        {
            _openTriviaClient = openTriviaClient;
            _logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            _logger.LogInformation("Activating new Game grain with id {0}", this.GetPrimaryKey());
            _subscriptionManager = new ObserverSubscriptionManager<IGameObserver>();
            _players = ImmutableHashSet.Create<IPlayerGrain>();
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

        public async Task Start()
        {
            _logger.LogInformation("Starting game...");
            ImmutableList<TriviaQuestion> questions = await _openTriviaClient.GetTriviaQuestions(_questionOptions, _openTriviaSessionToken);
            State.Questions = new Stack<TriviaQuestion>(questions.ToArray());
            _logger.LogInformation("Cached {0} trivia questions", questions.Count);
            await _subscriptionManager.Notify(observer => observer.UpdateGame(State));
        }

        public async Task AddPlayer(IPlayerGrain player)
        {
            Player p = await player.GetPlayer();
            State.Players.Add(p);
            _players = _players.Add(player);
            await _subscriptionManager.Notify(observer => observer.UpdateGame(State));
        }

        public async Task RemovePlayer(IPlayerGrain player)
        {
            Player p = await player.GetPlayer();
            State.Players.Remove(p);
            _players = _players.Remove(player);
            _subscriptionManager.RemoveSubscriber(player);
            await _subscriptionManager.Notify(observer => observer.UpdateGame(State));
        }

        public async Task<bool> StartNextRound(int bufferSeconds = 3)
        {
            if (!State.IsComplete)
            {
                State.RoundNumber = State.RoundNumber++;
                _logger.LogInformation("Starting round {0} of {1}", State.RoundNumber, State.TotalRounds);
                TriviaQuestion question = State.Questions.Pop();
                _logger.LogInformation("Next question: {0}", question.Question);

                var choices = question.IncorrectAnswers.Insert(new Random().Next(0, question.IncorrectAnswers.Length), question.CorrectAnswer);
                var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(_gameOptions.SecondsPerRound + bufferSeconds));
                _logger.LogInformation("Number of players: {0}", _players.Count);
                IEnumerable<Task> tasks = _players.Select(player =>
                {
                    var task = new Task(async () =>
                    {
                        _logger.LogInformation("Asking question to player {0}", player.GetPrimaryKey());
                        string answer = await player.AnswerQuestion(new Round(question.Question, bufferSeconds, choices));
                        _logger.LogInformation("Received answer from player {0}: {1}", player.GetPrimaryKey(), answer);

                        if (answer == question.CorrectAnswer)
                        {
                            State.Players.First(p => p.Key == player.GetPrimaryKey()).Points += 1;
                        }
                    });
                    task.Start();
                    return task;
                });

                await Task.WhenAll(tasks);
            }
            return State.IsComplete;
        }
    }
}
