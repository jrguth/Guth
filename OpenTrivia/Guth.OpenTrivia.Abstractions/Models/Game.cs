using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Guth.OpenTrivia.Abstractions.Models
{
    public class Game
    {
        [JsonIgnore]
        public string Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GameState State { get; set; } = default;
        public string HostPlayerId { get; set; }
        public string ConnectionCode { get; set; }
        public QuestionOptions QuestionOptions { get; set; }
        public ICollection<string> Players { get; set; }
        public Stack<TriviaQuestion> Questions { get; set; }
        public ICollection<TriviaRound> Rounds { get; set; }
    }
}
