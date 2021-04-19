using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.GrainInterfaces.Models
{
    public class Game
    {
        public Guid Key { get; set; }
        public HashSet<Player> Players { get; set; } = new HashSet<Player>();
        public Stack<TriviaQuestion> Questions { get; set; }
        public GameResults Results { get; set; }
    }
}
