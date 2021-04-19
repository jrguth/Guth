using System.Collections.Immutable;
namespace Guth.OpenTrivia.Abstractions.Models
{
    public class TriviaRound
    {
        public int RoundNumber { get; set; }
        public int TotalRounds { get; set; }
        public int CountdownSeconds { get; set; }
        public string Question { get; set; }
        public ImmutableArray<string> Choices { get; set; }
    }
}
