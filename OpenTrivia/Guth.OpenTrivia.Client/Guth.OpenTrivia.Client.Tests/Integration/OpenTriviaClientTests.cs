using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Linq;
using Guth.OpenTrivia.Client.Enums;
using Guth.OpenTrivia.Client.Models;

namespace Guth.OpenTrivia.Client.Tests.Integration
{
    public class OpenTriviaClientTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetOrCreateSession_WhenCalled_ReturnsNewSessionToken()
        {
            var client = new OpenTriviaClient();
            string token = await client.GetSessionToken();
            Assert.That(token, Is.Not.Null);
        }

        [Test]
        public async Task GetTriviaCategories_WhenCalled_ReturnsTriviaCategories()
        {
            var client = new OpenTriviaClient();
            var categories = await client.GetTriviaCategories();
            Assert.That(categories, Is.InstanceOf<ImmutableList<TriviaCategory>>());
            Assert.That(categories.IsEmpty, Is.False);
        }

        [Test]
        public async Task GetTriviaQuestions_NoCustomOptions_ReturnsTriviaQuestions()
        {
            var client = new OpenTriviaClient();
            var questions = await client.GetTriviaQuestions();
            Assert.That(questions, Is.Not.Null);
            Assert.That(questions, Is.InstanceOf<ImmutableList<TriviaQuestion>>());
            Assert.That(questions.Any(), Is.True);
        }

        [Test]
        public async Task GetTriviaQuestions_WithOneQuestion_WithGeneralKnowledgeCategory_ReturnsOneGeneralKnowledgeQuestion()
        {
            var client = new OpenTriviaClient();
            var questions = await client.GetTriviaQuestions(options => options
                .WithCategory(QuestionCategory.GeneralKnowledge)
                .WithNumberOfQuestions(1));
            Assert.That(questions, Is.Not.Null);
            Assert.That(questions.Count, Is.EqualTo(1));
            Assert.That(questions.FirstOrDefault()?.Category, Is.EqualTo("General Knowledge"));
        }
    }
}