using Newtonsoft.Json;
namespace Guth.OpenTrivia.Abstractions.Models
{
    public class Player
    {
        [JsonIgnore]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
