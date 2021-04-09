using Newtonsoft.Json;
using System.Collections.Immutable;
using Guth.OpenTrivia.Abstractions.Enums;

namespace Guth.OpenTrivia.Abstractions.Models
{
    public record GetTriviaQuestionsResponse(
        [JsonProperty("response_code")] int ResponseCode,
        [JsonProperty("results")] ImmutableList<TriviaQuestion> Questions);
}
