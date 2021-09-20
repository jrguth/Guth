using System.Collections.Generic;
using System.Threading.Tasks;

namespace Guth.PoetryDB
{
    public interface IPoetryDBClient
    {
        Task<IEnumerable<Poem>> GetRandomTitles(int numTitles);
        Task<IEnumerable<Poem>> GetTitlesByAuthor(string author);
        Task<Poem> GetTitle(string title);
    }
}