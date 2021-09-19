using System;
using System.Collections.Generic;

namespace Guth.PoetryDB.Models
{
    public class Poem
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public IEnumerable<string> Lines { get; set; }
    }
}
