using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Orleans;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.GrainInterfaces
{
    public interface IGameGrain : IGrainWithGuidKey
    {
        Task ConfigureOptions(GameOptions gameOptions, QuestionOptions questionOptions);
        Task AddPlayer(IPlayerGrain player);
        Task RemovePlayer(IPlayerGrain player);
        Task Start();
    }
}
