using Newtonsoft.Json;
using System.Collections.Immutable;
namespace Guth.OpenTrivia.Abstractions.Models
{
    public record TriviaCategory(
        [JsonProperty("id")] int Id,
        [JsonProperty("name")] string Name);
}
