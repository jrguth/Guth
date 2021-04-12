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
            _subscriptionManager = new ObserverSubscriptionManager<IGameObserver>();
            _players = ImmutableHashSet.Create<IPlayerGrain>();
            _openTriviaSessionToken = await _openTriviaClient.GetSessionToken();
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
            ImmutableList<TriviaQuestion> questions = await _openTriviaClient.GetTriviaQuestions(_questionOptions, _openTriviaSessionToken);
            State.Questions = ImmutableStack.Create(questions.ToArray());
            await _subscriptionManager.Notify(observer => observer.UpdateGame(State));
        }

        public async Task AddPlayer(IPlayerGrain player)
        {
            Player p = await player.GetPlayer();
            State.Players.Add(p);
            _subscriptionManager.AddSubscriber(player);
            await _subscriptionManager.Notify(observer => observer.UpdateGame(State));
        }

        public async Task RemovePlayer(IPlayerGrain player)
        {
            Player p = await player.GetPlayer();
            State.Players.Remove(p);
            _subscriptionManager.RemoveSubscriber(player);
            await _subscriptionManager.Notify(observer => observer.UpdateGame(State));
        }

        public async Task NextRound(int bufferSeconds = 3)
        {
            if (!State.Completed)
            {
                State.RoundNumber++;
                State.Questions.Pop(out TriviaQuestion question); 
                var choices = question.IncorrectAnswers.Insert(new Random().Next(0, question.IncorrectAnswers.Length), question.CorrectAnswer);
                var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(_gameOptions.SecondsPerRound + bufferSeconds));
                _logger.LogInformation("Asking next question: '{0}'", question.Question);
                await Task.WhenAll(_players.Select(player => new Task(async () =>
                {
                    string answer = await player.AnswerQuestion(new Round(question.Question, bufferSeconds, choices), cancellation.Token);
                    _logger.LogInformation("Received answer from player {0}: ", player.GetPrimaryKey(), answer == question.CorrectAnswer ? "Correct!" : "Incorrect!");

                    if (answer == question.CorrectAnswer)
                    {
                        State.Players.First(p => p.Key == player.GetPrimaryKey()).Points += 1;
                    }
                })));
                await _subscriptionManager.Notify(observer => observer.UpdateGame(State));
            }
           
        }
    }
}
