using static System.Console;
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
        static async Task Main(string[] args)
        {
            await ConnectToCluster();
            _player = _cluster.GetGrain<IPlayerGrain>(Guid.Empty);
            await _player.SetName(PrintAndReceiveInput("Enter your name: "));

            if (PrintAndReceiveInput("Enter 1 to start a new OpenTrivia game, anything else to connect to an existing one: ") == "1")
            {
                await CreateGame();
            }
            else
            {

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
            await _cluster.Connect();
        }

        private async static Task CreateGame()
        {
            Guid gameGuid = await _player.CreateGame(new GameOptions(), ConfigureQuestionOptions());
            WriteLine($"Game created with id: {gameGuid}");
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
