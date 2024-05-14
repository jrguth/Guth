using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Guth.OpenTrivia.FirebaseDB;
using Microsoft.AspNetCore.Mvc;

namespace Guth.OpenTrivia.WebApp.Api.ConnectionCodes
{
    public class GetConnectionCode : EndpointBaseAsync
        .WithRequest<GetConnectionCodeCommand>
        .WithActionResult<ConnectionCode>
    {
        private readonly TriviaRealtimeDB _db;

        public GetConnectionCode(TriviaRealtimeDB db)
        {
            _db = db;
        }

        [HttpGet("/api/v1/connectioncodes/{connectionCode}")]
        public override async Task<ActionResult<ConnectionCode>> HandleAsync([FromRoute]GetConnectionCodeCommand command, CancellationToken cancellationToken = default)
        {
            ConnectionCode code = await _db.GetConnectionCode(command.ConnectionCode);
            if (code is null)
            {
                return NotFound();
            }
            return code;
        }
    }
}
