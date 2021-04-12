using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Guth.OpenTrivia.GrainInterfaces;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.Grains
{
    public class PlayerGrain : Grain<Player>, IPlayerGrain
    {
        private ILogger<IPlayerGrain> _logger;
        private IGameGrain _currentGame;
        private IGrainFactory _grainFactory;

        private Game _game;

        public PlayerGrain(ILogger<IPlayerGrain> logger, IGrainFactory grainFactory)
        {
            _logger = logger;
            _grainFactory = grainFactory;
        }

        public override async Task OnActivateAsync()
        {
            State.Key = this.GetPrimaryKey();
            await base.OnActivateAsync();
        }

        public async Task<Player> GetPlayer() => await Task.FromResult(State);
        public async Task<Game> GetCurrentGame() => await Task.FromResult(_game);
        public Task SetName (string name)
        {
            State.Name = name;
            return Task.CompletedTask;
        }

        public async Task<IGameGrain> CreateGame(GameOptions gameOptions, QuestionOptions questionOptions)
        {
            _currentGame = _grainFactory.GetGrain<IGameGrain>(Guid.NewGuid());
            await JoinGame(_currentGame);
            await _currentGame.ConfigureOptions(gameOptions, questionOptions);
            return await Task.FromResult(_currentGame);
        }

        public async Task JoinGame(IGameGrain game)
        {
            await game.AddPlayer(this);
            _currentGame = game;
            _logger.LogInformation("Player {0} has joined game {1}", this.GetPrimaryKey(), _currentGame.GetPrimaryKey());
        }

        public async Task LeaveGame(IGameGrain game)
        {
            if (_currentGame != null)
            {
                await game.RemovePlayer(this);
                _currentGame = null;
            }
        }

        public async Task<string> AnswerQuestion(Round round)
        {
            _logger.LogInformation("Player {0} answering next question: {1}", this.GetPrimaryKey(), round.Question);
            int selection = new Random().Next(0, round.Choices.Length - 1);
            string choice = round.Choices[selection];
            _logger.LogInformation("Player {0} answers: {1}", this.GetPrimaryKey(), choice);
            return await Task.FromResult(choice);
        }

        public void UpdateGame(Game game)
        {
            _game = game;
            Console.WriteLine("Game updated!");
            _logger.LogInformation("Game updated for player {0}", this.GetPrimaryKey());
        }
    }
}
