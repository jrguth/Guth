using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Guth.OpenTrivia.WebApp.Api.Games
{
    public class StartGameCommand
    {
        [FromRoute(Name = "gameId")] 
        [Required(AllowEmptyStrings = false)]
        public string GameId { get; set; }

        [FromQuery(Name = "playerId")]
        [Required(AllowEmptyStrings = false)]
        public string PlayerId { get; set; }
    }
}
