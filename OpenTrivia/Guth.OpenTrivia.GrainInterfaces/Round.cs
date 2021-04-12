using System.Collections.Immutable;
namespace Guth.OpenTrivia.GrainInterfaces
{
    public class Round
    {
        public string Question { get; }
        public int TimeoutSeconds { get; }
        public ImmutableArray<string> Choices { get; }
        public Round(string question, int timeoutSeconds, ImmutableArray<string> choices)
        {
            Question = question;
            TimeoutSeconds = timeoutSeconds;
            Choices = choices;
        }
    }
    
}
