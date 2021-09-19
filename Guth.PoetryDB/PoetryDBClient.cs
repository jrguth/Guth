using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Guth.PoetryDB.Models;

namespace Guth.PoetryDB
{
    public class PoetryDBClient
    {
        private readonly Uri _baseUri;
        private readonly HttpClient _client;

        internal PoetryDBClient(string baseUrl)
        {
            _client = new HttpClient();
            _baseUri = new Uri(baseUrl);

        public async Task<IEnumerable<Poem>> SearchByAuthorAsync(string author)
        {
            var res = await _client.GetFromJsonAsync()
        }

        private async Task<T> ReturnIfSuccessful<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        private HttpRequestMessage GetBaseRequest()
            => new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = _baseUri
            };
    }
}
