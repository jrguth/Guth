
using Guth.Poetry.Db.Models;

using Microsoft.EntityFrameworkCore;

namespace Guth.Poetry.Db
{
    public class PoetryContext : DbContext
    {
        public PoetryContext(DbContextOptions options): base(options) { }

        public DbSet<PoemTitle> Titles { get; set; }
        public DbSet<PoemAuthor> Authors { get; set; }
    }
}
