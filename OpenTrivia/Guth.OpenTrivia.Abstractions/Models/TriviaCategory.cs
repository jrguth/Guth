using Newtonsoft.Json;
using System.Collections.Immutable;
namespace Guth.OpenTrivia.Abstractions.Models
{
    public class TriviaCategory
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")] 
        public string Name { get; set; }
    }
}
