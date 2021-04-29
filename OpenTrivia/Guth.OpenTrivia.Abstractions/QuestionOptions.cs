using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Guth.OpenTrivia.Abstractions.Enums;

namespace Guth.OpenTrivia.Abstractions
{
    public class QuestionOptions
    {
        public int NumberOfQuestions { get; set; } = 10;
        [JsonConverter(typeof(StringEnumConverter))]
        public QuestionDifficulty Difficulty { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public QuestionType Type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public QuestionCategory Category { get; set; }


        public QuestionOptions WithCategory(QuestionCategory category)
        {
            Category = category;
            return this;
        }

        public QuestionOptions WithNumberOfQuestions(int numberOfQuestions)
        {
            if (numberOfQuestions < 1 || numberOfQuestions > 50)
            {
                throw new InvalidOperationException("Number of questions must be between 1 and 50");
            }
            NumberOfQuestions = numberOfQuestions;
            return this;
        }

        public QuestionOptions WithDifficulty(QuestionDifficulty difficulty)
        {
            Difficulty = difficulty;
            return this;
        }

        public QuestionOptions WithQuestionType(QuestionType type)
        {
            Type = type;
            return this;
        }
    }
}
