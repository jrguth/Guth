using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Guth.OpenTrivia.FirebaseDB;
using Microsoft.AspNetCore.Mvc;

namespace Guth.OpenTrivia.WebApp.Api.ConnectionCodes
{
    public class CreateConnectionCode : EndpointBaseAsync
        .WithoutRequest
        .WithResult<ConnectionCode>
    {
        private readonly TriviaRealtimeDB _db;

        public CreateConnectionCode(TriviaRealtimeDB db)
        {
            _db = db;
        }

        [HttpPost("/api/v1/connectioncodes")]
        public override async Task<ConnectionCode> HandleAsync(CancellationToken cancellationToken = default)
            => await _db.GenerateConnectionCode(cancellationToken);
    }
}
