using System.Collections.Generic;
using System.Threading.Tasks;

using RestSharp;

namespace Guth.PoetryDB
{
    public class PoetryDBClient : IPoetryDBClient
    {
        private readonly IRestClient _client;

        public PoetryDBClient(IRestClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Poem>> GetRandomTitles(int numTitles)
        {
            var req = new RestRequest($"/random/{numTitles}/all.json", Method.GET);
            return await _client.GetAsync<List<Poem>>(req);
        }
    }
}
