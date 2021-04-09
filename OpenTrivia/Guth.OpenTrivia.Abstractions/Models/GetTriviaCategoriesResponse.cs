using Newtonsoft.Json;
using System.Collections.Immutable;
namespace Guth.OpenTrivia.Abstractions.Models
{
    public record GetTriviaCategoriesResponse([JsonProperty("trivia_categories")]ImmutableList<TriviaCategory> Categories);
}
