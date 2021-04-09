﻿using System;
using System.Threading.Tasks;
using System.Collections.Immutable;
using RestSharp;
using Newtonsoft.Json;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.Abstractions.Enums;

namespace Guth.OpenTrivia.Abstractions
{
    public class OpenTriviaClient : IOpenTriviaClient
    {
        private IRestClient _client;

        public OpenTriviaClient()
        {
            _client = new RestClient("https://opentdb.com");
        }

        public async Task<string> GetSessionToken()
        {
            var request = new OpenTriviaRequest<GetSessionTokenResponse>("api_token.php")
                     .AddParameter("command", "request");
            GetSessionTokenResponse response = await Execute(request);
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
            var request = new OpenTriviaRequest<GetTriviaCategoriesResponse>("api_category.php", sessionToken);
            GetTriviaCategoriesResponse response = await Execute(request);
            return response.Categories;
        }

        public async Task<ImmutableList<TriviaQuestion>> GetTriviaQuestions(Action<QuestionOptions> configureOptions = null, string sessionToken = null)
        {
            var optionsBuilder = new QuestionOptions();
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
