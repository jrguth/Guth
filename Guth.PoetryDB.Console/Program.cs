using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using RestSharp;

namespace Guth.PoetryDB.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string entry;
            do
            {
                System.Console.ForegroundColor = System.ConsoleColor.White;
                System.Console.WriteLine("Enter 'y' for a new poem, and anything else to exit:\r\n");
                entry = System.Console.ReadLine();
                if  (entry == "y")
                {
                    System.Console.ForegroundColor = System.ConsoleColor.Cyan;
                    var client = new PoetryDBClient(new RestClient("https://poetrydb.org"));
                    IEnumerable<Poem> poems = await client.GetRandomTitles(1);
                    var poem = poems.First();

                    System.Console.WriteLine($"Title: {poem.Title}\r\nAuthor: {poem.Author}\r\n\r\n{string.Join("\r\n", poem.Lines)}\r\n");
                }
            } while (entry.ToLower().Trim() == "y");
        }
    }
}
