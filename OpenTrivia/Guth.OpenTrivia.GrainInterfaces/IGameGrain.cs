using System;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Orleans;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.GrainInterfaces
{
    public interface IGameGrain : IGrainWithGuidKey
    {
        Task ConfigureOptions(GameOptions gameOptions, QuestionOptions questionOptions);
        Task Start();
        Task AddPlayer(IPlayerGrain player);
        Task RemovePlayer(IPlayerGrain player);
        Task<bool> StartNextRound(int bufferSeconds = 3);
    }
}
