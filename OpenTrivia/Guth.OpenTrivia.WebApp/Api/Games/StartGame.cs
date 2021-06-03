using Ardalis.ApiEndpoints;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.FirebaseDB;
using Guth.OpenTrivia.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace Guth.OpenTrivia.WebApp.Api.Games
{
    public class StartGame : BaseAsyncEndpoint
        .WithRequest<StartGameCommand>
        .WithResponse<Game>
    {
        private readonly TriviaRealtimeDB _db;
        private readonly IOpenTriviaClient _openTriviaClient;
        public StartGame(TriviaRealtimeDB db, IOpenTriviaClient openTriviaClient)
        {
            _db = db;
            _openTriviaClient = openTriviaClient;
        }

        [HttpPost("/api/v1/games/{gameId}/start")]
        public override async Task<ActionResult<Game>> HandleAsync([FromRoute,FromQuery]StartGameCommand request, CancellationToken cancellationToken = default)
        {
            Game game = await _db.GetGame(request.GameId);
            if (game.HostPlayerId != request.PlayerId)
            {
                return Unauthorized();
            }
            ImmutableList<TriviaQuestion> questions = await _openTriviaClient.GetTriviaQuestions(game.QuestionOptions);
            await _db.StartGame(game.Id, questions);
            return Ok(game);
        }
    }
}
