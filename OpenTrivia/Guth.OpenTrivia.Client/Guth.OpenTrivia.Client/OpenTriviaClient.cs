﻿using System;
using System.Threading.Tasks;
using System.Collections.Immutable;
using RestSharp;
using Newtonsoft.Json;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.Abstractions;

namespace Guth.OpenTrivia.Client
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

        public async Task<ImmutableList<TriviaQuestion>> GetTriviaQuestions(QuestionOptions questionOptions, string sessionToken = null)
        {

            var request = new OpenTriviaRequest<GetTriviaQuestionsResponse>("api.php", sessionToken)
                .AddParameter("amount", questionOptions.NumberOfQuestions)
                .AddParameterIfNotNull("category", (int?)questionOptions.Category)
                .AddParameterIfNotNull("difficulty", questionOptions.Difficulty.ToString().ToLower())
                .AddParameterIfNotNull("type", questionOptions.Type == Abstractions.Enums.QuestionType.MultipleChoice ? "multiple" : "boolean");

            GetTriviaQuestionsResponse response = await Execute(request);
            return response.Questions;
        }
    }
}
