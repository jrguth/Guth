using static System.Console;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.Abstractions.Enums;
using Guth.OpenTrivia.GrainInterfaces;

namespace Guth.OpenTrivia.ConsoleApp
{
    class Program
    {
        private static string name;
        private static IClusterClient _cluster;
        private static IPlayerGrain _player;
        private static IGameGrain _game;
        static async Task Main(string[] args)
        {
            await ConnectToCluster();
            _player = _cluster.GetGrain<IPlayerGrain>(Guid.NewGuid());
            await _player.SetName(PrintAndReceiveInput("Enter your name: "));

            if (PrintAndReceiveInput("Enter 1 to start a new OpenTrivia game, anything else to connect to an existing one: ") == "1")
            {
                _game = await CreateGame();
                PrintAndReceiveInput("Press enter to start...");
                await _game.Start();
                bool isComplete = false;
                try
                {
                    do
                    {
                        isComplete = await _game.StartNextRound();
                    } while (!isComplete);
                    ReadLine();
                }
                catch (Exception e)
                {
                    WriteLine($"EXCEPTION:\n{e}");
                    ReadLine();
                }
            }
            else
            {
                _game = _cluster.GetGrain<IGameGrain>(new Guid(PrintAndReceiveInput("Enter connection id of game: ")));
                await _player.JoinGame(_game);
                ReadLine();
            }
        }

        private static string PrintAndReceiveInput(string text)
        {
            WriteLine(text);
            return ReadLine();
        }

        private async static Task ConnectToCluster()
        {
            _cluster = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(opts =>
                {
                    opts.ClusterId = "guth-open-trivia-cluster";
                    opts.ServiceId = "Guth.OpenTrivia.Silo";
                })
                .ConfigureApplicationParts(_ => _.AddApplicationPart(typeof(IPlayerGrain).Assembly).WithReferences().WithCodeGeneration())
                .Build();
            int retries = 0;
            await _cluster.Connect(exception =>
            {
                retries += 1;
                if (retries >= 3)
                {
                    return Task.FromResult(false);
                }
                Thread.Sleep(3 * 1000);
                return Task.FromResult(true);
            });
        }

        private async static Task<IGameGrain> CreateGame()
        {
            IGameGrain gameGrain = await _player.CreateGame(new GameOptions(), ConfigureQuestionOptions());
            WriteLine($"Game created with id: {gameGrain.GetPrimaryKey()}");
            return gameGrain;
        }

        private static QuestionOptions ConfigureQuestionOptions()
        {
            var categories = Enum.GetValues<QuestionCategory>();
            return new QuestionOptions()
                .WithCategory((QuestionCategory)int.Parse(PrintAndReceiveInput($"Enter one of the following categories:\n{string.Join("\n", categories.Select(cat => $"{(int)cat}: {cat}"))}")))
                .WithDifficulty((QuestionDifficulty)int.Parse(PrintAndReceiveInput("Enter 1 for easy, 2 for medium, 3 for hard questions: ")))
                .WithNumberOfQuestions(int.Parse(PrintAndReceiveInput("Enter number of questions between 1 and 50: ")))
                .WithQuestionType(QuestionType.MultipleChoice);
        }

    }
}
