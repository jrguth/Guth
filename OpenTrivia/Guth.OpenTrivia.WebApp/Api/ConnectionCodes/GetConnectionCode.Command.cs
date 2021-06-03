using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Guth.OpenTrivia.WebApp.Api.ConnectionCodes
{
    public class GetConnectionCodeCommand
    {
        [Required]
        [FromRoute(Name = "connectionCode")]
        [StringLength(4, MinimumLength = 4)]
        public string ConnectionCode { get; set; }
    }
}
