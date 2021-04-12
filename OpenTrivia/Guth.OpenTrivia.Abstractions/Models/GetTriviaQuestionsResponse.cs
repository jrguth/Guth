using Newtonsoft.Json;
using System.Collections.Immutable;
using Guth.OpenTrivia.Abstractions.Enums;

namespace Guth.OpenTrivia.Abstractions.Models
{
    public class GetTriviaQuestionsResponse
    {
        [JsonProperty("response_code")]
        public int ResponseCode { get; set; }
        [JsonProperty("results")] 
        public ImmutableList<TriviaQuestion> Questions { get; set; }
    }
}
