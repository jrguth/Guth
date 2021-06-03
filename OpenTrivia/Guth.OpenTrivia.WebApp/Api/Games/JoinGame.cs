using Ardalis.ApiEndpoints;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.FirebaseDB;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Guth.OpenTrivia.WebApp.Api.Games
{
    public class JoinGame : BaseAsyncEndpoint
        .WithRequest<JoinGameCommand>
        .WithResponse<Game>
    {
        private readonly TriviaRealtimeDB _db;

        public JoinGame(TriviaRealtimeDB db)
        {
            _db = db;
        }

        [HttpPost("/api/v1/games/{gameId}/join")]
        public override async Task<ActionResult<Game>> HandleAsync([FromQuery][FromRoute] JoinGameCommand request, CancellationToken cancellationToken = default)
        {
            ConnectionCode connection = await _db.GetConnectionCode(request.ConnectionCode);
            await _db.AddPlayerToGame(connection.GameId, request.PlayerId);
            return await _db.GetGame(connection.GameId);
        }
    }
}
