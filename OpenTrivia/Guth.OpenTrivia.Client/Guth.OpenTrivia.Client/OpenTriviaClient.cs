using System;
using System.Threading.Tasks;
using System.Collections.Immutable;
using RestSharp;
using Newtonsoft.Json;
using Guth.OpenTrivia.Client.Models;
using Guth.OpenTrivia.Client.Enums;

namespace Guth.OpenTrivia.Client
{
    public class OpenTriviaClient
    {
        private IRestClient _client;

        public OpenTriviaClient()
        {
            _client = new RestClient("https://opentdb.com");
        }

        public async Task<string> GetSessionToken()
        {
            var request = new OpenTriviaRequest<TokenResponse>("api_token.php")
                     .AddParameter("command", "request");
            TokenResponse response = await Execute(request);
            return response.Token;
        }

        private async Task<TResponse> Execute<TResponse>(OpenTriviaRequest<TResponse> request)
        {
            IRestResponse response = await _client.ExecuteAsync(request.Request);
            if (!response.IsSuccessful)
            {
                throw new Exception(null, response.ErrorException);
            }
            return JsonConvert.DeserializeObject<TResponse>(response.Content);
        }

        public async Task<ImmutableList<TriviaCategory>> GetTriviaCategories(string sessionToken = null)
        {
            var request = new OpenTriviaRequest<TriviaCategoriesResponse>("api_category.php", sessionToken);
            TriviaCategoriesResponse response = await Execute(request);
            return response.Categories;
        }

        public async Task<ImmutableList<TriviaQuestion>> GetTriviaQuestions(Action<QuestionOptionsBuilder> configureOptions = null, string sessionToken = null)
        {
            var optionsBuilder = new QuestionOptionsBuilder();
            if (configureOptions != null)
            {
                configureOptions(optionsBuilder);
            }
                
            var request = new OpenTriviaRequest<GetTriviaQuestionsResponse>("api.php", sessionToken)
                .AddParameter("amount", optionsBuilder.NumberOfQuestions)
                .AddParameterIfNotNull("category", (int?)optionsBuilder.Category)
                .AddParameterIfNotNull("difficulty", optionsBuilder.Difficulty)
                .AddParameterIfNotNull("type", optionsBuilder.Type);

            GetTriviaQuestionsResponse response = await Execute(request);
            return response.Questions;
        }
    }
}
