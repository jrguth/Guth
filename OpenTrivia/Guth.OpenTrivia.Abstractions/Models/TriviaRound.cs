using System.Collections.Generic;

namespace Guth.OpenTrivia.Abstractions.Models
{
    public class TriviaRound
    {
        public TriviaQuestion Question { get; private set; }
        public ICollection<TriviaAnswer> Answers { get; set; } = new List<TriviaAnswer>();

        public TriviaRound(TriviaQuestion question)
        {
            Question = question;
        }
    }
}
