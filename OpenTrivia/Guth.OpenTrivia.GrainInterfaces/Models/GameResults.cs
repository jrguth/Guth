using System.Collections.Generic;
using Guth.OpenTrivia.Abstractions.Models;

namespace Guth.OpenTrivia.GrainInterfaces.Models
{
    public class GameResults : Dictionary<TriviaQuestion, List<TriviaAnswer>>
    {
        public bool AddQuestionAnswer(TriviaQuestion question, TriviaAnswer answer)
        {
            if (ContainsKey(question))
            {
                this[question].Add(answer);
            }
            else
            {
                this[question] = new List<TriviaAnswer> { answer };
            }
            return answer.Answer == question.CorrectAnswer;
        }
    }
}
