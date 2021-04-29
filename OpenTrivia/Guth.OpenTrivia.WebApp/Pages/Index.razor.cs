using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.WebApp.Components;


namespace Guth.OpenTrivia.WebApp.Pages
{
    public partial class Index
    {
        public Player Player { get; set; }
        public Game Game { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
