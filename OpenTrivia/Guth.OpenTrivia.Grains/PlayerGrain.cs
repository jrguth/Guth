using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using Guth.OpenTrivia.GrainInterfaces;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.Grains
{
    public class PlayerGrain : Grain<Player>, IPlayerGrain
    {
        private IGameSessionGrain _game;

        public Task<IGameSessionGrain> CreateGame()
        {
            if (_game != null)
            {
                LeaveGame();
            }
            _game = GrainFactory.GetGrain<IGameSessionGrain>(Guid.NewGuid());
            return Task.FromResult(_game);
        }

        public Task<IGameSessionGrain> GetCurrentGame()
            => Task.FromResult(_game);

        public async Task JoinGame(Guid gameKey)
        {
            var game = GrainFactory.GetGrain<IGameSessionGrain>(gameKey);
            await game.AddPlayer(State);
            _game = game;
            IAsyncStream<TriviaQuestion> questionStream = 
                GetStreamProvider(Constants.QuestionStreamProvider)
                .GetStream<TriviaQuestion>(_game.GetPrimaryKey(), Constants.QuestionStreamNamespace);
            await questionStream.SubscribeAsync(async (question, token) => await AnswerQuestion(question));
        }

        public Task LeaveGame()
        {
            _game.RemovePlayer(State);
            _game = null;
            return Task.CompletedTask;
        }

        public async Task AnswerQuestion(TriviaQuestion question)
        {
            // hook up service layer to obtain answer
        }
    }
}
