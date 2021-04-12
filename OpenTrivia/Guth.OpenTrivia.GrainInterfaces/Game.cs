using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.GrainInterfaces
{
    public class Game
    {
        private ImmutableStack<TriviaQuestion> _questions;
        public Guid Key { get; set; }
        public ImmutableHashSet<Player> Players { get; set; } = ImmutableHashSet.Create<Player>();
        public ImmutableStack<TriviaQuestion> Questions
        {
            get => _questions;
            set
            {
                TotalRounds = value.ToImmutableArray().Length;
                _questions = value;
            }
        }
        public bool Completed => Questions.IsEmpty;
        public int RoundNumber { get; set; } = 1;
        public int TotalRounds { get; private set; }
    }
}
