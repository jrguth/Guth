using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.Abstractions.Enums;
using Guth.OpenTrivia.FirebaseDB;

namespace Guth.OpenTrivia.WebApp.Workers
{
    public class GameConductor
    {
        private Game _game;
        private TriviaRealtimeDB _db;

        public GameConductor(Game game, TriviaRealtimeDB db)
        {
            _game = game;
            _db = db;
            _db.OnGameUpdate(_game.Id, game => _game = game);
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && _game.RoundNumber < _game.Questions.Count)
            {
                await ExecuteNextRound();
            }
        }

        private async Task ExecuteNextRound()
        {
            var countdown = new CountdownEvent(1);
            _db.OnRoundUpdate(_game.Id, async(round) =>
            {
                Game updatedGame = await _db.GetGame(_game.Id);
                if (round.Answers.Count >= updatedGame.Players.Count)
                {
                    countdown.Signal();
                }
            });
            countdown.Wait(TimeSpan.FromSeconds(30));
            await _db.EndRound(_game.Id);
        }
    }
}
