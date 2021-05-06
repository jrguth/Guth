using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Guth.OpenTrivia.Abstractions.Models
{
    public class Game
    {
        public string Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GameState State { get; set; } = default;
        public int RoundNumber { get; set; }
        public string HostPlayerId { get; set; }
        public string ConnectionCode { get; set; }
        public QuestionOptions QuestionOptions { get; set; }
        public ICollection<string> Players { get; set; } = new List<string>();
        public IReadOnlyCollection<TriviaQuestion> Questions { get; set; } = new List<TriviaQuestion>();
        public ICollection<TriviaRound> Rounds { get; set; } = new List<TriviaRound>();
        public ICollection<PlayerScore> Results { get; set; } = new List<PlayerScore>();
    }
}
