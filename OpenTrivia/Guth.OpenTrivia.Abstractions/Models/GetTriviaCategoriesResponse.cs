using Newtonsoft.Json;
using System.Collections.Immutable;
namespace Guth.OpenTrivia.Abstractions.Models
{
    public class GetTriviaCategoriesResponse
    {
        [JsonProperty("trivia_categories")]
        public ImmutableList<TriviaCategory> Categories { get; set; }
    }
}
