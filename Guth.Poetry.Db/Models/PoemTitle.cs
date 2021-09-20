namespace Guth.Poetry.Db.Models
{
    public class PoemTitle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public PoemAuthor Author { get; set; }
        public string Lines { get; set; }
    }
}
