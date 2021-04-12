using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.GrainInterfaces
{
    public class Game
    {
        private Stack<TriviaQuestion> _questions;
        public Guid Key { get; set; }
        public HashSet<Player> Players { get; set; } = new HashSet<Player>();
        public Stack<TriviaQuestion> Questions
        {
            get => _questions;
            set
            {
                TotalRounds = value.Count;
                _questions = value;
            }
        }
        public bool IsComplete => Questions.Count == 0;
        public int RoundNumber { get; set; } = 1;
        public int TotalRounds { get; private set; }
    }
}
