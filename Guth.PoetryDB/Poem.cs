using System.Collections.Generic;

namespace Guth.PoetryDB
{
    public class Poem
    {
        public Poem() { }
        public string Title { get; set; }
        public string Author { get; set; }
        public IEnumerable<string> Lines { get; set; }
    }
}
