using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Firebase.Database;
using Guth.OpenTrivia.FirebaseDB;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;
namespace Guth.OpenTrivia.FirebaseDB.Tests
{
    public class TriviaRealtimeDBTests
    {
        private TriviaRealtimeDB db;
        private FirebaseClient client;
        private FirebaseConfig config;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            config = new FirebaseConfig();
            IConfigurationRoot root = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Path.GetDirectoryName(typeof(TriviaRealtimeDB).Assembly.Location), "appsettings.json"))
                .Build();
            root.GetSection("Firebase").Bind(config);
            client = new FirebaseClient(config.Url, new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(config.AppSecret)
            });
            db = new TriviaRealtimeDB(client);
        }

        [SetUp]
        public async Task Setup() => await ClearDb();

        [TearDown]
        public async Task TearDown() => await ClearDb();

        [Test]
        public async Task GenerateConnectionCode_NoCodesAdded_AddsCodeAndReturnsString()
        {
            ConnectionCode connection = await db.GenerateConnectionCode();
            Assert.That(connection, Is.Not.Null);
            Assert.That(connection.Code.Length, Is.EqualTo(4));
        }

        [Test]
        public async Task CreateGame_PostsNewGame_ReturnsStringKey()
        {
            ConnectionCode code = await db.GenerateConnectionCode();
            Game game = await db.CreateGame(code.Code);
            Assert.That(game, Is.Not.Null);
        }

        [Test]
        public async Task UpdateQuestionOptions_GameExists_ReturnsUpdateGame()
        {
            ConnectionCode code = await db.GenerateConnectionCode();
            Game game = await db.CreateGame(code.Code);
            var questionOptions = new QuestionOptions();
            game = await db.UpdateGameOptions(game.Id, questionOptions);
            Assert.That(game, Is.Not.Null);
            Assert.That(game.QuestionOptions, Is.EqualTo(questionOptions));
        }

        [Test]
        public async Task SubscribeToGame_OnGameUpdate_InvokesHandler()
        {
            Player p = await db.CreatePlayer("Test Player");
            Game gameOne = await db.CreateGame((await db.GenerateConnectionCode()).Code);
            Game gameTwo = await db.CreateGame((await db.GenerateConnectionCode()).Code);

            bool gameOneHandlerInvoked = false;
            bool gameTwoHandlerInvoked = false;

            db.SubscribeToGame(gameOne.Id, game =>
            {
                gameOneHandlerInvoked = true;
            });

            //await db.UpdateGameOptions(gameOne.Id, new QuestionOptions());
            await db.AddPlayerToGame(gameOne.Id, p.Id);
            await db.UpdateGameOptions(gameTwo.Id, new QuestionOptions());

            Assert.That(gameOneHandlerInvoked, Is.True);
            Assert.That(gameTwoHandlerInvoked, Is.False);
        }

        private async Task ClearDb()
        {
            await db.DbClient.Child("Games").DeleteAsync();
            await db.DbClient.Child("ConnectionCodes").DeleteAsync();
            await db.DbClient.Child("Players").DeleteAsync();
        }
    }
}