using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.GrainInterfaces
{
    public class Game
    {
        public Guid Key { get; set; }
        public ImmutableHashSet<Guid> Players { get; set; } = ImmutableHashSet.Create<Guid>();
        public int MaxSecondsPerRound { get; set; } = 15;
        public ImmutableStack<TriviaQuestion> Questions { get; set; }
        public Dictionary<Guid, int> PlayerResults { get; set; } = new Dictionary<Guid, int>();
    }
}
