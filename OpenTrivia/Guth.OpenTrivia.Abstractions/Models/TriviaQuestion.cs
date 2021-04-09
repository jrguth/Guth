using Newtonsoft.Json;
using System.Collections.Immutable;
using Guth.OpenTrivia.Abstractions.Enums;
namespace Guth.OpenTrivia.Abstractions.Models
{
    public record TriviaQuestion(
        [JsonProperty("category")]string Category,
        [JsonProperty("type")] QuestionType Type,
        [JsonProperty("difficulty")] QuestionDifficulty Difficulty,
        [JsonProperty("question")] string Question,
        [JsonProperty("correct_answer")] string CorrectAnswer,
        [JsonProperty("incorrect_answers")] ImmutableList<string> IncorrectAnswers);
}
