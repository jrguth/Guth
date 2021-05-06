using System.Collections.Generic;

namespace Guth.OpenTrivia.Abstractions.Models
{
    public class TriviaRound
    {
        public int RoundNumber { get; set; }
        public TriviaQuestion Question { get; set; }
        public ICollection<TriviaAnswer> Answers { get; set; } = new List<TriviaAnswer>();
    }
}
