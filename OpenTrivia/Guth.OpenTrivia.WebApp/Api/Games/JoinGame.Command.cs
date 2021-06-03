using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Guth.OpenTrivia.WebApp.Api.Games
{
    public class JoinGameCommand
    {
        [FromQuery(Name = "connectionCode")]
        [StringLength(4, MinimumLength = 4)]
        [Required]
        [SwaggerParameter]
        public string ConnectionCode { get; set; }

        [FromQuery(Name = "playerId")]
        [Required(AllowEmptyStrings = false)]
        [SwaggerParameter]
        public string PlayerId { get; set; }

        [FromRoute(Name = "gameId")]
        [Required(AllowEmptyStrings = false)]
        public string GameId { get; set; }
    }
}
