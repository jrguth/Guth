using System.Collections.Generic;

namespace Guth.Poetry.Db.Models
{
    public class PoemAuthor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PoemTitle> Titles { get; set; }
    }
}
