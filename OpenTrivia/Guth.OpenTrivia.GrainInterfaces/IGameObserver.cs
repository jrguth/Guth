using System.Threading.Tasks;
using System.Collections.Immutable;
using Orleans;

namespace Guth.OpenTrivia.GrainInterfaces
{
    public interface IGameObserver : IGrainObserver
    {
        void UpdateGame(Game game);
    }
}
