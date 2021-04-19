using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Guth.OpenTrivia.GrainInterfaces;

namespace Guth.OpenTrivia.Grains
{
    public class GameHub : Hub<IPlayerGrain>, IGameHub
    {
        private readonly ILogger<GameHub> _logger;
        private readonly IClusterClient _clusterClient;
        private readonly IGameGrain _gameGrain;

        public GameHub(ILogger<GameHub> logger, IClusterClient clusterClient, IGameGrain gameGrain)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clusterClient = clusterClient ?? throw new ArgumentNullException(nameof(clusterClient));
            _gameGrain = gameGrain;
        }


    }
}
