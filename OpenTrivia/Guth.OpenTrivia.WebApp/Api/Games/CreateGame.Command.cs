using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Guth.OpenTrivia.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Guth.OpenTrivia.WebApp.Api.Games
{
    public class CreateGameCommand
    {
        [FromQuery(Name = "playerId")]
        [Required(AllowEmptyStrings = false)]
        public string PlayerId { get; set; }

        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)]
        [Required]
        public QuestionOptions Options { get; set; }
    }
}
