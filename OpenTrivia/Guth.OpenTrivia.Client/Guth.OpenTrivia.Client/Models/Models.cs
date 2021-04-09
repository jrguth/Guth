using Newtonsoft.Json;
using System.Collections.Immutable;
using Guth.OpenTrivia.Client.Enums;

namespace Guth.OpenTrivia.Client.Models
{
    public record TokenResponse(
        [JsonProperty("response_code")] int ResponseCode, 
        [JsonProperty("response_message")] string ResponseMessage,
        [JsonProperty("token")] string Token);

    public record TriviaCategory(
        [JsonProperty("id")] int Id,
        [JsonProperty("name")] string Name);

    public record TriviaCategoriesResponse([JsonProperty("trivia_categories")]ImmutableList<TriviaCategory> Categories);

    public record TriviaQuestion(
        [JsonProperty("category")]string Category,
        [JsonProperty("type")] QuestionType Type,
        [JsonProperty("difficulty")] QuestionDifficulty Difficulty,
        [JsonProperty("question")] string Question,
        [JsonProperty("correct_answer")] string CorrectAnswer,
        [JsonProperty("incorrect_answers")] ImmutableList<string> IncorrectAnswers);

    public record GetTriviaQuestionsResponse(
        [JsonProperty("response_code")] int ResponseCode,
        [JsonProperty("results")] ImmutableList<TriviaQuestion> Questions);
}
