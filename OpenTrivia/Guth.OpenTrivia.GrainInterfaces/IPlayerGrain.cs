using System.Threading.Tasks;
using Orleans;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.GrainInterfaces
{
    public interface IPlayerGrain : IGrainWithGuidKey
    {
        Task<IGameSessionGrain> CurrentGame { get; }
        Task JoinGame(IGameSessionGrain game);
        Task LeaveGame(IGameSessionGrain game);
    }
}
