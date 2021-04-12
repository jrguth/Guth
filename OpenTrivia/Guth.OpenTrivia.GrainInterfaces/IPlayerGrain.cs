using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System;
using Orleans;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.GrainInterfaces
{
    public interface IPlayerGrain : IGrainWithGuidKey, IGameObserver
    {
        Task<Player> GetPlayer();
        Task<Game> GetCurrentGame();
        Task SetName(string name);
        Task<IGameGrain> CreateGame(GameOptions gameOptions, QuestionOptions questionOptions);
        Task JoinGame(IGameGrain game);
        Task LeaveGame(IGameGrain game);
        Task<string> AnswerQuestion(Round round);
    }
}
