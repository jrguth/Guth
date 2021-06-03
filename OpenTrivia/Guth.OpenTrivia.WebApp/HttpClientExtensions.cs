using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetAsync<T>(this HttpClient client, string uri)
            where T : class
            => await ExecuteAndDeserialize<T>(client, HttpMethod.Get, uri);


        public static async Task<TOut> PostWithJsonAsync<TRequest, TOut>(this HttpClient client, string uri, TRequest content)
            => await ExecuteAndDeserialize<TRequest, TOut>(client, HttpMethod.Post, uri, content);

        public static async Task<TOut> PostReturnJsonAsync<TOut>(this HttpClient client, string uri)
            => await ExecuteAndDeserialize<TOut>(client, HttpMethod.Post, uri);

        private static async Task<TOut> ExecuteAndDeserialize<TOut>(this HttpClient client, HttpMethod method, string uri)
        {
            using (var request = new HttpRequestMessage(method, uri))
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return DeserializeJsonFromStream<TOut>(await response.Content.ReadAsStreamAsync());
            }
        }

        private static async Task<TOut> ExecuteAndDeserialize<TIn,TOut>(this HttpClient client, HttpMethod method, string uri, TIn content)
        {
            using (var request = new HttpRequestMessage(method, uri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content))
            })
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return DeserializeJsonFromStream<TOut>(await response.Content.ReadAsStreamAsync());
            }
        }

        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default(T);

            using (var sr = new StreamReader(stream))
            using (var jtr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                var searchResult = js.Deserialize<T>(jtr);
                return searchResult;
            }
        }
    }
}
