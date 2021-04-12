using Guth.OpenTrivia.Abstractions.Models;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Guth.OpenTrivia.Abstractions
{
    public interface IOpenTriviaClient
    {
        Task<string> GetSessionToken();
        Task<ImmutableList<TriviaCategory>> GetTriviaCategories(string sessionToken = null);
        Task<ImmutableList<TriviaQuestion>> GetTriviaQuestions(QuestionOptions configureOptions = null, string sessionToken = null);
    }
}