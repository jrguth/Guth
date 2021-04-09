using System;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Orleans;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.GrainInterfaces
{
    public interface IGameSessionGrain : IGrainWithGuidKey
    {
        Task SubscribeToGame(IGameObserver observer);
        Task UnsubscribeFromGame(IGameObserver observer);
    }
    public interface IGameObserver : IGrainObserver
    {
        Task AskQuestion(TriviaQuestion question);
        Task AnswerQuestion(Guid player, string answer);
    }
}
