using System;
using System.Collections.Generic;
using System.Text;
using Guth.OpenTrivia.Abstractions.Enums;

namespace Guth.OpenTrivia.GrainInterfaces
{
    public class Player
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public int Points { get; set; } = 0;
    }
}
