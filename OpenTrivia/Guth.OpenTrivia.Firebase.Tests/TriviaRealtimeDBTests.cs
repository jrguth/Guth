using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Firebase.Database;
using Guth.OpenTrivia.Firebase;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;
namespace Guth.OpenTrivia.Firebase.Tests
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
            ConnectionCode connection = await db.GenerateGameConnection();
            Assert.That(connection, Is.Not.Null);
            Assert.That(connection.Code.Length, Is.EqualTo(4));
            Assert.That(connection.IsActive, Is.True);
        }

        [Test]
        public async Task CreateGame_PostsNewGame_ReturnsStringKey()
        {
            string id = await db.CreateGame("123");
            Assert.That(id, Is.Not.Null);
        }

        [Test]
        public async Task UpdateQuestionOptions_GameExists_ReturnsUpdateGame()
        {
            string gameId = await db.CreateGame("123");
            var questionOptions = new QuestionOptions();
            Game game = await db.UpdateGameOptions(gameId, questionOptions);
            Assert.That(game, Is.Not.Null);
            Assert.That(game.QuestionOptions, Is.EqualTo(questionOptions));
        }

        [Test]
        public async Task SubscribeToGame_OnGameUpdate_InvokesHandler()
        {
            string playerId = await db.CreatePlayer("Test Player");
            string gameId = await db.CreateGame(playerId);
            string gameId2 = await db.CreateGame(playerId);

            bool gameOneHandlerInvoked = false;
            bool gameTwoHandlerInvoked = false;

            db.SubscribeToGame(gameId, game => gameOneHandlerInvoked = true);

            await db.UpdateGameOptions(gameId, new QuestionOptions());
            await db.UpdateGameOptions(gameId2, new QuestionOptions());

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