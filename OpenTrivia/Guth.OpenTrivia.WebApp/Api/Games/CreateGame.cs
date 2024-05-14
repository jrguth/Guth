using Microsoft.AspNetCore.Mvc;
using Ardalis.ApiEndpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guth.OpenTrivia;
using Guth.OpenTrivia.FirebaseDB;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.Abstractions.Enums;
using System.Threading;
using Swashbuckle.AspNetCore.Annotations;

namespace Guth.OpenTrivia.WebApp.Api.Games
{
    public class CreateGame : EndpointBaseAsync
        .WithRequest<CreateGameCommand>
        .WithResult<Game>
    {
        private readonly TriviaRealtimeDB _db;

        public CreateGame(TriviaRealtimeDB db)
        {
            _db = db;
        }

        [HttpPost("/api/v1/games")]
        [SwaggerOperation(
            Summary = "Create a game",
            Description = "Create a game")]
        public override async Task<Game> HandleAsync([FromQuery][FromBody] CreateGameCommand request, CancellationToken cancellationToken = default)
        {
            ConnectionCode connection = await _db.GenerateConnectionCode(cancellationToken);
            return await _db.CreateGame(connection.Code, request.PlayerId, request.Options);
        }
    }
}
