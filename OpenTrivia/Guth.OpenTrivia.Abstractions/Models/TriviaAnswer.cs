using System;
using System.Collections.Generic;
using System.Text;

namespace Guth.OpenTrivia.Abstractions.Models
{
    public class TriviaAnswer
    {
        public Player Player { get; set; }
        public string Answer { get; set; }
        public TimeSpan TimeElapsed { get; set; }
    }
}
