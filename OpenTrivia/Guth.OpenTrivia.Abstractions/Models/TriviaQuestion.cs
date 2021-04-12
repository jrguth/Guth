using Newtonsoft.Json;
using System.Collections.Immutable;
using Guth.OpenTrivia.Abstractions.Enums;
namespace Guth.OpenTrivia.Abstractions.Models
{
    public class TriviaQuestion
    {
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("type")] 
        public QuestionType Type { get; set; }
        [JsonProperty("difficulty")]
        public QuestionDifficulty Difficulty { get; set; }
        [JsonProperty("question")] 
        public string Question { get; set; }
        [JsonProperty("correct_answer")] 
        public string CorrectAnswer { get; set; }
        [JsonProperty("incorrect_answers")] 
        public ImmutableArray<string> IncorrectAnswers { get; set; }
    }
}
